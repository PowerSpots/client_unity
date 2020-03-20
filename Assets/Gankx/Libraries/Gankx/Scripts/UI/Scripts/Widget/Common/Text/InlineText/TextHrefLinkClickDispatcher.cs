using EmojText;

namespace Gankx.UI
{
    public class TextHrefLinkClickDispatcher : EventDispatcher
    {
        private InlineText inlineText;

        protected override void OnInit()
        {
            inlineText = GetComponent<InlineText>();
            if (null == inlineText)
            {
                return;
            }

            inlineText.onHrefClick.AddListener(OnHrefLinkClick);
        }

        public void OnHrefLinkClick(string content)
        {
            SendPanelMessage("OnHrefClick", content);
        }
    }
}
