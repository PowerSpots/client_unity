using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable once UnusedMember.Global
// ReSharper disable once InconsistentNaming
// ReSharper disable once CheckNamespace
public class UITextLoopAnimation : MonoBehaviour
{


    public string[] LoopStrings;

    public float LoopGapTime = 0.5f;

    private int _index;

    private Text _text;

    public void OnEnable()
    {
        _text = GetComponent<Text>();
        _index = 0;

        if (_text != null && LoopStrings != null && LoopStrings.Length > 0)
        {
            StartCoroutine(AnimationCor());
        }
        
    }

    public IEnumerator AnimationCor()
    {
        
        while (true)
        {
            var i = _index++ % LoopStrings.Length;
            _text.text = LoopStrings[i];
            yield return new WaitForSeconds(LoopGapTime);
        }
        // ReSharper disable once IteratorNeverReturns
    }

}
