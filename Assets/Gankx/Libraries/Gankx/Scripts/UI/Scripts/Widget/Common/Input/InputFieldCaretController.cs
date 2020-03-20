
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputFieldCaretController : MonoBehaviour, ISelectHandler
{
    private InputField mInputField;
    void Awake()
    {
        mInputField = GetComponent<InputField>();
    }

    void ISelectHandler.OnSelect(BaseEventData eventData)
    {
        StartCoroutine(MoveTextEnd());
    }

    public void SetCaretPos(int pos)
    {
        if (mInputField == null)
        {
            return;
        }
        mInputField.ActivateInputField();

        //        EventSystem.current.SetSelectedGameObject(mInputField.gameObject);
        StartCoroutine(MoveTextToIndex(pos));
    }

    public void MoveCaretToEnd()
    {
        if (mInputField == null)
        {
            return;
        }
        mInputField.ActivateInputField();
        //        EventSystem.current.SetSelectedGameObject(mInputField.gameObject);
        StartCoroutine(MoveTextEnd());
    }

    public void MoveCaretToBegin()
    {
        if (mInputField == null)
        {
            return;
        }
        mInputField.ActivateInputField();

        //        EventSystem.current.SetSelectedGameObject(mInputField.gameObject);
        StartCoroutine(MoveTextBegin());
    }

    IEnumerator MoveTextEnd()
    {
        yield return null;
        mInputField.MoveTextEnd(false);
    }

    IEnumerator MoveTextBegin()
    {
        yield return null;
        mInputField.MoveTextStart(false);
    }

    IEnumerator MoveTextToIndex(int index)
    {
        yield return null;
        mInputField.caretPosition = index;
    }

}