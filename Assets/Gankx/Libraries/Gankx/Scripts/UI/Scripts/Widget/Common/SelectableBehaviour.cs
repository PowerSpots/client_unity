using UnityEngine;
using UnityEngine.UI;

namespace Gankx.UI
{
    public class SelectableBehaviour : MonoBehaviour
    {
        private Selectable mSelectable;

        void Awake()
        {
            mSelectable = GetComponent<Selectable>();

            Button button = mSelectable as Button;
            button.onClick.AddListener(OnClick);

            Toggle toggle = mSelectable as Toggle;
            toggle.onValueChanged.AddListener(OnActivate);

            Slider slider = mSelectable as Slider;

            Scrollbar scrollbar = mSelectable as Scrollbar;
            scrollbar.onValueChanged.AddListener(OnScroll);

            Dropdown dropdown = mSelectable as Dropdown;

            InputField inputField = mSelectable as InputField;
            inputField.onEndEdit.AddListener(OnSubmit);
        }

        protected virtual void OnClick()
        {
            
        }

        protected virtual void OnToggle(bool activated)
        {
            
        }

        protected virtual void OnSubmit(string text)
        {

        }

        protected virtual void OnScroll(float delta)
        {

        }

        protected virtual void OnActivate(bool activated)
        {
            
        }
    }
}