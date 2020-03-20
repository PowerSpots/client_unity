using System;
using System.Collections.Generic;
using System.Collections;
using Gankx;
using UnityEngine;
using UnityEngine.UI;

public class LoadedSprite
{
    public Sprite sprite;
    public int refCount;
    public bool IsNeedClear;

    public LoadedSprite(Sprite s)
    {
        sprite = s;
        refCount = 1;
        IsNeedClear = false;
    }

    public void AddReference()
    {
        ++refCount;
        IsNeedClear = false;
    }

    public void DecReference()
    {
        --refCount;
        if (refCount <= 0 )
        {
            IsNeedClear = true;
        }
    }
}

public class TextureManager : Singleton<TextureManager>
{
    private class AsyncLoadTask
    {
        public Image image { get; set; }
        public string path { get; set; }

        public AsyncLoadTask(Image img, string path)
        {
            this.image = img;
            this.path = path;
        }
    }

    private uint mUnrefMaxSize = 50 * 1024 * 1024;
    private long mUnRefSize = 0;

    // 已加载资源
    private Dictionary<string, LoadedSprite> mLoadedSprites = new Dictionary<string, LoadedSprite>();

    // 异步加载
    private List<AsyncLoadTask> mLoadTasks = new List<AsyncLoadTask>();

    private Dictionary<string, int> mLoadTaskCountDict = new Dictionary<string, int>();

    private readonly Predicate<AsyncLoadTask> mInvalidTaskPredicate = item => (null == item.image);

    private bool mShouldRemoveTask = false;

    private List<string> m_RemoveList = new List<string>();

    private bool m_ClearUnuseSprites = false;

    private void LateUpdate()
    {
        if (mShouldRemoveTask)
        {
            mShouldRemoveTask = false;
            mLoadTasks.RemoveAll(mInvalidTaskPredicate);
        }

        if (m_ClearUnuseSprites)
        {
            ClearTexturesWithFlag();
        }
    }


    public void SetUnrefMaxSize(uint size)
    {
        mUnrefMaxSize = size * 1024 * 1024;
    }

    public void SetTexture(Image img, string path)
    {
        if (null == img)
        {
            return;
        }

        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("TextureManager.SetTextureAsync Error: input path is empty");
            return;
        }

        Log(string.Format("--------TextureManager.SetTexture({0}, {1})", img.name, path));
        
        DereferenceTexture(img);

        if (string.IsNullOrEmpty(path))
        {
            img.sprite = null;
            return;
        }

        LoadedSprite loadedtex = null;
        mLoadedSprites.TryGetValue(path, out loadedtex);
        if (null != loadedtex)
        {
            loadedtex.AddReference();
            img.sprite = loadedtex.sprite;
        }
        else
        {
            Sprite sprite = ResourceService.Load<Sprite>(path);
            if (null != sprite)
            {
                mLoadedSprites[path] = new LoadedSprite(sprite);
            }
            img.sprite = sprite;
        }
    }

    public void SetTextureAsync(Image img, string path, bool keep = false)
    {
        if (null == img)
        {
            return;
        }

        Log(string.Format("--------------TextureManager.SetTextureAsync({0}, {1})", img.name, path));

        DereferenceTexture(img, keep);
        
        img.sprite = null;

        AsyncLoadTask asyncLoadTask = GetLoadTask(img);
        // img already in loading status, remove this task
        if (null != asyncLoadTask)
        {            
            if (path == asyncLoadTask.path)
            {
                return;               
            }

            RemoveLoadTask(asyncLoadTask);
        }

        // if path empty, img.sprite is already null, just return.
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
               
        LoadedSprite loadedtex = null;
        mLoadedSprites.TryGetValue(path, out loadedtex);
        if (null != loadedtex)
        {
            loadedtex.AddReference();
            img.sprite = loadedtex.sprite;                
        }
        else
        {
            AddLoadTask(img, path);
        }        
    }

    public void UnloadTexture(Image img , string path)
    {
        if (img == null)
        {
            return;
        }

        Log("_______________________UnloadTexture, Pahth:" + path);
        //ref
        DereferenceTexture(img);
        //task
        AsyncLoadTask loadtask = GetLoadTask(img);
        if (loadtask != null && loadtask.image == img)
        {
            RemoveLoadTask(loadtask);
        }
    }
    

    #region Texture Manager
    // private 内部保证texture不为null
    // 更新texture的引用
    private void DereferenceTexture(Image img, bool keep = false)
    {
        if (null == img.sprite)
        {
            return;
        }

        foreach (var item in mLoadedSprites)
        {
            if (item.Value.sprite == img.sprite)
            {
                if (--item.Value.refCount <= 0 && !keep)
                {
                    item.Value.IsNeedClear = true;
                    m_ClearUnuseSprites = true;
                }
                return;
            }
        }
    }

    public void ClearTextureAll()
    {
        foreach (var item in mLoadedSprites)
        {
            ResourceService.Unload();
            item.Value.refCount = 0;
            item.Value.IsNeedClear = false;
            item.Value.sprite = null;
        }
        mLoadedSprites.Clear();
    }
    
    public void ClearTextureUnrefered()
    {
        foreach (var pair in mLoadedSprites)
        {
            if (pair.Value.refCount <= 0)
            {
                mUnRefSize += (pair.Value.sprite.texture.width * pair.Value.sprite.texture.height * 4);
                pair.Value.sprite = null;
                m_RemoveList.Add(pair.Key);
            }
        }

        for (int i = 0; i < m_RemoveList.Count; ++i)
        {
            ResourceService.Unload();
            mLoadedSprites.Remove(m_RemoveList[i]);
        }
        m_RemoveList.Clear();

        if (mUnRefSize > mUnrefMaxSize)
        {
            Log("<color=red>@@@@--------TextureManager.RemoveTextureRef GC........</color>");
            mUnRefSize = 0;
            ResourceService.instance.CollectAll();
        }
    }

    public void ClearTexturesWithFlag()
    {
        Log("________________________________ClearTexturesWithFlag");

        m_ClearUnuseSprites = false;

        mUnRefSize = 0;

        foreach (var pair in mLoadedSprites)
        {
            if (pair.Value.IsNeedClear  == true)
            {
                LoadedSprite loadedSprite = pair.Value;
                if (loadedSprite != null && loadedSprite.sprite != null)
                {
                    if (loadedSprite.sprite.texture != null)
                    {
                        mUnRefSize += (loadedSprite.sprite.texture.width*loadedSprite.sprite.texture.height*4);
                        m_RemoveList.Add(pair.Key);
                    }
                    else
                    {
                        Debug.LogError("TetureManager ClearTexturesWithFlag  Sprite.texture is null,Path:" + pair.Key);
                    }
                }
                else
                {
                    Debug.LogError("TetureManager ClearTexturesWithFlag  Sprite is null : " + pair.Key);
                }
            }
        }

        if (mUnRefSize > mUnrefMaxSize)
        {
            for (int i = 0; i < m_RemoveList.Count; ++i)
            {
                string path = m_RemoveList[i];
                ResourceService.Unload();
                mLoadedSprites[path].sprite = null;
                mLoadedSprites.Remove(path);
            }
            ResourceService.instance.CollectAll();
        }
        //如果未达到，不卸载
        m_RemoveList.Clear();
        mUnRefSize = 0;
    }

    #endregion TextureManager


    #region  Task

    private void AddLoadTask(Image img, string path)
    {
        //1. add new loadtask
        mLoadTasks.Add(new AsyncLoadTask(img, path));

        img.enabled = false;
        //2. check loading texture
        if (mLoadTaskCountDict.ContainsKey(path))
        {
            mLoadTaskCountDict[path]++;
        }
        else
        {            
            mLoadTaskCountDict[path] = 1;
            LoadTextureWithCallback<Sprite>(path, obj => LoadTextureAsyncHandler(obj as Sprite, path));
        }
    }


    private void LoadTextureAsyncHandler(Sprite sprite, string path)
    {
        Log(string.Format("--------TextureManager.LoadTextureHandler, load finish:{0}", path));
        if (sprite == null || sprite.texture == null)
        {
            Debug.LogError("加载图片缺少Texture资源,Path:" + path);
        }

        //1 add loaded texture
        if (null != sprite)
        {
            LoadedSprite loadedtex;
            mLoadedSprites.TryGetValue(path, out loadedtex);
            if (null != loadedtex)
            {
                loadedtex.AddReference();
                sprite = loadedtex.sprite;
            }
            else
            {
                mLoadedSprites[path] = new LoadedSprite(sprite);
            }
        }

        //2 remove from mLoadTaskCountDict
        if (mLoadTaskCountDict.ContainsKey(path))
        {
            mLoadTaskCountDict.Remove(path);
        }
        else
        {
            LogError("-------- m_LoadTextrue doesn't contain:" + path);
        }

        //3 remove loadtasks with the same path
        int refCount2 = 0;
        for (int i = 0; i < mLoadTasks.Count; ++i)
        {
            if (mLoadTasks[i].path == path)
            {
                ++refCount2;
                Image img = mLoadTasks[i].image;
                if (null != img)
                {
                    //++refCount2;
                    img.sprite = sprite;

                    //Color color = img.color;
                    //color.a = 1.0f;
                    img.enabled = true;
                }

                mLoadTasks[i].image = null;
                mShouldRemoveTask = true;
            }
        }

        //4 check refcount
        //        if (refCount1 != refCount2)
        //        {
        //            LogError(string.Format("--------TextureManager.LateUpdate, refCount check error:{0} != {1}", refCount1, refCount2));
        //        }
    }

    private void RemoveLoadTask(AsyncLoadTask asyncLoadTask)
    {
        if (null != asyncLoadTask)
        {
            //1 dereference mLoadTaskCountDict
            if (mLoadTaskCountDict.ContainsKey(asyncLoadTask.path))
            {
                if (--mLoadTaskCountDict[asyncLoadTask.path] <= 0)
                {
                    if (CancelLoadFromResources(asyncLoadTask.path))
                    {
                        mLoadTaskCountDict.Remove(asyncLoadTask.path);
                        Log("--------TextureManager.RemoveLoadTask loadingTexture cancel success:" + asyncLoadTask.path);
                    }
                    else
                    {
                        Log("--------TextureManager.RemoveLoadTask loadingTexture cancel fail:" + asyncLoadTask.path);
                    }
                }
            }
            else
            {
                LogError("-------- m_LoadTextrue doesn't contain:" + asyncLoadTask.path);
            }

            if (null != asyncLoadTask.image)
            {
                //Color color = loadTask.image.color;
                //color.a = 1f;
                asyncLoadTask.image.enabled = true;
            }
            //2 dereference loadTask
            asyncLoadTask.image = null;
            mShouldRemoveTask = true;
        }
    }

    public bool CancelLoadFromResources(string path)
    {
        bool removeSuccess = RemoveTasksByPath(path);
        Action<UnityEngine.Object> action;
        if (mActionDict.TryGetValue(path, out action))
        {
            mActionDict.Remove(path);
        }

        return removeSuccess;
    }

    private bool RemoveTasksByPath(string path)
    {
        int prevCount = mLoadTasks.Count;
        Predicate<AsyncLoadTask> predicate = item => (path == item.path);
        mLoadTasks.RemoveAll(predicate);

        return prevCount != mLoadTasks.Count;
    }

    private AsyncLoadTask GetLoadTask(Image img)
    {
        AsyncLoadTask asyncLoadTask = null;
        for (int i = 0; i < mLoadTasks.Count; ++i)
        {
            Image t = mLoadTasks[i].image;
            if (null != t && t.GetInstanceID() == img.GetInstanceID())
            {
                asyncLoadTask = mLoadTasks[i];
                break;
            }
        }

        return asyncLoadTask;
    }

    public string GetLoadingPath(Image img)
    {
        string path = null;
        if (img != null)
        {
            AsyncLoadTask loadtask = GetLoadTask(img);
            if (loadtask != null)
            {
                path = loadtask.path;
            }
        }

        return path;
    }

    public void RemoveAllLoadTasks()
    {
        while (mLoadTasks.Count > 0)
        {
            RemoveLoadTask(mLoadTasks[0]);
        }
    }

    #endregion 


    #region LoadTextureAsync

    private IEnumerator mLoadCoroutine;
    private Dictionary<string, Action<UnityEngine.Object>> mActionDict = new Dictionary<string, Action<UnityEngine.Object>>();

    /// <summary>
    /// 添加到加载列表，启动加载
    /// </summary>
    public void LoadTextureWithCallback<T>(string resourcesPath, Action<UnityEngine.Object> callback) where T : UnityEngine.Object
    {
        if (!mActionDict.ContainsKey(resourcesPath) || null == mActionDict[resourcesPath])
        {
            mActionDict[resourcesPath] = callback;
        }
        else
        {
            mActionDict[resourcesPath] += callback;  //TODO
        }

        if (null == mLoadCoroutine)
        {
            mLoadCoroutine = LoadTextureRoutine<T>();
            StartCoroutine(mLoadCoroutine);
        }
    }

    private IEnumerator LoadTextureRoutine<T>()  where T : UnityEngine.Object
    {
        while (mLoadTasks.Count > 0)
        {
            string path = mLoadTasks[0].path;

            if (mActionDict.ContainsKey(path) && null != mActionDict[path])
            {
                LoadAssetRequest request = ResourceService.LoadAsync<T>(path);
                yield return request;

                T res = request.asset as T;

                if (mActionDict.ContainsKey(path) && null != mActionDict[path])
                {
                    mActionDict[path](res);
                }
            }

            RemoveTasksByPath(path);

            mActionDict[path] = null;
            mActionDict.Remove(path);
        }
        mLoadCoroutine = null;
    }

    #endregion

    #region log
    private void Log(string s)
    {
//        Debug.Log(s);
    }

    private void LogError(string s)
    {
        Debug.LogError(s);
    }

    #endregion 
}