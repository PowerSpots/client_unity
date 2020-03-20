using System;
using System.Collections;
using System.IO;
using Gankx.IO;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Gankx.UI
{
    [RequireComponent(typeof(RawImage))]
    public class RawImageLocalLoader : MonoBehaviour
    {
        private const string LocalFilePath = UIConfig.LocalJpgPath;
        private string myFolderpath;
        private RawImage myRawImage;
        private Texture2D myTexture;

        private Coroutine myLoadcoroutine;
        private WWW myWww;

        [FormerlySerializedAs("UseRawImageSize")]
        public bool useRawImageSize = false;

        [FormerlySerializedAs("SetWithFullPath")]
        public bool setWithFullPath = false;

        [FormerlySerializedAs("WithPostFix")]
        public bool pathWithPostfix = false;

        [FormerlySerializedAs("m_InitJpg")]
        public string initJpg = "";

        public Texture2D texture
        {
            get
            {
                if (myTexture == null)
                {
                    if (useRawImageSize)
                    {
                        myTexture = new Texture2D((int) myRawImage.uvRect.width, (int) myRawImage.uvRect.height,
                            TextureFormat.RGB24, false);
                    }
                    else
                    {
                        myTexture = new Texture2D(1280, 960, TextureFormat.RGB24, false);
                    }
                }

                return myTexture;
            }
            set { myTexture = value; }
        }

        private void Awake()
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            myFolderpath = "file:///" + FileService.dataPath + LocalFilePath;
#else
            myFolderpath = "file://" + FileService.dataPath + LocalFilePath;
#endif

            myRawImage = GetComponent<RawImage>();

            if (!string.IsNullOrEmpty(initJpg))
            {
                LoadJpgSync(initJpg);
            }
        }

        private void OnDisable()
        {
            Release();
            DestroyTexture();
        }

        public void Clear()
        {
            Release();
        }

        private void DestroyTexture()
        {
            if (myRawImage.texture != null)
            {
                texture.Resize(0, 0, TextureFormat.RGBA32, false);
                DestroyImmediate(myRawImage.texture);
                myRawImage.texture = null;
            }
        }

        private void Release()
        {
            if (myWww != null)
            {
                myWww.Dispose();
                if (myLoadcoroutine != null)
                {
                    StopCoroutine(myLoadcoroutine);
                    myLoadcoroutine = null;
                }

                myWww = null;
            }
        }

        private string GetRealPath(string path, bool isSync = false)
        {
            if (setWithFullPath)
            {
                return path;
            }

            var prefix = FileService.dataPath + LocalFilePath;
            if (!isSync)
            {
                prefix = myFolderpath;
            }

            if (pathWithPostfix)
            {
                return prefix + path;
            }

            return prefix + path + ".jpg";
        }

        private bool IsFileExist(string path)
        {
            if (setWithFullPath)
            {
                return File.Exists(path);
            }

            string realPath;
            var prefix = FileService.dataPath + LocalFilePath;
            if (pathWithPostfix)
            {
                realPath = prefix + path;
            }
            else
            {
                realPath = prefix + path + ".jpg";
            }

            return File.Exists(realPath);
        }

        private string GetStreamingPath(string path)
        {
            var prefix = "data/" + LocalFilePath;
            if (pathWithPostfix)
            {
                return prefix + path;
            }

            return prefix + path + ".jpg";
        }

        public void LoadJpg(string path, bool sync = false)
        {
            if (myRawImage == null)
            {
                myRawImage = GetComponent<RawImage>();
            }

            if (myRawImage.texture == null)
            {
                myRawImage.enabled = false;
            }

            if (sync)
            {
                Release();
                LoadJpgSync(path);
                myRawImage.enabled = true;
            }
            else
            {
                Release();
                if (IsFileExist(path))
                {
                    var realPath = GetRealPath(path);
                    myLoadcoroutine = StartCoroutine(Load(realPath));
                }
                else
                {
                    var streamingPath = GetStreamingPath(path);
                    myLoadcoroutine =
                        StartCoroutine(FileLoaderHelper.LoadFromStreamAssetsAsyc(streamingPath, ShowImage));
                }
            }
        }

        private void LoadJpgSync(string path)
        {
            byte[] bytes;
            if (IsFileExist(path))
            {
                var realPath = GetRealPath(path, true);
                try
                {
                    bytes = File.ReadAllBytes(realPath);
                }
                catch (Exception e)
                {
                    Debug.LogError("load local jpg sync from path [" + realPath + "] occurred error\n" + e);
                    return;
                }
            }
            else
            {
                try
                {
                    bytes = FileLoaderHelper.LoadFromStreamAssets(GetStreamingPath(path));
                }
                catch (Exception e)
                {
                    Debug.LogError("load local jpg sync from streaming path [" + path + "] occurred error\n" + e);
                    return;
                }
            }

            texture.LoadImage(bytes);
            myRawImage.texture = texture;
        }

        private IEnumerator Load(string path)
        {
            using (myWww = new WWW(path))
            {
                while (myWww != null && !myWww.isDone && string.IsNullOrEmpty(myWww.error))
                {
                    yield return null;
                }

                if (myWww == null)
                {
                    Debug.LogError("load local jpg www is null");
                    yield break;
                }

                if (myWww.isDone)
                {
                    ShowImage(myWww.bytes);
                }
                else
                {
                    Debug.LogError(myWww.error);
                }

                myRawImage.enabled = true;
            }

            myWww = null;
        }

        private void ShowImage(byte[] bytes)
        {
            texture.LoadImage(bytes);
            myRawImage.texture = texture;
            myRawImage.enabled = true;
        }
    }
}