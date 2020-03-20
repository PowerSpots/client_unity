using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace SpriteParticleEmitter {
    /// <summary>
    ///     This class is a modification on the class shared publicly by Glenn Powell (glennpow) tha can be found here
    ///     http://forum.unity3d.com/threads/free-script-particle-systems-in-ui-screen-space-overlay.406862/
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(CanvasRenderer))]
    [RequireComponent(typeof(ParticleSystem))]
    [AddComponentMenu("UI/Effects/Extensions/UI Particle System")]
    public class UIParticleRenderer : MaskableGraphic {
        private Transform _transform;
        private ParticleSystem pSystem;
        public static ParticleSystem.Particle[] particles = new ParticleSystem.Particle[1000];
        private readonly UIVertex[] _quad = new UIVertex[4];
        private Vector4 imageUV = Vector4.zero;
        private ParticleSystem.TextureSheetAnimationModule textureSheetAnimation;
        private int textureSheetAnimationFrames;
        private Vector2 textureSheetAnimationFrameSize;
        //private ParticleSystemRenderer pRenderer;     // member not used anthonyzhao

        private Material currentMaterial;

        private Texture currentTexture;

        private ParticleSystem.MainModule mainModule;

        public ParticleSystem.MinMaxGradient m_PSColorGradient;

        public override Texture mainTexture {
            get { return currentTexture; }
        }

        [SerializeField] private Transform followTransform;

        public Transform FollowTransform {
            get { return followTransform; }
            set {
                followTransform = value;
                previousParentPosition = followTransform.position;
            }
        }

        [ContextMenu("Refresh Material")]
        public void RefreshMaterial()
        {
            if (currentMaterial && currentMaterial.HasProperty("_MainTex"))
            {
                currentTexture = currentMaterial.mainTexture;
                if (currentTexture == null)
                    currentTexture = Texture2D.whiteTexture;
            }

            this.UpdateMaterial();
        }

        private Vector3 previousParentPosition;

        protected bool Initialize() {
            // initialize members
            if (_transform == null) {
                _transform = transform;
            }
            if (pSystem == null) {
                pSystem = GetComponent<ParticleSystem>();

                if (pSystem == null) {
                    return false;
                }

                mainModule = pSystem.main;
                if (pSystem.main.maxParticles > 1000) {
                    mainModule.maxParticles = 1000;
                }

                if (pSystem.main.maxParticles == 0) {
                    return false;
                }

                var pRenderer = pSystem.GetComponent<ParticleSystemRenderer>();
                if (pRenderer != null)
                    pRenderer.enabled = false;

                if (material == null) {
                    Shader foundShader = Shader.Find("UI/Particles/Additive");
                    Material pMaterial = new Material(foundShader);
                    material = pMaterial;
                }

                currentMaterial = material;
                if (currentMaterial && currentMaterial.HasProperty("_MainTex")) {
                    currentTexture = currentMaterial.mainTexture;
                    if (currentTexture == null)
                        currentTexture = Texture2D.whiteTexture;
                }
                material = currentMaterial;
                // automatically set scaling
                mainModule.scalingMode = ParticleSystemScalingMode.Hierarchy;
            }
            if (Mathf.Approximately(pSystem.main.duration, pSystem.main.startLifetime.constant) && mainModule.maxParticles < 3) {
                mainModule.maxParticles += 2;
            }

            imageUV = new Vector4(0, 0, 1, 1);

            // prepare texture sheet animation
            textureSheetAnimation = pSystem.textureSheetAnimation;
            textureSheetAnimationFrames = 0;
            textureSheetAnimationFrameSize = Vector2.zero;
            if (textureSheetAnimation.enabled) {
                textureSheetAnimationFrames = textureSheetAnimation.numTilesX * textureSheetAnimation.numTilesY;
                textureSheetAnimationFrameSize = new Vector2(1f / textureSheetAnimation.numTilesX,
                    1f / textureSheetAnimation.numTilesY);
            }

            if (pSystem.colorOverLifetime.enabled) {
                m_PSColorGradient = pSystem.colorOverLifetime.color;
            }

            return true;
        }

        protected override void Awake() {
            base.Awake();
            if (!Initialize())
                enabled = false;

            raycastTarget = false;

            if (null != followTransform) {
                previousParentPosition = followTransform.position;
            }
        }

        private void Start() {
            if (!Application.isPlaying) return;
            UISoftClip softClip = transform.GetComponentInParent<UISoftClip>();
            if (softClip != null && material != null) {
                material = Instantiate(currentMaterial);
                material.EnableKeyword("UI_SOFT_CLIP");
                material.SetVector("_ClipSoft", softClip.SoftClip);
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh) {
#if UNITY_EDITOR
            if (!Application.isPlaying) {
                if (!Initialize()) {
                    return;
                }
            }
#endif
            // prepare vertices
            vh.Clear();

            if (!gameObject.activeInHierarchy) {
                return;
            }

            Vector2 temp = Vector2.zero;
            Vector2 corner1 = Vector2.zero;
            Vector2 corner2 = Vector2.zero;
            // iterate through current particles
            int count = pSystem.GetParticles(particles);

            for (int i = 0; i < count; ++i) {
                ParticleSystem.Particle particle = particles[i];

                // get particle properties
                Vector2 position = mainModule.simulationSpace == ParticleSystemSimulationSpace.Local
                    ? particle.position
                    : _transform.InverseTransformPoint(particle.position);

                float rotation = -particle.rotation * Mathf.Deg2Rad;
                float rotation90 = rotation + Mathf.PI / 2;
                Color32 color = particle.startColor;

                if (pSystem.colorOverLifetime.enabled) {
                    color *= m_PSColorGradient.Evaluate(1 - particle.remainingLifetime / particle.startLifetime);
                }

                Vector3 startSize3D = particle.GetCurrentSize3D(pSystem) * 0.5f;

                // apply scale
                if (mainModule.scalingMode == ParticleSystemScalingMode.Shape)
                    position /= canvas.scaleFactor;

                // apply texture sheet animation
                Vector4 particleUV = imageUV;
                if (textureSheetAnimation.enabled) {
                    float frameProgress =
                        textureSheetAnimation.frameOverTime.Evaluate(
                            1 - particle.remainingLifetime / particle.startLifetime);

                    if (textureSheetAnimation.frameOverTime.mode == ParticleSystemCurveMode.TwoConstants) {
                        Random.InitState((int) particle.randomSeed);
                        frameProgress = Random.Range(textureSheetAnimation.frameOverTime.constantMin,
                            textureSheetAnimation.frameOverTime.constantMax);
                    }

                    frameProgress = Mathf.Repeat(frameProgress * textureSheetAnimation.cycleCount, 1);
                    int frame = 0;

                    switch (textureSheetAnimation.animation) {
                        case ParticleSystemAnimationType.WholeSheet:
                            frame = Mathf.FloorToInt(frameProgress * textureSheetAnimationFrames);
                            break;

                        case ParticleSystemAnimationType.SingleRow:
                            frame = Mathf.FloorToInt(frameProgress * textureSheetAnimation.numTilesX);

                            int row = textureSheetAnimation.rowIndex;
                            frame += row * textureSheetAnimation.numTilesX;
                            break;
                    }

                    frame %= textureSheetAnimationFrames;

                    particleUV.x = frame % textureSheetAnimation.numTilesX * textureSheetAnimationFrameSize.x;
                    particleUV.y = Mathf.FloorToInt(frame / textureSheetAnimation.numTilesX) *
                                   textureSheetAnimationFrameSize.y;
                    particleUV.z = particleUV.x + textureSheetAnimationFrameSize.x;
                    particleUV.w = particleUV.y + textureSheetAnimationFrameSize.y;

                    particleUV.y = 1 - particleUV.y;
                    particleUV.w = 1 - particleUV.w;
                }

                temp.x = particleUV.x;
                temp.y = particleUV.y;

                _quad[0] = UIVertex.simpleVert;
                _quad[0].color = color;
                _quad[0].uv0 = temp;

                temp.x = particleUV.x;
                temp.y = particleUV.w;
                _quad[1] = UIVertex.simpleVert;
                _quad[1].color = color;
                _quad[1].uv0 = temp;

                temp.x = particleUV.z;
                temp.y = particleUV.w;
                _quad[2] = UIVertex.simpleVert;
                _quad[2].color = color;
                _quad[2].uv0 = temp;

                temp.x = particleUV.z;
                temp.y = particleUV.y;
                _quad[3] = UIVertex.simpleVert;
                _quad[3].color = color;
                _quad[3].uv0 = temp;

                if (Mathf.Approximately(rotation, 0)) {
                    // no rotation
                    corner1.x = position.x - startSize3D.x;
                    corner1.y = position.y - startSize3D.y;
                    corner2.x = position.x + startSize3D.x;
                    corner2.y = position.y + startSize3D.y;

                    temp.x = corner1.x;
                    temp.y = corner1.y;
                    _quad[0].position = temp;
                    temp.x = corner1.x;
                    temp.y = corner2.y;
                    _quad[1].position = temp;
                    temp.x = corner2.x;
                    temp.y = corner2.y;
                    _quad[2].position = temp;
                    temp.x = corner2.x;
                    temp.y = corner1.y;
                    _quad[3].position = temp;
                }
                else {
                    // apply rotation
                    Vector2 right = new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation)) * startSize3D.x;
                    Vector2 up = new Vector2(Mathf.Cos(rotation90), Mathf.Sin(rotation90)) * startSize3D.y;

                    _quad[0].position = position - right - up;
                    _quad[1].position = position - right + up;
                    _quad[2].position = position + right + up;
                    _quad[3].position = position + right - up;
                }

                vh.AddUIVertexQuad(_quad);
            }
        }

        public float m_UpdateFreq = 0.03f;
        private float m_CurrentTime;

        //private Dictionary<ParticleSystem.Particle, Vector3> m_PSStartPos = new Dictionary<ParticleSystem.Particle, Vector3>();   // unused member, anthonyzhao

        private void LateUpdate() {
            if (null != followTransform) {
                int numParticlesAlive = pSystem.GetParticles(particles);
                Vector3 parentMovement = followTransform.position - previousParentPosition;

                if (!Mathf.Approximately(parentMovement.x, 0) || !Mathf.Approximately(parentMovement.y, 0)) {
                    for (int i = 0; i < numParticlesAlive; i++) {
                        particles[i].position += parentMovement;
                    }
                    pSystem.SetParticles(particles, numParticlesAlive);
                }

                previousParentPosition = followTransform.position;
            }
            else {
                // ���͸���Ƶ��
                if (Time.timeSinceLevelLoad < m_UpdateFreq + m_CurrentTime && m_CurrentTime > 0) {
                    return;
                }
                m_CurrentTime = Time.timeSinceLevelLoad;
            }

            if (!Application.isPlaying) {
                SetVerticesDirty();
                ChangeTexureOffset();
            }
            else {
                SetVerticesDirty();

                ChangeTexureOffset();

                if (currentMaterial != null && currentTexture != currentMaterial.mainTexture ||
                    material != null && currentMaterial != null && material.shader != currentMaterial.shader) {
                    pSystem = null;
                    Initialize();
                }
            }

            if (material == currentMaterial)
                return;
            pSystem = null;
            Initialize();
        }

        public bool m_EnableChangeTextureOffset;
        public string m_ChangedTextureName = "_MainTex";
        public Vector2 m_ChangedTextureOffset = Vector2.zero;

        private int m_ChangedTextureNameIndex = -1;

        private void ChangeTexureOffset() {
            if (!m_EnableChangeTextureOffset)
                return;
            if (m_Material != null && !string.IsNullOrEmpty(m_ChangedTextureName)) {
                if (m_ChangedTextureNameIndex < 0) {
                    m_ChangedTextureNameIndex = Shader.PropertyToID(m_ChangedTextureName);
                }

                if (!m_Material.HasProperty(m_ChangedTextureNameIndex)) return;

                m_Material.SetTextureOffset(m_ChangedTextureName, m_ChangedTextureOffset);
            }
        }
    }
}