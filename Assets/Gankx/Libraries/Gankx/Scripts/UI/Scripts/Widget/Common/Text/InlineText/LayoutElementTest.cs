using UnityEngine;
using UnityEngine.UI;

//
//class LayoutElementTest : LayoutElement
//{
//
//    public virtual void CalculateLayoutInputHorizontal()
//    {
//        Debug.Log("_CalculateLayoutInputHorizontal");
//    }
//
//
//    public virtual void CalculateLayoutInputVertical()
//    {
//        Debug.Log("CalculateLayoutInputVertical");
//    }
//}

class LayoutElementTest :MonoBehaviour
{
    public Text uiText ;

    void Awake()
    {
        uiText = GetComponent<Text>();
    }
    void Update()
    {
        int width = CalculateLengthOfMessage(uiText.text);
        Debug.Log("——————————————————————————————width:" + width);
        Debug.Log("——————————————————————————————preferredWidth:" + uiText.preferredWidth);
    }


    int CalculateLengthOfMessage(string message)
    {
        int totalLength = 0;

        Font myFont = uiText.font;  //chatText is my Text component
        CharacterInfo characterInfo = new CharacterInfo();

        char[] arr = message.ToCharArray();

        foreach (char c in arr)
        {
            myFont.GetCharacterInfo(c, out characterInfo, uiText.fontSize);

            totalLength += characterInfo.advance;
        }

        return totalLength;
    }
}