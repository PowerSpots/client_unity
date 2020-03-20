using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Gankx.UI
{
    public enum SlotControlOverrideEnum
    {
        Text,
        Image,
        Color,
        Active,
        DynamicImage,
        Max
    }

    public class SlotControlOverrideConfig
    {
        public class SlotControlOverrideStruct
        {
            public SlotControlOverrideEnum overrideEnum;
            public Type overrideType;
            public string overridePath;
        }

        private static readonly Dictionary<SlotControlOverrideEnum, SlotControlOverrideStruct> OverrideMap =
            new Dictionary<SlotControlOverrideEnum, SlotControlOverrideStruct>();

        private static void Init()
        {
            {
                var overrideStruct = new SlotControlOverrideStruct();
                overrideStruct.overrideEnum = SlotControlOverrideEnum.Text;
                overrideStruct.overrideType = typeof(Text);
                overrideStruct.overridePath = "Text";
                OverrideMap.Add(overrideStruct.overrideEnum, overrideStruct);
            }
            {
                var overrideStruct = new SlotControlOverrideStruct();
                overrideStruct.overrideEnum = SlotControlOverrideEnum.Image;
                overrideStruct.overrideType = typeof(Image);
                overrideStruct.overridePath = "Image";
                OverrideMap.Add(overrideStruct.overrideEnum, overrideStruct);
            }

            {
                var overrideStruct = new SlotControlOverrideStruct();
                overrideStruct.overrideEnum = SlotControlOverrideEnum.Color;
                overrideStruct.overrideType = typeof(Graphic);
                overrideStruct.overridePath = "Image";
                OverrideMap.Add(overrideStruct.overrideEnum, overrideStruct);
            }

            {
                var overrideStruct = new SlotControlOverrideStruct();
                overrideStruct.overrideEnum = SlotControlOverrideEnum.Active;
                overrideStruct.overrideType = typeof(Transform);
                overrideStruct.overridePath = "Image";
                OverrideMap.Add(overrideStruct.overrideEnum, overrideStruct);
            }
            {
                var overrideStruct = new SlotControlOverrideStruct();
                overrideStruct.overrideEnum = SlotControlOverrideEnum.DynamicImage;
                overrideStruct.overrideType = typeof(DynamicImage);
                overrideStruct.overridePath = "Image";
                OverrideMap.Add(overrideStruct.overrideEnum, overrideStruct);
            }
        }

        public static SlotControlOverrideStruct GetOverrideStruct(SlotControlOverrideEnum overrideEnum)
        {
            if (OverrideMap.Count == 0)
            {
                Init();
            }

            return OverrideMap[overrideEnum];
        }
    }

    [ExecuteInEditMode]
    [RequireComponent(typeof(SlotControl))]
    public class SlotControlOverride : MonoBehaviour
    {
        [FormerlySerializedAs("m_OverriderEnum")]
        public SlotControlOverrideEnum overrideEnum;

        [FormerlySerializedAs("m_OverridePath")]
        public string overridePath;

        [FormerlySerializedAs("m_OverrideInfo")]
        public string overrideInfo;

        [FormerlySerializedAs("m_SetNativeSize")]
        public bool setNativeSize = false;

        [NonSerialized]
        private SlotControl mySlotControl;

#if UNITY_EDITOR
        [NonSerialized]
        private SlotControlOverrideEnum myPreOverrideEnum = SlotControlOverrideEnum.Max;
#endif

        private void OnEnable()
        {
            if (mySlotControl == null)
            {
                mySlotControl = GetComponent<SlotControl>();
            }

            mySlotControl.overrideAction += Override;

            Override();
        }

        private void OnDisable()
        {
            if (mySlotControl == null)
            {
                mySlotControl = GetComponent<SlotControl>();
            }

            mySlotControl.overrideAction -= Override;
        }

        private void Awake()
        {
            Override();
        }

        private Component GetTypeOnPath()
        {
            var overrideStruct = SlotControlOverrideConfig.GetOverrideStruct(overrideEnum);
            overridePath = string.IsNullOrEmpty(overridePath) ? overrideStruct.overridePath : overridePath;
            if (mySlotControl.cachedControlTransform == null ||
                mySlotControl.cachedControlTransform.Find(overridePath) == null)
            {
                return null;
            }

            var child = mySlotControl.cachedControlTransform.Find(overridePath);
            return child.GetComponent(overrideStruct.overrideType);
        }

        public void Override()
        {
            if (mySlotControl == null)
            {
                mySlotControl = GetComponent<SlotControl>();
            }

            if (mySlotControl == null)
            {
                return;
            }

            if (mySlotControl.cachedControlTransform == null)
            {
                return;
            }

            var component = GetTypeOnPath();
            if (component == null)
            {
                if (overrideEnum == SlotControlOverrideEnum.Image)
                {
                    overrideInfo = string.Empty;
                }

                return;
            }

            switch (overrideEnum)
            {
                case SlotControlOverrideEnum.Text:
                    var text = component as Text;
                    if (text != null)
                    {
                        if (string.IsNullOrEmpty(overrideInfo))
                        {
                            overrideInfo = text.text;
                        }

                        text.text = overrideInfo;
                    }

                    break;
                case SlotControlOverrideEnum.Image:
                    var image = component as AtlasImage;
                    if (image != null)
                    {
                        if (string.IsNullOrEmpty(overrideInfo))
                        {
                            overrideInfo = GetSpritePath(image);
                        }

                        SetPath(image, overrideInfo);
                        if (setNativeSize)
                        {
                            image.SetNativeSize();
                        }
                    }

                    break;
                case SlotControlOverrideEnum.Color:
                    var graphic = component as Graphic;
                    if (graphic != null)
                    {
                        if (string.IsNullOrEmpty(overrideInfo))
                        {
                            overrideInfo = "FFFFFF";
                        }

                        graphic.color = int.Parse(overrideInfo, NumberStyles.AllowHexSpecifier).HexToColor32();
                    }

                    break;
                case SlotControlOverrideEnum.Active:
                    var trans = component as Transform;
                    if (trans != null)
                    {
                        if (string.IsNullOrEmpty(overrideInfo))
                        {
                            overrideInfo = "True";
                        }

                        trans.gameObject.SetActive(bool.Parse(overrideInfo));
                    }

                    break;
                case SlotControlOverrideEnum.DynamicImage:
                    var dynamicImage = component as DynamicImage;
                    if (dynamicImage != null)
                    {
                        if (string.IsNullOrEmpty(overrideInfo))
                        {
                            overrideInfo = "";
                        }

                        dynamicImage.SetPath(overrideInfo);
                        if (setNativeSize)
                        {
                            dynamicImage.SetNativeSize();
                        }
                    }

                    break;
            }
        }

        private string GetSpritePath(Image image)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                var path = AssetDatabase.GetAssetPath(image.sprite);
                if (!string.IsNullOrEmpty(path))
                {
                    if (path.StartsWith(UIConfig.AssetPath))
                    {
                        path = path.Substring(UIConfig.AssetPath.Length);
                    }

                    path = path.Substring(0, path.LastIndexOf("."));
                    return path;
                }
            }
#endif
            return string.Empty;
        }

        private void SetPath(AtlasImage image, string mPath)
        {
            if (image == null || string.IsNullOrEmpty(mPath))
            {
                return;
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                var assetPath2 = UIConfig.AssetPath + mPath;
                var res = AssetDatabase.LoadAssetAtPath(assetPath2 + ".png", typeof(Sprite));
                if (res != null)
                {
                    image.sprite = res as Sprite;
                    return;
                }
            }
#endif

            var di = image.GetOrAddComponent<DynamicImage>();
            di.SetAtlasPath(mPath);
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                return;
            }

#if UNITY_EDITOR
            if (myPreOverrideEnum != overrideEnum)
            {
                if (myPreOverrideEnum != SlotControlOverrideEnum.Max)
                {
                    var overrideStruct = SlotControlOverrideConfig.GetOverrideStruct(overrideEnum);
                    overridePath = overrideStruct.overridePath;
                }

                myPreOverrideEnum = overrideEnum;
            }
#endif

            Override();
        }
    }
}