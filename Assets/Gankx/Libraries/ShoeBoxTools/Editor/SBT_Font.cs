/*****************************************************************************/
/* ShoeBox Tools                                                             */
/* support@project-jack.com                                                  */
/*                                                                           */
/* Module Description:                                                       */
/*                                                                           */
/* Imports XML font files exported by ShoeBox and likely other tools as well */
/*                                                                           */
/* Copyright © 2015 project|JACK, LLC                                        */
/*****************************************************************************/

/* MODIFICATION LOG */
/*****************************************************************************/
/*  Date     * Who     * Comment                                             */
/*---------------------------------------------------------------------------*/
/*  4/23/15  * Austin  * Created                                             */
/*****************************************************************************/

using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

public class SBT_Font
{
	private static string directory = "";

    private static string fontRootPath = "Assets/UI_v2/bitmap_font/";    

    [ MenuItem("Tools/Import Bitmap Font") ]
	public static void Init()
	{
		string path = EditorUtility.OpenFilePanel( "Import ShoeBox Font", directory, "xml" );

		if ( !string.IsNullOrEmpty( path ) )
		{
			directory = Path.GetDirectoryName( path );
			Import( Path.GetFullPath( path ) );
		}
	}

	static void Import( string path )
	{
		XElement root = XElement.Load( path );
		XElement common = root.Element( "common" );

		float textureW = (float) common.Attribute( "scaleW" );
		float textureH = (float) common.Attribute( "scaleH" );

		// parse and compute the glyph data
		CharacterInfo[] charInfo = ParseChars( root.Element( "chars" ), textureW, textureH );

		// find and load the font texture
		string assetPath = LoadFontTexture( root.Element( "pages" ) );

		if ( string.IsNullOrEmpty( assetPath ) )
		{
			return;
		}

		// create the font texture material
		Material material = LoadFontMaterial( assetPath );

		if ( material == null )
		{
			return;
		}
		
		// finally create the font
		string fontName = Path.GetFileNameWithoutExtension( path );

		Font font = new Font( fontName );
		font.characterInfo = charInfo;
		font.material = material;

		path = fontRootPath + font.name + ".fontsettings";

        Font destFont = AssetDatabase.LoadMainAssetAtPath(path) as Font;
	    if (null != destFont)
	    {
	        EditorUtility.CopySerialized(font, destFont);	        
	    }
	    else
	    {
	        AssetDatabase.CreateAsset(font, path);
        }
	    
        AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	static CharacterInfo[] ParseChars( XElement chars, float texW, float texH )
	{
		int count = (int) chars.Attribute( "count" );
		CharacterInfo[] charInfo = new CharacterInfo[ count ];

		IEnumerable<XElement> elements = chars.Elements( "char" );
		count = 0;

		foreach( XElement e in elements )
		{
			float x = (float) e.Attribute( "x" );
			float y = (float) e.Attribute( "y" );
			float w = (float) e.Attribute( "width" );
			float h = (float) e.Attribute( "height" );
		    int a = (int)e.Attribute("xadvance");
            float yoffset = (float) e.Attribute( "yoffset" );
			int id = (int) e.Attribute( "id" );

			charInfo[ count ].index = id;
			charInfo[ count ].uv.width = w / texW;
			charInfo[ count ].uv.height = h / texH;
			charInfo[ count ].uv.x = x / texW;
			charInfo[ count ].uv.y = 1f - ( y / texH ) - ( h / texH );
			charInfo[ count ].vert.x = 0;
			charInfo[ count ].vert.y = -yoffset;
			charInfo[ count ].vert.width = w;
			charInfo[ count ].vert.height = -h;
			charInfo[ count ].width = w;
			charInfo[ count ].flipped = false;
			charInfo[ count ].advance = a;

			count++;
		}

		return charInfo;
	}

	static string LoadFontTexture( XElement pages )
	{
		string path = pages.Element( "page" ).Attribute( "file" ).Value;
		path = Path.GetFullPath( path );

		if ( string.IsNullOrEmpty( path ) )
		{
			Debug.LogError( "Invalid Font Import Path" );
			return null;
		}

		string name = Path.GetFileName( path );
		string destPath = Application.dataPath + "/UI_v2/bitmap_font/" + name;
		string assetPath = fontRootPath + name;

	    if (File.Exists(destPath))
	    {
	        File.SetAttributes(destPath, FileAttributes.Normal);	        
            // 如果存在，直接覆盖资源图
	        File.Copy(path, destPath, true);
	        AssetDatabase.Refresh();
        }
	    else
	    {
            // 不存在，需要更改导入设置
	        File.Copy(path, destPath, true);
	        AssetDatabase.Refresh();

	        TextureImporter importer = TextureImporter.GetAtPath(assetPath) as TextureImporter;

	        TextureImporterSettings settings = new TextureImporterSettings();
	        importer.ReadTextureSettings(settings);
	        settings.textureFormat = TextureImporterFormat.AutomaticTruecolor;
	        settings.mipmapEnabled = false;
	        importer.SetTextureSettings(settings);

	        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
	        AssetDatabase.Refresh();
        }
		
		return assetPath;
	}

	static Material LoadFontMaterial( string assetPath )
	{
		if ( string.IsNullOrEmpty( assetPath ) )
		{
			return null;
		}

	    string matPath = fontRootPath + Path.GetFileNameWithoutExtension(assetPath) + ".mat";
	    Material mat = AssetDatabase.LoadAssetAtPath(matPath, typeof(Material)) as Material;
	    if (null == mat)
	    {
	        Shader shader = Shader.Find("Unlit/Transparent");
	        mat = new Material(shader);
	        mat.mainTexture = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture2D)) as Texture2D;
	        AssetDatabase.CreateAsset(mat, matPath);
	    }
	    else
	    {
	        mat.mainTexture = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture2D)) as Texture2D;
        }

	    AssetDatabase.SaveAssets();
		AssetDatabase.Refresh( ImportAssetOptions.ForceSynchronousImport );
		
		return AssetDatabase.LoadAssetAtPath( matPath, typeof(Material) ) as Material;
	}
}
