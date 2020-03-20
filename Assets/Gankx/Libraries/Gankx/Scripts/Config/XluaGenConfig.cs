/*
 * Tencent is pleased to support the open source community by making xLua available.
 * Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
 * Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
 * http://opensource.org/licenses/MIT
 * Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using XLua;

//配置的详细介绍请看Doc下《XLua的配置.doc》
public static class XluaGenConfig
{

    // xlua UseReflection won't get an error
    public static bool CanUseReflection;

    //lua中要使用到C#库的配置，比如C#标准库，或者Unity API，第三方库等。
#if UNITY_EDITOR
    public static bool IsTypeGenerated(Type type)
    {
        foreach(Type t in LuaCallCSharp)
        {
            if(t == type)
            {
                return true;
            }
        }

        return false;
    }
#endif

    [LuaCallCSharp]
    public static List<Type> LuaCallCSharp = new List<Type>() {

                typeof(Gankx.LuaExport),

                typeof(System.Object),
                typeof(System.Type),
                typeof(System.Reflection.Missing),
                typeof(LuaStringServiceExport),

                typeof(DebugExport),
                // typeof(List<ApolloQQGroup>),

                typeof(ClipboardExport),
                typeof(StopwatchExport),
                typeof(LuaProfilerExport),
                typeof(TimeControlExport),

//                typeof(LuaDebug),
                typeof(UIButtonExport),
                typeof(UIDropdownExport),
                typeof(UIImageExport),
                typeof(UIInputFieldExport),
                typeof(UITextFieldValidateExport),
                (typeof(UILongPressTriggerExport)),
                (typeof(UIPanelServiceExport)),
                (typeof(UIPlayAnimationExport)),
                (typeof(UIPlayTweenExport)),
                (typeof(UIScrollViewExport)),
                (typeof(UIScrollbarExport)),
                (typeof(UISelectableExport)),
                (typeof(UISliderExport)),
                (typeof(UITextExport)),
                (typeof(UIToggleExport)),
                (typeof(UIWindowExport)),
                (typeof(EasyListViewExport)),
                (typeof(UICanvasGroupExport)),


                (typeof(UITweenAlphaExport)),
                (typeof(UITweenAmountExport)),
                (typeof(UITweenColorExport)),
                (typeof(UITweenerExport<Gankx.UI.TweenSlider>)),
                typeof(UITweenerExport<Gankx.UI.TweenAmount>),
                typeof(UITweenerExport<Gankx.UI.TweenPosition>),
                typeof(UITweenerExport<Gankx.UI.TweenAlpha>),
                typeof(UITweenerExport<Gankx.UI.TweenRotation>),
                typeof(UITweenMaterialFloatExport),
                (typeof(UITweenFOVExport)),
                (typeof(UITweenHeightExport)),
                (typeof(UITweenOrthoSizeExport)),
                (typeof(UITweenPositionExport)),
                (typeof(Gankx.UI.TweenPosition)),
                (typeof(UITweenRotationExport)),
                (typeof(Gankx.UI.TweenRotation)),
                (typeof(UITweenScaleExport)),
                (typeof(UITweenSliderExport)),
                (typeof(UITweenScrollRectExport)),
                (typeof(UITweenVolumeExport)),
                (typeof(UITweenWidthExport)),
                (typeof(RawImageLocalLoaderExport)),

                (typeof(PlatformExport)),
                (typeof(ResourceServiceExport)),

                (typeof(NetworkExport)),
                (typeof(SceneManagerExport)),

                (typeof(CustomAsyncOperationExport)),
                (typeof(ResourcesExport)),
                (typeof(UnityEngine.Object)),
                typeof(Behaviour),
                (typeof(GameObject)),
                (typeof(Transform)),
                (typeof(Vector3)),
                (typeof(MonoBehaviour)),
                (typeof(Component)),
                (typeof(BoxCollider)),
                (typeof(BoxCollider2D)),
                (typeof(UnityEngine.AI.NavMeshAgent)),
                (typeof(UIProgressExport)),
                (typeof(GroupColorEffectExport)),
                (typeof(SetMaterialAttrExport)),
                (typeof(ContentSizeFitterExport)),
                (typeof(LayoutElementExport)),
                (typeof(InlineTextManagerExport)),
                (typeof(Util)),
                typeof(EasyLayoutContentSizeFitterExport),

                (typeof(Canvas)),
                (typeof(UnityEngineObjectExtention)),
                (typeof(LoopScrollRectExport)),
                (typeof(UIRawimageExport)),
                (typeof(UICirclePolygonExport)),
                (typeof(GamePortalExport)),
                (typeof(TypewriterEffectExport)),

                // Dotween
                typeof(DG.Tweening.AutoPlay),
                typeof(DG.Tweening.AxisConstraint),
                typeof(DG.Tweening.Ease),
                typeof(DG.Tweening.LogBehaviour),
                typeof(DG.Tweening.LoopType),
                typeof(DG.Tweening.PathMode),
                typeof(DG.Tweening.PathType),
                typeof(DG.Tweening.RotateMode),
                typeof(DG.Tweening.ScrambleMode),
                typeof(DG.Tweening.TweenType),
                typeof(DG.Tweening.UpdateType),

                typeof(DG.Tweening.DOTween),
                typeof(DG.Tweening.DOVirtual),
                typeof(DG.Tweening.EaseFactory),
                typeof(DG.Tweening.Tweener),
                typeof(DG.Tweening.Tween),
                typeof(DG.Tweening.Sequence),
                typeof(DG.Tweening.TweenParams),
                typeof(DG.Tweening.Core.ABSSequentiable),

                typeof(DG.Tweening.Core.TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions>),
                typeof(DG.Tweening.Core.TweenerCore<Quaternion,Vector3,DG.Tweening.Plugins.Options.QuaternionOptions>),
                typeof(DG.Tweening.Core.TweenerCore<float,float,DG.Tweening.Plugins.Options.FloatOptions>),

                typeof(DG.Tweening.TweenCallback),
                typeof(DG.Tweening.TweenExtensions),
                typeof(DG.Tweening.TweenSettingsExtensions),
                typeof(DG.Tweening.ShortcutExtensions),
                typeof(DG.Tweening.ShortcutExtensions),
                typeof(DG.Tweening.Plugins.Options.QuaternionOptions),

                typeof(BlurOptimized),
                typeof(BlurOptimized.BlurType),
                typeof(ScreenRTMgr),
                typeof(RectTransform),
                typeof(PerformanceCounterExport),
                typeof(UITextureExport),
                typeof(UIAnchorExport),
                typeof(UnityEngine.Quaternion),
                typeof(GridLayoutGroupExport),
                typeof(UICenterOnChildExport),
                typeof(UIAnimatorExport),
                typeof(UITweenBaseExport),
                typeof(CameraExport),
                typeof(DebugExport.LogLevel),
                typeof(UITextExport.OverflowMethod),
                typeof(RectTransform.Axis),
                typeof(RectTransform.Edge),
                typeof(UIFollowGameObject),
                typeof(PostEffectsBase),
                typeof(MonoBehaviour),

                typeof(BoxCollider),
                typeof(UnityEngine.EventSystems.PointerEventData),
                typeof(UnityEngine.EventSystems.PointerEventData.FramePressState),
                typeof(UnityEngine.EventSystems.PointerEventData.InputButton),

                typeof(ApplicationExport),
                typeof(DeviceExport),


                typeof(DateTimeExport),
                typeof(MessengerExport),
                typeof(AtlasManagerExport),

                typeof(BundleSpriteManagerExport),
                typeof(RawImageAssetTextureManagerExport),
                typeof(LayoutGrouptContentSizeFitterExport),

                typeof(FileLoaderHelper),
//                typeof(GameMovieCaptureServiceExport),
            };


    //C#静态调用Lua的配置（包括事件的原型），仅可以配delegate，interface
    [CSharpCallLua]
    public static List<Type> CSharpCallLua = new List<Type>()
            {
                typeof(Action),
                typeof(Action<string>),
                typeof(Action<double>),
                typeof(Action<double,double>),
                typeof(Func<int, string>),
                typeof(Action<double,double,bool>),
                typeof(UnityEngine.Events.UnityAction),
                typeof(Func<ulong, bool, LuaTable>),
            };

    //黑名单
    [BlackList] public static List<List<string>> BlackList = new List<List<string>>()
    {
        new List<string>() {"UnityEngine.MonoBehaviour", "runInEditMode"},
    };
}
