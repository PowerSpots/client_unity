using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SpriteParticleEmitter;

public class ParticleFollowTrans : MonoBehaviour {
    [SerializeField] private Transform followTransform;

    private Vector3 previousParentPosition;

    private ParticleSystem pSystem;

    public Transform FollowTransform {
        get { return followTransform; }
        set {
            followTransform = value;
            previousParentPosition = followTransform.position;
        }
    }

    void Awake() {
        if (null != followTransform) {
            previousParentPosition = followTransform.position;
        }

        pSystem = GetComponent<ParticleSystem>();
    }

    void LateUpdate() {
        if (null != followTransform) {
            Vector3 parentMovement = followTransform.position - previousParentPosition;
            if (!Mathf.Approximately(parentMovement.x, 0) || !Mathf.Approximately(parentMovement.y, 0)) {
                ParticleSystem.Particle[] particles = UIParticleRenderer.particles;
                int numParticlesAlive = pSystem.GetParticles(particles);
                for (int i = 0; i < numParticlesAlive; i++) {
                    particles[i].position += parentMovement;
                }
                pSystem.SetParticles(particles, numParticlesAlive);
            }

            previousParentPosition = followTransform.position;
        }
    }
}