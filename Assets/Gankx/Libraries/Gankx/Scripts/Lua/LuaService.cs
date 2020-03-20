using System;
using System.Collections;
using System.IO;
using Gankx.IO;
using UnityEngine;
using XLua;

namespace Gankx
{
    internal partial class LuaService : Singleton<LuaService>
    {
        private float myDeltaTime;
        private float myUnscaledDeltaTime;

        private Action myApplicationStartup;
        private Action myApplicationShutdown;
        private Action<double, double, bool> myApplicationUpdate;
        private LuaFunction myFireEvent;
        private LuaFunction myBootComplete;

        public LuaEnv env { get; private set; }

        public void Init()
        {
            if (env != null)
            {
                return;
            }

            env = new LuaEnv();
            myDeltaTime = 0f;
            myUnscaledDeltaTime = 0f;

            env.AddLoader(CustomLoader);
            StartLua();
        }


        private void StartLua()
        {
            StartCoroutine(StartupScripts());
        }

        protected override void OnRelease()
        {
            ShutdownScripts();

            if (env != null)
            {
                env.Dispose();
            }
        }

        private void Update()
        {
            if (null == env)
            {
                return;
            }

            myDeltaTime += Time.deltaTime;
            myUnscaledDeltaTime += Time.unscaledDeltaTime;

            var isLimit = myUnscaledDeltaTime > 0.05f;

            myApplicationUpdate?.Invoke(myDeltaTime, myUnscaledDeltaTime, isLimit);

            if (isLimit)
            {
                myDeltaTime = 0f;
                myUnscaledDeltaTime = 0f;
            }
        }

        private IEnumerator StartupScripts()
        {
            LuaExport.DoFile("scripts/booter.lua");

            myApplicationStartup = env.Global.Get<Action>("Application_Startup");
            myApplicationShutdown = env.Global.Get<Action>("Application_Shutdown");
            myApplicationUpdate = env.Global.Get<Action<double, double, bool>>("Application_Update");
            myFireEvent = env.Global.Get<LuaFunction>("fireEvent");
            myBootComplete = env.Global.Get<LuaFunction>("BootComplete");

            while (BootProgress() < 1)
            {
                yield return null;
            }

            yield return null;
            LoadExtraFiles();

            myApplicationStartup();
        }

        public float BootProgress()
        {
            if (myBootComplete == null)
            {
                return 0;
            }

            return myBootComplete.Func<float>();
        }

        private void LoadExtraFiles()
        {
            var checkPath = FileService.dataPath + "../hidden8848/";
            var checkDirectoryInfo = new DirectoryInfo(checkPath);
            if (checkDirectoryInfo.Exists)
            {
                var fileEntries = Directory.GetFiles(checkDirectoryInfo.FullName);
                for (var i = 0; i < fileEntries.Length; i++)
                {
                    var fileName = fileEntries[i];
                    if (!fileName.EndsWith(".lua"))
                    {
                        continue;
                    }

                    instance.env.DoString(File.ReadAllBytes(fileName));
                }
            }
        }

        private void ShutdownScripts()
        {
            if (myApplicationShutdown != null)
            {
                myApplicationShutdown();
            }

            myApplicationStartup = null;
            myApplicationUpdate = null;
            myApplicationShutdown = null;
            myFireEvent = null;
            myBootComplete = null;
        }

        public LuaTable NewTable()
        {
            if (null == env)
            {
                return null;
            }

            return env.NewTable();
        }

        public byte[] CustomLoader(ref string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            if (false == fileName.EndsWith(".lua"))
            {
                fileName = fileName + ".lua";
            }

            byte[] buff;
            var bufSize = LuaLoader.Load(fileName, out buff);
            if (bufSize <= 0)
            {
                Debug.LogError("Lua require Fail: FilePath:" + fileName);
                return null;
            }

            return buff;
        }
    }
}