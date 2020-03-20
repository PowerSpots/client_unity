using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class GameRenderQueue : MonoBehaviour
    {
        [SerializeField]
        private int renderQueue = 3100; //Transparent + 100

        private List<Material> materials = null;

        private void Awake()
        {
            InitMaterials();
            Apply();
        }

        private void InitMaterials()
        {
            materials = new List<Material>();
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            if (renderers != null)
            {
                for (int i = 0; i < renderers.Length; ++i)
                {
                    Material[] mats = renderers[i].sharedMaterials;
                    if (mats == null)
                    {
                        continue;
                    }

                    for (int j = 0; j < mats.Length; ++j)
                    {
                        if (mats[j] == null)
                        {
                            continue;
                        }
                        materials.Add(mats[j]);
                    }
                }
            }

            if (materials.Count > 0 && renderQueue == 3100)
            {
                renderQueue = materials[0].renderQueue;
            }
        }

        private void Apply()
        {
            return;
            for (int i = 0; i < materials.Count; ++i)
            {
                materials[i].renderQueue = renderQueue;
            }
        }

        private void OnValidate()
        {
            if (materials == null || materials.Count == 0)
            {
                InitMaterials();
            }
            Apply();
        }
    }
}