﻿////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

using UnityEngine;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/FX/Spot")]
public class CameraFilterPack_FX_Spot : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
private Vector4 ScreenResolution;
private Material SCMaterial;
public Vector2 center = new Vector2(0.5f,0.5f);
public float Radius = 0.2f;
#endregion
#region Properties
Material material
{
get
{
if(SCMaterial == null)
{
SCMaterial = new Material(SCShader);
SCMaterial.hideFlags = HideFlags.HideAndDontSave;	
}
return SCMaterial;
}
}
#endregion
void Start () 
{
SCShader = Shader.Find("CameraFilterPack/FX_Spot");
if(!SystemInfo.supportsImageEffects)
{
enabled = false;
return;
}
}
void OnRenderImage (RenderTexture sourceTexture, RenderTexture destTexture)
{
if(SCShader != null)
{
TimeX+=Time.deltaTime;
if (TimeX>100)  TimeX=0;
material.SetFloat("_TimeX", TimeX);
material.SetFloat("_PositionX", center.x);
material.SetFloat("_PositionY", center.y);
material.SetFloat("_Radius", Radius);
material.SetVector("_ScreenResolution",new Vector4(sourceTexture.width,sourceTexture.height,0.0f,0.0f));
Graphics.Blit(sourceTexture, destTexture, material);
}
else
{
Graphics.Blit(sourceTexture, destTexture);	
}
}
// Update is called once per frame
void Update () 
{
#if UNITY_EDITOR
if (Application.isPlaying!=true)
{
SCShader = Shader.Find("CameraFilterPack/FX_Spot");
}
#endif
}
void OnDisable ()
{
if(SCMaterial)
{
DestroyImmediate(SCMaterial);	
}
}
}