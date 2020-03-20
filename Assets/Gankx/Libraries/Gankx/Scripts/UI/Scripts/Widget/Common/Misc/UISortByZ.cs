using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISortByZ : MonoBehaviour {

    public struct ChildTrans {
        public Transform current;
        public Transform checkChild;
    }

    public class ChildTransOrderByDescendingZ : Comparer<ChildTrans> {
        public bool useCameraPos = true;
        public override int Compare(ChildTrans x, ChildTrans y)
        {
            if (useCameraPos && Camera.main != null) {
                return Vector3.Dot(y.checkChild.position - Camera.main.transform.position,
                    Camera.main.transform.forward).CompareTo(
                    Vector3.Dot(x.checkChild.position - Camera.main.transform.position, Camera.main.transform.forward));
            }
            return y.checkChild.position.z.CompareTo(x.checkChild.position.z);
        }
    }

    public string m_Path = "";
    public ChildTransOrderByDescendingZ m_Compare = new ChildTransOrderByDescendingZ();
    
	void LateUpdate () {
        if (Time.frameCount % 5 == 0)
        {
            CheckAndSort();
        }
	}

    public List<ChildTrans> m_Childs = new List<ChildTrans>();

    void CheckAndSort() {
        if (m_Childs.Count != transform.childCount) {
            m_Childs.Clear();
            for (int i = 0; i < transform.childCount; i++) {
                ChildTrans child = new ChildTrans();
                child.current = transform.GetChild(i);
                child.checkChild = transform.GetChild(i).Find(m_Path);
                m_Childs.Add(child);
            }
        }

        PerformInsertionSort(m_Childs, m_Compare);

//        m_Childs = m_Childs.OrderByDescending(a => a.checkChild.position.z).ToList();

        for (int i = 0; i < m_Childs.Count; i++)
        {
            m_Childs[i].current.SetSiblingIndex(i);
        }
    }

    public static List<T> PerformInsertionSort<T>(List<T> inputarray, Comparer<T> comparer = null) {
        Comparer<T> equalityComparer = comparer;
        if (equalityComparer == null) equalityComparer = Comparer<T>.Default;
        for (int counter = 0; counter < inputarray.Count - 1; counter++)
        {
            int index = counter + 1;
            while (index > 0)
            {
                if (equalityComparer.Compare(inputarray[index - 1], inputarray[index]) > 0)
                {
                    T temp = inputarray[index - 1];
                    inputarray[index - 1] = inputarray[index];
                    inputarray[index] = temp;
                }
                index--;
            }
        }
        return inputarray;
    }
}


