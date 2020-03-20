using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Gankx.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Gankx.UIEditor
{
    public class AtlasImageTools : EditorWindow
    {
        [MenuItem("Tools/Image/Pack All Atlas", false, 0)]
        public static void PackAllAtlas()
        {
            AtlasImage.SpriteAtlasDict.Clear();
            var subFolders = AssetDatabase.GetSubFolders(UIConfig.AtlasAssetPath);
            for (var i = 0; i < subFolders.Length; ++i)
            {
                PackAtlasUnderPath(subFolders[i]);
            }

            UpdateBundleSprites();
        }

        private static void UpdateBundleSprites()
        {
            for (var i = 0; i < UIConfig.BundleSpritePaths.Length; i++)
            {
                var absoluteBasePath = Application.dataPath + "/" + UIConfig.BundleSpritePaths[i];
                var spriteDirInfo = new DirectoryInfo(absoluteBasePath);
                foreach (var pngFile in spriteDirInfo.GetFiles("*.png", SearchOption.TopDirectoryOnly))
                {
                    var allPath = pngFile.FullName;
                    var assetPath = allPath.Substring(allPath.IndexOf("Assets"));
                    var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                    if (null == sprite)
                    {
                        Debug.LogError("UpdateBundleSprites Error. Cannot load sprite: " + assetPath);
                        continue;
                    }

                    var convertedBundlePrefix = UIConfig.BundleSpritePaths[i].Replace('/', '_');
                    AtlasImage.SpriteBundleNameDict[sprite.GetInstanceID()] =
                        string.Format("{0}_{1}.u", convertedBundlePrefix, sprite.name);
                }
            }
        }


        [MenuItem("Tools/Image/Pack Selected Atlas", false, 0)]
        public static void PackAtlas()
        {
            var basePath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(basePath))
            {
                Debug.LogError("Please select the sprite folder to pack!!!");
                return;
            }

            PackAtlasUnderPath(basePath);
        }

        public static void PackAtlasUnderPath(string basePath)
        {
            var atlasDirInfo = new DirectoryInfo(Application.dataPath + "/" + UIConfig.AtlasResourcePath);
            if (!atlasDirInfo.Exists)
            {
                atlasDirInfo.Create();
            }

            var prefix = "Assets/";
            basePath = basePath.Substring(prefix.Length);
            if (!basePath.StartsWith(UIConfig.AtlasBasePath))
            {
                Debug.LogError("The source folder should be under: " + UIConfig.AtlasBasePath);
                return;
            }

            var absoluteBasePath = Application.dataPath + "/" + basePath;
            if (!Directory.Exists(absoluteBasePath))
            {
                Debug.LogError("Please select the sprite folder to pack!");
                return;
            }

            var spriteDirInfo = new DirectoryInfo(absoluteBasePath);
            var lastIndexOfSlash = basePath.LastIndexOf('/');
            if (lastIndexOfSlash < 0)
            {
                return;
            }

            var folderName = basePath.Substring(lastIndexOfSlash + 1);

            var atlasName = "";
            if (!basePath.Equals(UIConfig.AtlasBasePath))
            {
                atlasName = "atlas" + UIConfig.AtlasNameSeparator + folderName;
                atlasName = atlasName.ToLower();
            }

            PackDir(spriteDirInfo, atlasName);
        }

        private static void PackDir(DirectoryInfo dirInfo, string atlasName)
        {
            if (atlasName != "")
            {
                var prefabPath = "Assets/" + UIConfig.AtlasResourcePath + "/" + atlasName + ".prefab";
                Object obj = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                GameObject go;
                AtlasAsset atlasAsset;
                if (obj != null)
                {
                    go = PrefabUtility.InstantiatePrefab(obj) as GameObject;
                    go.name = atlasName;
                    atlasAsset = go.GetComponent<AtlasAsset>();
                    atlasAsset.RemoveAll();
                }
                else
                {
                    go = new GameObject(atlasName);
                    atlasAsset = go.AddComponent<AtlasAsset>();
                }

                atlasAsset.atlasName = atlasName;
                foreach (var pngFile in dirInfo.GetFiles("*.png", SearchOption.TopDirectoryOnly))
                {
                    var allPath = pngFile.FullName;
                    var assetPath = allPath.Substring(allPath.IndexOf("Assets"));
                    var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                    if (null == sprite)
                    {
                        Debug.LogError("PackAtlas Error. Cannot load sprite: " + assetPath);
                        continue;
                    }

                    AtlasImage.SpriteAtlasDict[sprite.GetInstanceID()] = atlasName;

                    var indexOfSlash = sprite.name.IndexOf('/');
                    if (indexOfSlash != -1)
                    {
                        sprite.name = sprite.name.Substring(indexOfSlash + 1);
                    }

                    atlasAsset.AddSprite(sprite);
                }

                if (obj != null && go != null)
                {
                    PrefabUtility.ReplacePrefab(go, obj);
                }
                else
                {
                    var atlasPath = UIConfig.AtlasResourcePath + "/" + atlasName + ".prefab";
                    PrefabUtility.CreatePrefab("Assets/" + atlasPath, go);
                }

                DestroyImmediate(go);
            }

            foreach (var subDirInfo in dirInfo.GetDirectories())
            {
                var subAtlasName = atlasName + UIConfig.AtlasNameSeparator + subDirInfo.Name;
                if (atlasName == "")
                {
                    subAtlasName = subDirInfo.Name;
                }

                PackDir(subDirInfo, subAtlasName);
            }
        }

        [MenuItem("Tools/Image/Arrange AtlasImage Sprites By spritePackingTag", false, 0)]
        public static void ArrangeAtlasImageSprites()
        {
            var selections = Selection.GetFiltered<Object>(SelectionMode.Assets);
            if (selections.Length <= 0)
            {
                return;
            }

            var panelPrefabs = Resources.LoadAll<GameObject>(UIConfig.PanelSimplePath);

            var folderPath = AssetDatabase.GetAssetPath(selections[0]);
            if (!folderPath.Contains(UIConfig.AtlasAssetPath))
            {
                Debug.LogError("The foler can only be under " + UIConfig.AtlasAssetPath);
                return;
            }

            var folderAtlas = folderPath.Substring(folderPath.LastIndexOf('/') + 1);
            var oldAtlasPrefix = UIConfig.AtlasSimplePath + "/" + folderAtlas + "/";

            var outputStrList = new List<string>();
            var filePaths = Directory.GetFiles(folderPath, "*.png");
            for (var i = 0; i < filePaths.Length; i++)
            {
                var oldPath = filePaths[i].Replace('\\', '/');
                var fileName = filePaths[i].Substring(filePaths[i].LastIndexOf('\\') + 1,
                    filePaths[i].LastIndexOf('.') - filePaths[i].LastIndexOf('\\') - 1);
                var textureImporter = AssetImporter.GetAtPath(filePaths[i]) as TextureImporter;
                if (textureImporter != null && !string.IsNullOrEmpty(textureImporter.spritePackingTag) &&
                    textureImporter.spritePackingTag != folderAtlas)
                {
                    var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(filePaths[i]);

                    var oldAtlasPath = oldAtlasPrefix + fileName;
                    var newAtlasPrefix = UIConfig.AtlasSimplePath + "/" + textureImporter.spritePackingTag + "/";
                    var newAtlasPath = oldAtlasPath.Replace(oldAtlasPrefix, newAtlasPrefix);
                    var newPath = oldPath.Replace(oldAtlasPrefix, newAtlasPrefix);
                    var newFolder = newPath.Substring(0, newPath.LastIndexOf('/'));
                    var newParentFolder = newFolder.Substring(0, newFolder.LastIndexOf('/'));
                    if (!AssetDatabase.IsValidFolder(newFolder))
                    {
                        AssetDatabase.CreateFolder(newParentFolder, textureImporter.spritePackingTag);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }

                    var moveResult = AssetDatabase.MoveAsset(oldPath, newPath);
                    if (!string.IsNullOrEmpty(moveResult))
                    {
                        Debug.LogError(string.Format("Move Asset Failed, {0}-->{1}. Reason: {2}", oldPath, newPath,
                            moveResult), sprite);
                        AssetDatabase.DeleteAsset(oldPath);
                        continue;
                    }

                    textureImporter.spritePackingTag = "";
                    Debug.Log(string.Format("{0}-->{1}", oldAtlasPath,
                        newAtlasPath), sprite);

                    var output = string.Format("{0}-->{1}", oldAtlasPath,
                        newAtlasPath);
                    if (!outputStrList.Contains(output))
                    {
                        outputStrList.Add(output);
                    }

                    for (var j = 0; j < panelPrefabs.Length; j++)
                    {
                        var overrides = panelPrefabs[j].GetComponentsInChildren<SlotControlOverride>(true);
                        var isDirty = false;
                        for (var k = 0; k < overrides.Length; k++)
                        {
                            if (overrides[k].overrideEnum == SlotControlOverrideEnum.Image)
                            {
                                if (overrides[k].overrideInfo == oldAtlasPath)
                                {
                                    overrides[k].overrideInfo = newAtlasPath;
                                    isDirty = true;
                                }
                            }
                        }

                        if (isDirty)
                        {
                            var panelInstnace = PrefabUtility.InstantiatePrefab(panelPrefabs[j]) as GameObject;
                            PrefabUtility.ReplacePrefab(panelInstnace, PrefabUtility.GetPrefabParent(panelInstnace));
                            Debug.Log("Apply prefab: " + panelInstnace.name);
                            DestroyImmediate(panelInstnace);
                        }
                    }
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            var result = "";
            for (var i = 0; i < outputStrList.Count; i++)
            {
                result = result + outputStrList[i] + "\n";
            }

            Debug.Log(result);
        }

        [MenuItem("GameObject/UI/Replace AtlasImage")]
        public static void ReplaceSingle(MenuCommand menuCommand)
        {
            var go = menuCommand.context as GameObject;

            DoReplaceSingle(go, PrefabUtility.GetPrefabParent(go) as GameObject, false);
        }

        private static void DoReplaceSingle(GameObject panelInstance, GameObject prefab,
            bool shouldDestroyInstance = true)
        {
            var imgs = panelInstance.GetComponentsInChildren<Image>(true);
            var images = new List<Image>();
            for (var j = 0; j < imgs.Length; j++)
            {
                if (!(imgs[j] is AtlasImage))
                {
                    images.Add(imgs[j]);
                }
            }

            for (var i = 0; i < images.Count; i++)
            {
                var img = Instantiate(images[i]);
                var material = images[i].material;
                if (material.name == images[i].defaultMaterial.name)
                {
                    material = null;
                }

                var enabled = images[i].enabled;

                var fieldsDict = new Dictionary<FieldInfo, MonoBehaviour>();

                var behaviours = panelInstance.GetComponentsInChildren<MonoBehaviour>(true);
                for (var j = 0; j < behaviours.Length; j++)
                {
                    var fieldInfos = new List<FieldInfo>();
                    var curType = behaviours[j].GetType();
                    while (curType != null)
                    {
                        var fieldInfoArray = curType
                            .GetFields(
                                BindingFlags.FlattenHierarchy |
                                BindingFlags.Instance |
                                BindingFlags.NonPublic |
                                BindingFlags.Public);

                        for (var m = 0; m < fieldInfoArray.Length; m++)
                        {
                            fieldInfos.Add(fieldInfoArray[m]);
                        }

                        curType = curType.BaseType;
                    }

                    foreach (var info in fieldInfos)
                    {
                        if (typeof(Graphic).IsAssignableFrom(info.FieldType))
                        {
                            if ((Image) info.GetValue(behaviours[j]) == images[i])
                            {
                                fieldsDict[info] = behaviours[j];
                            }
                        }
                    }
                }

                var go = images[i].gameObject;
                DestroyImmediate(images[i], false);
                var atlasImage = go.AddComponent<AtlasImage>();
                if (null == atlasImage)
                {
                    Debug.LogError("Replace Failed, please check reason.", go);
                    continue;
                }

                atlasImage.enabled = enabled;
                atlasImage.sprite = img.sprite;
                atlasImage.color = img.color;
                atlasImage.material = material;
                atlasImage.raycastTarget = img.raycastTarget;
                atlasImage.type = img.type;
                atlasImage.fillCenter = img.fillCenter;
                atlasImage.fillMethod = img.fillMethod;
                atlasImage.fillOrigin = img.fillOrigin;
                atlasImage.fillAmount = img.fillAmount;
                atlasImage.fillClockwise = img.fillClockwise;
                atlasImage.preserveAspect = img.preserveAspect;

                foreach (var pair in fieldsDict)
                {
                    pair.Key.SetValue(pair.Value, atlasImage);
                }

                DestroyImmediate(img.gameObject);
            }

            PrefabUtility.ReplacePrefab(panelInstance, prefab, ReplacePrefabOptions.ConnectToPrefab);

            if (shouldDestroyInstance)
            {
                DestroyImmediate(panelInstance);
            }
        }

        [MenuItem("Tools/Image/Replace AtlasImage", false, 0)]
        public static void Replace()
        {
            Debug.Log("Replacing Start...");
            var parent = GameObject.Find("GamePortal/GMCanvas");
            var panels = Resources.LoadAll<GameObject>("ui");
            for (var i = 0; i < panels.Length; i++)
            {
                var imgs = panels[i].GetComponentsInChildren<Image>(true);

                if (imgs.Length <= 0)
                {
                    continue;
                }

                var images = new List<Image>();
                for (var j = 0; j < imgs.Length; j++)
                {
                    if (imgs[j].GetType() != typeof(AtlasImage))
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

        [MenuItem("Tools/Image/Replace AtlasImage Missing Sprite", false, 0)]
        public static void ReplaceMissingSprite()
        {
            EditorUtility.DisplayProgressBar("ReplaceMissingSprite", "Please wait...", 0);

            var atlasAsset = ResourceService.LoadAtlas(UIConfig.SharedAtlasName);
            if (null == atlasAsset)
            {
                return;
            }

            var whitesprite = atlasAsset.GetSprite(UIConfig.SharedAtlasWhiteSprite);
            if (null == whitesprite)
            {
                return;
            }

            var panels = Resources.LoadAll<GameObject>(UIConfig.PanelSimplePath);
            for (var i = 0; i < panels.Length; i++)
            {
                var needSave = false;
                var panel = panels[i];
                var imgs = panel.GetComponentsInChildren<AtlasImage>(true);
                if (imgs.Length <= 0)
                {
                    continue;
                }

                for (var j = 0; j < imgs.Length; j++)
                {
                    var image = imgs[j];
                    if (null == image.sprite)
                    {
                        image.sprite = whitesprite;
                        needSave = true;
                    }
                }

                if (needSave)
                {
                    var instance = PrefabUtility.InstantiatePrefab(panel) as GameObject;
                    PrefabUtility.ReplacePrefab(instance, panel, ReplacePrefabOptions.ConnectToPrefab);
                    DestroyImmediate(instance);
                }

                EditorUtility.DisplayProgressBar("ReplaceMissingSprite", panel.name, i / (float) panels.Length);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.ClearProgressBar();
        }

        [MenuItem("Tools/Image/Find AtlasImage Sprite Dependency", false, 0)]
        public static void FindDependency()
        {
            var textures = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);
            if (textures.Length <= 0)
            {
                Debug.LogError("Please select one sprite in assets!");
                return;
            }

            var tex = textures[0];
            var path = AssetDatabase.GetAssetPath(tex);

            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (null == sprite)
            {
                Debug.LogError("Please select one sprite in assets!");
                return;
            }

            var parent = GameObject.Find(UIConfig.PanelEditorCanvas);
            if (null == parent)
            {
                Debug.LogError("Can not find panel editor canvas in scene!");
                return;
            }

            var panels = Resources.LoadAll<GameObject>(UIConfig.PanelSimplePath);
            for (var i = 0; i < panels.Length; i++)
            {
                var images = panels[i].GetComponentsInChildren<Image>(true);
                var hasDependency = false;
                for (var j = 0; j < images.Length; j++)
                {
                    if (images[j].sprite == sprite)
                    {
                        hasDependency = true;
                        break;
                    }
                }

                if (hasDependency)
                {
                    var panelInstance = PrefabUtility.InstantiatePrefab(panels[i]) as GameObject;
                    if (null == panelInstance)
                    {
                        continue;
                    }

                    panelInstance.SetActive(false);
                    panelInstance.transform.SetParent(parent.transform);
                    panelInstance.transform.localPosition = Vector3.zero;
                    panelInstance.transform.localRotation = Quaternion.identity;
                    panelInstance.transform.localScale = Vector3.one;

                    images = panelInstance.GetComponentsInChildren<Image>(true);
                    for (var j = 0; j < images.Length; j++)
                    {
                        if (images[j].sprite == sprite)
                        {
                            Debug.Log(string.Format("Find dependency: {0} --> {1}", panelInstance.name, images[j].name),
                                images[j]);
                        }
                    }
                }
            }
        }
    }
}