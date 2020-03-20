using UnityEngine;

namespace Gankx.UI
{
    public class ImageLruHud : MonoBehaviour
    {
        public static ImageLruHud instance;

        private GUIStyle myStyle;
        private Texture2D myBackgroundTex;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            myStyle = new GUIStyle();
            myStyle.fontSize = 24;
            myStyle.fontStyle = FontStyle.Bold;
            myStyle.normal.textColor = Color.black;

            myBackgroundTex = new Texture2D(4, 4);
            var colors = myBackgroundTex.GetPixels();
            for (var i = 0; i < colors.Length; i++)
            {
                colors[i] = new Color(1, 1, 1, 0.2f);
            }

            myBackgroundTex.SetPixels(colors);
        }

        private void OnGUI()
        {
            GUI.DrawTexture(new Rect(0, 0, 1920, 1080), myBackgroundTex);
            var rect = new Rect(0, 0, 500, 400);
            GUI.TextArea(rect, " atlas:\n" + AtlasManager.instance.CacheFeed().Replace(',', '\n'), myStyle);
            rect = new Rect(0, 420, 500, 200);
            GUI.TextArea(rect, " raw image:\n" + RawImageAssetTextureManager.instance.CacheFeed().Replace(',', '\n'),
                myStyle);
            rect = new Rect(0, 640, 500, 500);
            GUI.TextArea(rect, " bundle sprite:\n" + BundleSpriteManager.instance.CacheFeed().Replace(',', '\n'),
                myStyle);
        }
    }
}