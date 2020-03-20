using System.Collections.Generic;
using UnityEngine;

namespace Gankx
{
    public class ManagedResource<T>
    {
        public T resource;
        public int refCount;
        public string resourcePath;

        protected ManagedResource()
        {
            resource = default(T);
            refCount = 0;
            resourcePath = null;
        }

        public bool noRef
        {
            get { return refCount <= 0; }
        }

        public void IncRef()
        {
            ++refCount;
        }

        public void DecRef()
        {
            --refCount;
            if (refCount == 0)
            {
                Free();
            }
        }

        protected virtual void Free()
        {
        }
    }

    public class ResourceManager<T, R> where T : ManagedResource<R>, new() where R : Object
    {
        private class LoadTask
        {
            public readonly ResourceLoadedDelegate onLoaded;

            public LoadTask(string path, ResourceLoadedDelegate loadedDelegate)
            {
                this.path = path;
                onLoaded = loadedDelegate;
            }

            public string path { get; private set; }
        }

        protected delegate R LoadDelegate(string path);

        protected delegate void UnloadDelegate(string path);

        public delegate void ResourceLoadedDelegate(T resource, bool isAsync);

        private bool myShouldCheckUnusedResources;

        private bool myForceUnloadUnusedResources;

        public readonly Dictionary<string, T> loadedResourceDict =
            new Dictionary<string, T>();

        private LoadDelegate myLoader;

        private UnloadDelegate myUnloader;

        private readonly List<LoadTask> myLoadTasks = new List<LoadTask>();

        private readonly List<string> myUnloadList = new List<string>();

        private LoadAssetRequest myLoadingRequest;

        private string myLoadingAssetPath = string.Empty;

        protected virtual int mUnloadTotalWeight
        {
            get { return 100; }
        }

        protected LoadDelegate loader
        {
            get
            {
                if (null == myLoader)
                {
                    return DefaultLoader;
                }

                return myLoader;
            }

            set { myLoader = value; }
        }

        protected UnloadDelegate unloader
        {
            get
            {
                if (null == myUnloader)
                {
                    return DefaultUnloder;
                }

                return myUnloader;
            }
            set { myUnloader = value; }
        }

        public void Update()
        {
            UpdateLoadingTasks();
            TryUnloadUnusedResources();
        }

        public R DefaultLoader(string path)
        {
            return ResourceService.Load<R>(path);
        }

        public void DefaultUnloder(string path)
        {
            ResourceService.Unload();
        }

        public void Load(string path, ResourceLoadedDelegate loadedDelegate)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("ResourceManager.Load Error: input path is empty");
                return;
            }

            T loadedResource;
            if (!loadedResourceDict.TryGetValue(path, out loadedResource))
            {
                var resource = loader(path);
                if (null != resource)
                {
                    loadedResource = new T();
                    loadedResource.resource = resource;
                    loadedResource.resourcePath = path;
                    loadedResourceDict[path] = loadedResource;
                }

                InternalLoadCallback(resource, path);
            }

            if (null != loadedDelegate)
            {
                loadedDelegate(loadedResource, false);
            }
        }

        public void RemoveLoadTaskByDelegate(ResourceLoadedDelegate loadedDelegate)
        {
            for (var i = 0; i < myLoadTasks.Count; i++)
            {
                if (myLoadTasks[i].onLoaded == loadedDelegate)
                {
                    RemoveLoadTask(myLoadTasks[i]);
                    return;
                }
            }
        }

        public void LoadAsync(string path, ResourceLoadedDelegate loadedDelegate)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            RemoveLoadTaskByDelegate(loadedDelegate);

            T loadedResource;
            loadedResourceDict.TryGetValue(path, out loadedResource);
            if (null != loadedResource)
            {
                if (null != loadedDelegate)
                {
                    loadedDelegate(loadedResource, true);
                }
            }
            else
            {
                AddLoadTask(path, loadedDelegate);
            }
        }

        public void Unload(string path)
        {
            DoUnload(path);
        }

        private void DoUnload(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            unloader(path);

            if (loadedResourceDict.ContainsKey(path))
            {
                loadedResourceDict.Remove(path);
            }
        }

        protected virtual int GetSingleWeight(R resource)
        {
            return 0;
        }

        protected void TryUnloadUnusedResources()
        {
            if (!myShouldCheckUnusedResources && !myForceUnloadUnusedResources)
            {
                return;
            }

            myShouldCheckUnusedResources = false;
            myUnloadList.Clear();
            var canUnloadWeight = 0;
            foreach (var pair in loadedResourceDict)
            {
                if (pair.Value.noRef)
                {
                    var loadedResource = pair.Value;
                    if (loadedResource != null && loadedResource.resource != null)
                    {
                        canUnloadWeight += GetSingleWeight(loadedResource.resource);
                        myUnloadList.Add(pair.Key);
                    }
                    else
                    {
                        Debug.LogError("ResourceManager TryUnloadUnusedResources Resource is null : " + pair.Key);
                    }
                }
            }

            if (myForceUnloadUnusedResources || canUnloadWeight >= mUnloadTotalWeight)
            {
                for (var i = 0; i < myUnloadList.Count; ++i)
                {
                    var path = myUnloadList[i];
                    DoUnload(path);
                }

                myForceUnloadUnusedResources = false;
            }
        }

        private List<LoadTask> GetTasksByPath(string path)
        {
            var result = new List<LoadTask>();
            for (var i = 0; i < myLoadTasks.Count; i++)
            {
                if (myLoadTasks[i].path == path)
                {
                    result.Add(myLoadTasks[i]);
                }
            }

            return result;
        }

        private void AddLoadTask(string path, ResourceLoadedDelegate loadedDelegate)
        {
            myLoadTasks.Add(new LoadTask(path, loadedDelegate));
        }

        private void RemoveLoadTask(LoadTask loadTask)
        {
            if (null == loadTask)
            {
                return;
            }

            if (null != loadTask.onLoaded)
            {
                T managedResource;
                loadedResourceDict.TryGetValue(loadTask.path, out managedResource);
                loadTask.onLoaded(managedResource, true);
            }

            myLoadTasks.Remove(loadTask);
        }

        private void RemoveTasksByPath(string path)
        {
            var tasks = GetTasksByPath(path);
            for (var i = tasks.Count - 1; i >= 0; i--)
            {
                RemoveLoadTask(tasks[i]);
            }
        }

        private void InternalLoadCallback(R resource, string path)
        {
            if (null != resource)
            {
                T loadedResource;
                if (!loadedResourceDict.TryGetValue(path, out loadedResource))
                {
                    loadedResource = new T();
                    loadedResource.resource = resource;
                    loadedResource.resourcePath = path;
                    loadedResourceDict[path] = loadedResource;
                }
            }

            RemoveTasksByPath(path);
        }

        private void LogError(string s)
        {
            Debug.LogError(s);
        }


        public void UnloadUnusedAssets()
        {
            myForceUnloadUnusedResources = true;
            TryUnloadUnusedResources();
        }

        public void MarkDirty()
        {
            myShouldCheckUnusedResources = true;
        }

        public void UpdateLoadingTasks()
        {
            while (myLoadTasks.Count > 0)
            {
                var loadTask = myLoadTasks[0];

                var path = loadTask.path;

                //TODO Temp 
                if (!string.IsNullOrEmpty(myLoadingAssetPath) && myLoadingAssetPath != path)
                {
                    myLoadingRequest = null;
                    myLoadingAssetPath = string.Empty;
                }

                if (null == myLoadingRequest)
                {
                    myLoadingRequest = ResourceService.LoadAsync<R>(path);
                    myLoadingAssetPath = path;
                }

                if (null != myLoadingRequest && !myLoadingRequest.isDone)
                {
                    return;
                }

                if (null != myLoadingRequest && myLoadingRequest.isDone)
                {
                    var res = myLoadingRequest.asset as R;

                    InternalLoadCallback(res, path);

                    myLoadingRequest = null;
                }
            }
        }
    }
}