namespace Gankx.UI
{
    public class RawImageUrlTextureLoadedDispatcher : EventDispatcher
    {
        protected override void OnInit()
        {
            var urlLoader = gameObject.GetOrAddComponent<RawImageUrlLoader>();
            urlLoader.onLoaded.AddListener(OnUrlTextureLoaded);
        }

        public void OnUrlTextureLoaded()
        {
            SendPanelMessage("OnUrlTextureLoaded");
        }
    }
}