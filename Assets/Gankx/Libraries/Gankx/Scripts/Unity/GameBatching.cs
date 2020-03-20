using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class GameBatching : MonoBehaviour
    {
        private void Awake()
        {
            Batch1();
            //Batch2();
        }

        private void Batch1()
        {
            StaticBatchingUtility.Combine(gameObject);
        }

        private void Batch2()
        {
            List<GameObject> goList = new List<GameObject>();
            for (int i = 0; i < transform.childCount; ++i)
            {
                Transform trans = transform.GetChild(i);
                goList.Add(trans.gameObject);
            }

            GameObject[] goes = goList.ToArray();
            if (goes.Length > 0)
            {
                StaticBatchingUtility.Combine(goes, gameObject);
            }
        }
    }
}