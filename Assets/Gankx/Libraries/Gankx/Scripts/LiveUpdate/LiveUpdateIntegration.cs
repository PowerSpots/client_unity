using System.Collections.Generic;
using UnityEngine;

namespace Gankx
{
    [DisallowMultipleComponent]
    public abstract class LiveUpdateIntegration : IntegrationBase<LiveUpdateIntegration>
    {
        public abstract string GetVersionLabel(string appVersion, string resVersion);

        public abstract string GetForwardScene();

        public abstract string GetVersionResourcesPath();

        public abstract string GetVersionDataPath();

        public abstract string GetVersionExtraPath();

        public abstract void GetBuildinFileList(List<string> buildinFileList);

        public abstract bool ShouldCleanupFile(string fileName);

        public abstract void IntegrateBugreport();

        public abstract void UpdateRes(LiveUpdate liveUpdate);

        public abstract void UpdateApk(LiveUpdate liveUpdate);

        public abstract bool IsUpdating();
    }
}