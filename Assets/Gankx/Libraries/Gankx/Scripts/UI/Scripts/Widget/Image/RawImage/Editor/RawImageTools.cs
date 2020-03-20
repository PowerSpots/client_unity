using System.Collections.Generic;
using System.IO;
using Gankx.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Gankx.UIEditor
{
    public class RawImageTools : EditorWindow
    {
        [MenuItem("Tools/Image/Update RawImage Asset Textures", false, 200)]
        private static void UpdateRawImages()
        {
            var absoluteBasePath = Application.dataPath + "/" + UIConfig.RawImageResourcePath;
            var spriteDirInfo = new DirectoryInfo(absoluteBasePath);
            foreach (var pngFile in spriteDirInfo.GetFiles("*.png", SearchOption.AllDirectories))
            {
                var allPath = pngFile.FullName;
                var assetPath = allPath.Substring(allPath.IndexOf("Assets"));
                var texture = AssetDatabase.LoadAssetAtPath<Texture>(assetPath);
                if (null == texture)
                {
                    Debug.LogError("UpdateRawImages Error. Cannot load texture: " + assetPath);
                    continue;
                }

                SmartRawImage.TexturePathDict[texture.GetInstanceID()] =
                    assetPath.Substring(17, assetPath.Length - 21).Replace('\\', '/');
            }

            foreach (var pngFile in spriteDirInfo.GetFiles("*.jpg", SearchOption.AllDirectories))
            {
                var allPath = pngFile.FullName;
                var assetPath = allPath.Substring(allPath.IndexOf("Assets"));
                var texture = AssetDatabase.LoadAssetAtPath<Texture>(assetPath);
                if (null == texture)
                {
                    Debug.LogError("UpdateRawImages Error. Cannot load texture: " + assetPath);
                    continue;
                }

                SmartRawImage.TexturePathDict[texture.GetInstanceID()] =
                    assetPath.Substring(17, assetPath.Length - 21).Replace('\\', '/');
            }
        }

        [MenuItem("GameObject/UI/Replace RawImage")]
        public static void ReplaceSingle(MenuCommand menuCommand)
        {
            var go = menuCommand.context as GameObject;

            DoReplaceSingle(go, PrefabUtility.GetPrefabParent(go) as GameObject, false);
        }

        private static void DoReplaceSingle(GameObject panelInstance, GameObject prefab,
            bool shouldDestroyInstance = true)
        {
            var imgs = panelInstance.GetComponentsInChildren<RawImage>(true);
            var images = new List<RawImage>();
            for (var j = 0; j < imgs.Length; j++)
            {
                if (!(imgs[j] is SmartRawImage) && imgs[j].texture != null)
                {
                    images.Add(imgs[j]);
                }
            }

            for (var j = 0; j < images.Count; j++)
            {
                var img = Instantiate(images[j]);
                var material = images[j].material;
                if (material.name == images[j].defaultMaterial.name)
                {
                    material = null;
                }

                var enabled = images[j].enabled;

                if (images[j].GetType() == typeof(RawImage))
                {
                    var go = images[j].gameObject;
                    DestroyImmediate(images[j], false);

                    var smartImage = go.AddComponent<SmartRawImage>();
                    if (null == smartImage)
                    {
                        Debug.LogError("Replace Failed, please check reason.", go);
                        continue;
                    }

                    smartImage.enabled = enabled;
                    smartImage.texture = img.texture;
                    smartImage.color = img.color;
                    smartImage.material = material;
                    smartImage.raycastTarget = img.raycastTarget;
                    smartImage.uvRect = img.uvRect;
                }

                DestroyImmediate(img.gameObject);
            }

            PrefabUtility.ReplacePrefab(panelInstance, prefab, ReplacePrefabOptions.ConnectToPrefab);

            if (shouldDestroyInstance)
            {
                DestroyImmediate(panelInstance);
            }
        }

        [MenuItem("Tools/Image/Replace RawImage", false, 200)]
        public static void Replace()
        {
            Debug.Log("Replacing Start...");
            var parent = GameObject.Find("GamePortal/GMCanvas");
            var panels = Resources.LoadAll<GameObject>("ui");
            for (var i = 0; i < panels.Length; i++)
            {
                var imgs = panels[i].GetComponentsInChildren<RawImage>(true);

                if (imgs.Length <= 0)
                {
                    continue;
                }

                var images = new List<RawImage>();
                for (var j = 0; j < imgs.Length; j++)
                {
                    if (imgs[j].GetType() != typeof(AtlasImage) && imgs[j].texture != null)
                    {
                        images.Add(imgs[j]);
                    }
                }

                if (images.Count <= 0)
                {
                    continue;
                }

                var panelInstance = PrefabUtility.InstantiatePrefab(panels[i]) as GameObject;
                if (null == panelInstance)
                {
                    continue;
                }

                panelInstance.transform.SetParent(parent.transform);

                panelInstance.SetActive(false);
                panelInstance.transform.localScale = panels[i].transform.localScale;
                panelInstance.transform.localRotation = panels[i].transform.localRotation;
                panelInstance.GetComponent<RectTransform>().anchoredPosition =
                    panels[i].GetComponent<RectTransform>().anchoredPosition;
                panelInstance.GetComponent<RectTransform>().anchorMin =
                    panels[i].GetComponent<RectTransform>().anchorMin;
                panelInstance.GetComponent<RectTransform>().anchorMax =
                    panels[i].GetComponent<RectTransform>().anchorMax;
                panelInstance.GetComponent<RectTransform>().offsetMin =
                    panels[i].GetComponent<RectTransform>().offsetMin;
                panelInstance.GetComponent<RectTransform>().offsetMax =
                    panels[i].GetComponent<RectTransform>().offsetMax;
                panelInstance.GetComponent<RectTransform>().sizeDelta =
                    panels[i].GetComponent<RectTransform>().sizeDelta;
                DoReplaceSingle(panelInstance, panels[i]);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Replacing Done...");
        }
    }
}