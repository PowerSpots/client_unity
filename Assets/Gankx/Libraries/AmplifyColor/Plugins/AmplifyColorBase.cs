// Amplify Color - Advanced Color Grading for Unity Pro
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5_OR_NEWER
#define UNITY_5_0_OR_NEWER
#endif

using System;
using System.Collections.Generic;
using Gankx;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace AmplifyColor
{
	public enum Quality
	{
		Mobile,
		Standard
	}

	public enum Tonemapping
	{
		Disabled = 0,
		Photographic = 1,
		FilmicHable = 2,
		FilmicACES = 3
	}
}

[AddComponentMenu( "" )]
public class AmplifyColorBase : MonoBehaviour
{
	public const int LutSize = 32;
	public const int LutWidth = LutSize * LutSize;
	public const int LutHeight = LutSize;

	/*
	// HDR Control
	public AmplifyColor.Tonemapping Tonemapper = AmplifyColor.Tonemapping.Disabled;
	public float Exposure = 1.0f;
	public float LinearWhitePoint = 11.2f;
	[FormerlySerializedAs( "UseDithering" )]
	public bool ApplyDithering = false;
	*/

	// Color Grading
	public AmplifyColor.Quality QualityLevel = AmplifyColor.Quality.Mobile;
	public float BlendAmount = 0f;
	public Texture LutTexture = null;
	public Texture LutBlendTexture = null;
    public Color m_GhostColor = new Color(0.302f, 0.624f, 1f, 0.749f);

    private Camera ownerCamera = null;
	private Shader shaderBase = null;
	private Shader shaderBlend = null;
	private Shader shaderBlendCache = null;
	private Shader shaderProcessOnly = null;
	private RenderTexture blendCacheLut = null;
	private Texture2D defaultLut = null;
	private ColorSpace colorSpace = ColorSpace.Uninitialized;
	private AmplifyColor.Quality qualityLevel = AmplifyColor.Quality.Standard;

	public Texture2D DefaultLut { get { return ( defaultLut == null ) ? CreateDefaultLut() : defaultLut ; } }

	private Material materialBase = null;
	private Material materialBlend = null;
	private Material materialBlendCache = null;
	private Material materialProcessOnly = null;

	private bool blending;
	private float blendingTime;
	private float blendingTimeCountdown;
	private System.Action onFinishBlend;

	public bool IsBlending { get { return blending; } }

	private Texture worldLUT = null;
	private RenderTexture midBlendLUT = null;
	private bool blendingFromMidBlend = false;

	private float worldExposure = 1.0f;
	private float currentExposure = 1.0f;
	private float blendExposure = 1.0f;

	[SerializeField, HideInInspector] private string sharedInstanceID = "";
	public string SharedInstanceID { get { return sharedInstanceID; } }

	private bool silentError = false;

	public bool WillItBlend { get { return LutTexture != null && LutBlendTexture != null && !blending; } }

	public void NewSharedInstanceID()
	{
		sharedInstanceID = Guid.NewGuid().ToString();
	}

	void ReportMissingShaders()
	{
		Debug.LogError( "[AmplifyColor] Failed to initialize shaders. Please attempt to re-enable the Amplify Color Effect component. If that fails, please reinstall Amplify Color." );
		enabled = false;
	}

	void ReportNotSupported()
	{
		Debug.LogError( "[AmplifyColor] This image effect is not supported on this platform." );
		enabled = false;
	}

	bool CheckShader( Shader s )
	{
		if ( s == null )
		{
			ReportMissingShaders();
			return false;
		}
		if ( !s.isSupported )
		{
			ReportNotSupported();
			return false;
		}
		return true;
	}

	bool CheckShaders()
	{
		return CheckShader( shaderBase ) && CheckShader( shaderBlend ) && CheckShader( shaderBlendCache ) && CheckShader( shaderProcessOnly );
	}

	bool CheckSupport()
	{
		// Disable if we don't support image effect or render textures
	#if UNITY_5_5_OR_NEWER
		if ( !SystemInfo.supportsImageEffects )
	#else
		if ( !SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures )
	#endif
		{
			ReportNotSupported();
			return false;
		}
		return true;
	}

	void OnEnable()
	{
	#if UNITY_5_0_OR_NEWER
		bool nullDev = ( SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null );
	#else
		bool nullDev = ( SystemInfo.graphicsDeviceName == "Null Device" );
	#endif
		if ( nullDev )
		{
			Debug.LogWarning( "[AmplifyColor] Null graphics device detected. Skipping effect silently." );
			silentError = true;
			return;
		}

		if ( !CheckSupport() )
			return;

		if ( !CreateMaterials() )
			return;

		Texture2D lutTex2d = LutTexture as Texture2D;
		Texture2D lutBlendTex2d = LutBlendTexture as Texture2D;

		if ( ( lutTex2d != null && lutTex2d.mipmapCount > 1 ) || ( lutBlendTex2d != null && lutBlendTex2d.mipmapCount > 1 ) )
			Debug.LogError( "[AmplifyColor] Please disable \"Generate Mip Maps\" import settings on all LUT textures to avoid visual glitches. " +
				"Change Texture Type to \"Advanced\" to access Mip settings." );

        Shader.SetGlobalColor("_GhostColor", m_GhostColor);
	}

	void OnDisable()
	{
		ReleaseMaterials();
		ReleaseTextures();
	}

	public void BlendTo( Texture blendTargetLUT, float blendTimeInSec, System.Action onFinishBlend )
	{
		LutBlendTexture = blendTargetLUT;
		BlendAmount = 0.0f;
		this.onFinishBlend = onFinishBlend;
		blendingTime = blendTimeInSec;
		blendingTimeCountdown = blendTimeInSec;
		blending = true;
	}

	private void CheckCamera()
	{
		if  ( ownerCamera == null )
			ownerCamera = GetComponent<Camera>();
	}

	private void Start()
	{
		if ( silentError )
			return;

		CheckCamera();

		worldLUT = LutTexture;

		/*
		worldExposure = Exposure;
		blendExposure = currentExposure = worldExposure;
		*/
	}

	void Update()
	{
		if ( silentError )
			return;

		CheckCamera();

		if ( blending )
		{
			BlendAmount = ( blendingTime - blendingTimeCountdown ) / blendingTime;
			blendingTimeCountdown -= Time.smoothDeltaTime;

			if ( BlendAmount >= 1.0f )
			{
				LutTexture = LutBlendTexture;
				BlendAmount = 0.0f;
				blending = false;
				LutBlendTexture = null;

				if ( onFinishBlend != null )
					onFinishBlend();
			}
		}
		else
			BlendAmount = Mathf.Clamp01( BlendAmount );
	}

	private void SetupShader()
	{
		colorSpace = QualitySettings.activeColorSpace;
		qualityLevel = QualityLevel;

		shaderBase = Shader.Find( "Hidden/Amplify Color/Base" );
		shaderBlend = Shader.Find( "Hidden/Amplify Color/Blend" );
		shaderBlendCache = Shader.Find( "Hidden/Amplify Color/BlendCache" );
		shaderProcessOnly = Shader.Find( "Hidden/Amplify Color/ProcessOnly" );
	}

	private void ReleaseMaterials()
	{
		SafeRelease( ref materialBase );
		SafeRelease( ref materialBlend );
		SafeRelease( ref materialBlendCache );
		SafeRelease( ref materialProcessOnly );
	}

	private Texture2D CreateDefaultLut()
	{
		const int maxSize = LutSize - 1;

		defaultLut = new Texture2D( LutWidth, LutHeight, TextureFormat.RGB24, false, true ) { hideFlags = HideFlags.HideAndDontSave };
		defaultLut.name = "DefaultLut";
		defaultLut.hideFlags = HideFlags.DontSave;
		defaultLut.anisoLevel = 1;
		defaultLut.filterMode = FilterMode.Bilinear;
		Color32[] colors = new Color32[ LutWidth * LutHeight ];

		for ( int z = 0; z < LutSize; z++ )
		{
			int zoffset = z * LutSize;

			for ( int y = 0; y < LutSize; y++ )
			{
				int yoffset = zoffset + y * LutWidth;

				for ( int x = 0; x < LutSize; x++ )
				{
					float fr = x / ( float ) maxSize;
					float fg = y / ( float ) maxSize;
					float fb = z / ( float ) maxSize;
					byte br = ( byte ) ( fr * 255 );
					byte bg = ( byte ) ( fg * 255 );
					byte bb = ( byte ) ( fb * 255 );
					colors[ yoffset + x ] = new Color32( br, bg, bb, 255 );
				}
			}
		}

		defaultLut.SetPixels32( colors );
		defaultLut.Apply();

		return defaultLut;
	}

	private void CreateHelperTextures()
	{
		ReleaseTextures();

		blendCacheLut = ResourceService.CreateRendertTexture( LutWidth, LutHeight, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear );
		blendCacheLut.hideFlags = HideFlags.HideAndDontSave;
		blendCacheLut.name = "BlendCacheLut";
		blendCacheLut.wrapMode = TextureWrapMode.Clamp;
		blendCacheLut.useMipMap = false;
		blendCacheLut.anisoLevel = 0;
		blendCacheLut.Create();

		midBlendLUT = ResourceService.CreateRendertTexture(LutWidth, LutHeight, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		midBlendLUT.hideFlags = HideFlags.HideAndDontSave;
		midBlendLUT.name = "MidBlendLut";
		midBlendLUT.wrapMode = TextureWrapMode.Clamp;
		midBlendLUT.useMipMap = false;
		midBlendLUT.anisoLevel = 0;
		midBlendLUT.Create();
	#if !UNITY_5_0_OR_NEWER
		midBlendLUT.MarkRestoreExpected();
	#endif

		CreateDefaultLut();
	}

	bool CheckMaterialAndShader( Material material, string name )
	{
		if ( material == null || material.shader == null )
		{
			Debug.LogWarning( "[AmplifyColor] Error creating " + name + " material. Effect disabled." );
			enabled = false;
		}
		else if ( !material.shader.isSupported )
		{
			Debug.LogWarning( "[AmplifyColor] " + name + " shader not supported on this platform. Effect disabled." );
			enabled = false;
		}
		else
		{
			material.hideFlags = HideFlags.HideAndDontSave;
		}
		return enabled;
	}

	private bool CreateMaterials()
	{
		SetupShader();
		if ( !CheckShaders() )
			return false;

		ReleaseMaterials();

		materialBase = new Material( shaderBase );
		materialBlend = new Material( shaderBlend );
		materialBlendCache = new Material( shaderBlendCache );
		materialProcessOnly = new Material( shaderProcessOnly );

		bool ok = true;
		ok = ok && CheckMaterialAndShader( materialBase, "BaseMaterial" );
		ok = ok && CheckMaterialAndShader( materialBlend, "BlendMaterial" );
		ok = ok && CheckMaterialAndShader( materialBlendCache, "BlendCacheMaterial" );
		ok = ok && CheckMaterialAndShader( materialProcessOnly, "ProcessOnlyMaterial" );

		if ( !ok )
			return false;

		CreateHelperTextures();
		return true;
	}

	void SetMaterialKeyword( string keyword, bool state )
	{
	#if !UNITY_5_0_OR_NEWER
		if ( state )
			Shader.EnableKeyword( keyword );
		else
			Shader.DisableKeyword( keyword );
	#else
		bool keywordEnabled = materialBase.IsKeywordEnabled( keyword );
		if ( state && !keywordEnabled )
		{
			materialBase.EnableKeyword( keyword );
			materialBlend.EnableKeyword( keyword );
			materialBlendCache.EnableKeyword( keyword );
			materialProcessOnly.EnableKeyword( keyword );
		}
		else if ( !state && materialBase.IsKeywordEnabled( keyword ) )
		{
			materialBase.DisableKeyword( keyword );
			materialBlend.DisableKeyword( keyword );
			materialBlendCache.DisableKeyword( keyword );
			materialProcessOnly.DisableKeyword( keyword );
		}
	#endif
	}

	private void SafeRelease<T>( ref T obj ) where T : UnityEngine.Object
	{
		if ( obj != null )
		{
			if ( obj.GetType() == typeof( RenderTexture ) )
				( obj as RenderTexture ).Release();

			DestroyImmediate( obj );
			obj = null;
		}
	}

	private void ReleaseTextures()
	{
		RenderTexture.active = null;
		SafeRelease( ref blendCacheLut );
		SafeRelease( ref midBlendLUT );
		SafeRelease( ref defaultLut );
	}

	public static bool ValidateLutDimensions( Texture lut )
	{
		bool valid = true;
		if ( lut != null )
		{
			if ( ( lut.width / lut.height ) != lut.height )
			{
				Debug.LogWarning( "[AmplifyColor] Lut " + lut.name + " has invalid dimensions." );
				valid = false;
			}
			else
			{
				if ( lut.anisoLevel != 0 )
					lut.anisoLevel = 0;
			}
		}
		return valid;
	}

	private void UpdatePostEffectParams()
	{
		/*
		if ( UseDepthMask )
			CheckUpdateDepthCurveLut();
		Exposure = Mathf.Max( Exposure, 0 );
		*/
	}

	private int ComputeShaderPass()
	{
	#if UNITY_STANDALONE
		bool isMobile = false; // ( QualityLevel == AmplifyColor.Quality.Mobile );
	#else
		bool isMobile = true;
	#endif
		return isMobile ? 18 : 0;

		/*
		bool isLinear = ( colorSpace == ColorSpace.Linear );
	#if UNITY_5_6_OR_NEWER
		bool isHDR = ownerCamera.allowHDR;
	#else
		bool isHDR = ownerCamera.hdr;
	#endif

		int pass = isMobile ? 18 : 0;
		if ( isHDR )
		{
			pass += 2;						// skip LDR
			pass += isLinear ? 8 : 0;		// skip GAMMA, if applicable
			pass += ApplyDithering ? 4 : 0; // skip DITHERING, if applicable
			pass += ( int ) Tonemapper;
		}
		else
		{
			pass += isLinear ? 1 : 0;
		}
		return pass;
		*/
	}

	private void OnRenderImage( RenderTexture source, RenderTexture destination ) {
	    bool sourceIsNull = source == null;
	    if (sourceIsNull)
	        source = ScreenRTMgr.instance.GetScreenRT();

		if ( silentError )
		{
			Graphics.Blit( source, destination );
			return;
		}

		BlendAmount = Mathf.Clamp01( BlendAmount );

        if (colorSpace != QualitySettings.activeColorSpace || qualityLevel != QualityLevel || materialBase == null)
            CreateMaterials();

		UpdatePostEffectParams();

		bool validLut = ValidateLutDimensions( LutTexture );
		bool validLutBlend = ValidateLutDimensions( LutBlendTexture );
		bool skip = ( LutTexture == null && LutBlendTexture == null );

		Texture lut = ( LutTexture == null ) ? defaultLut : LutTexture;
		Texture lutBlend = LutBlendTexture;

		int pass = ComputeShaderPass();

		bool blend = ( BlendAmount != 0.0f ) || blending;
		bool requiresBlend = blend || ( blend && lutBlend != null );
		bool useBlendCache = requiresBlend;
		bool processOnly = !validLut || !validLutBlend || skip;

		Material material;
		if ( processOnly )
		{
			material = materialProcessOnly;
		}
		else
		{
			material = ( requiresBlend ) ? materialBlend : materialBase;
		}

		/*
		// HDR control params
		material.SetFloat( "_Exposure", Exposure );
		material.SetFloat( "_ShoulderStrength", 0.22f );
		material.SetFloat( "_LinearStrength", 0.30f );
		material.SetFloat( "_LinearAngle", 0.10f );
		material.SetFloat( "_ToeStrength", 0.20f );
		material.SetFloat( "_ToeNumerator", 0.01f );
		material.SetFloat( "_ToeDenominator", 0.30f );
		material.SetFloat( "_LinearWhite", LinearWhitePoint );
		*/

		// Blending params
		material.SetFloat( "_LerpAmount", BlendAmount );

		if ( !processOnly )
		{
			if ( useBlendCache )
			{
				materialBlendCache.SetFloat( "_LerpAmount", BlendAmount );

				RenderTexture temp = null;

				materialBlendCache.SetTexture( "_RgbTex", lut );

				materialBlendCache.SetTexture( "_LerpRgbTex", ( lutBlend != null ) ? lutBlend : defaultLut );

				Graphics.Blit( lut, blendCacheLut, materialBlendCache );

				if ( temp != null )
					RenderTexture.ReleaseTemporary( temp );

				material.SetTexture( "_RgbBlendCacheTex", blendCacheLut );

			}
			else
			{
				if ( lut != null )
					material.SetTexture( "_RgbTex", lut );
				if ( lutBlend != null )
					material.SetTexture( "_LerpRgbTex", lutBlend );
			}
		}

	    if (destination == null && ScreenRTMgr.instance.screenRT != null && source != ScreenRTMgr.instance.screenRT) {
	        Graphics.Blit(source, ScreenRTMgr.instance.GetScreenRT(), material, pass);
        }
	    else {
	        Graphics.Blit(source, destination, material, pass);
	        if (sourceIsNull) {
	            Graphics.Blit(destination, ScreenRTMgr.instance.GetScreenRT());
	        }
        }

		if ( useBlendCache )
			blendCacheLut.DiscardContents();
	}
}
