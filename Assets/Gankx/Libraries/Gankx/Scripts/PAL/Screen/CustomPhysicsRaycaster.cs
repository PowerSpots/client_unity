﻿using System.Collections.Generic;
using UnityEngine.UI;

namespace UnityEngine.EventSystems {
    /// <summary>
    /// Simple event system using physics raycasts.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CustomPhysicsRaycaster : BaseRaycaster {
        /// <summary>
        /// Const to use for clarity when no event mask is set
        /// </summary>
        protected const int kNoEventMaskSet = -1;

        protected Camera m_EventCamera;

        /// <summary>
        /// Layer mask used to filter events. Always combined with the camera's culling mask if a camera is used.
        /// </summary>
        [SerializeField]
        protected LayerMask m_EventMask = kNoEventMaskSet;

        protected CustomPhysicsRaycaster() { }

        public override Camera eventCamera {
            get {
                if (m_EventCamera == null)
                    m_EventCamera = GetComponent<Camera>();
                return m_EventCamera ?? Camera.main;
            }
        }


        /// <summary>
        /// Depth used to determine the order of event processing.
        /// </summary>
        public virtual int depth {
            get { return (eventCamera != null) ? (int)eventCamera.depth : 0xFFFFFF; }
        }

        /// <summary>
        /// Event mask used to determine which objects will receive events.
        /// </summary>
        public int finalEventMask {
            get { return (eventCamera != null) ? eventCamera.cullingMask & m_EventMask : kNoEventMaskSet; }
        }

        /// <summary>
        /// Layer mask used to filter events. Always combined with the camera's culling mask if a camera is used.
        /// </summary>
        public LayerMask eventMask {
            get { return m_EventMask; }
            set { m_EventMask = value; }
        }

        protected void ComputeRayAndDistance(PointerEventData eventData, out Ray ray, out float distanceToClipPlane) {
            ray = eventCamera.ScreenPointToRay(eventData.position);
            // compensate far plane distance - see MouseEvents.cs
            float projectionDirection = ray.direction.z;
            distanceToClipPlane = Mathf.Approximately(0.0f, projectionDirection)
                ? Mathf.Infinity
                : Mathf.Abs((eventCamera.farClipPlane - eventCamera.nearClipPlane) / projectionDirection);
        }

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList) {
            if (eventCamera == null)
                return;
            eventData.position = ScreenRTMgr.OriToScaleSize(eventData.position);
            Ray ray;
            float distanceToClipPlane;
            ComputeRayAndDistance(eventData, out ray, out distanceToClipPlane);

            if (ReflectionMethodsCache.Singleton.raycast3DAll == null)
                return;

            RaycastHit[] hits = ReflectionMethodsCache.Singleton.raycast3DAll(ray, distanceToClipPlane, finalEventMask);

            if (hits.Length > 1)
                System.Array.Sort(hits, (r1, r2) => r1.distance.CompareTo(r2.distance));

            if (hits.Length != 0) {
                for (int b = 0, bmax = hits.Length; b < bmax; ++b) {
                    var result = new RaycastResult {
                        gameObject = hits[b].collider.gameObject,
                        module = this,
                        distance = hits[b].distance,
                        worldPosition = hits[b].point,
                        worldNormal = hits[b].normal,
                        screenPosition = eventData.position,
                        index = resultAppendList.Count,
                        sortingLayer = 0,
                        sortingOrder = 0
                    };
                    resultAppendList.Add(result);
                }
            }

            eventData.position = ScreenRTMgr.ScaleToOriSize(eventData.position);
        }
    }
}

