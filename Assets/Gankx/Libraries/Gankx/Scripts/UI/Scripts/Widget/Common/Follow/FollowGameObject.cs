using System.Collections;
using UnityEngine;

public class FollowGameObject : MonoBehaviour {
    public Transform target;
    public Vector3 offset = Vector3.zero;

    public string m_DelayFindTarget = "";
    private float m_LastCheckTime;

    void Start() {
        FindTarget();
    }

    [ContextMenu("预览")]
    private void Update() {
        if (target != null) {
            transform.position = target.position + offset;
        }
        else if (!string.IsNullOrEmpty(m_DelayFindTarget) && Time.timeSinceLevelLoad - m_LastCheckTime > 0.2f) {
            FindTarget();
        }
    }

    private void FindTarget() {
        if (string.IsNullOrEmpty(m_DelayFindTarget)) return;
        GameObject obj = GameObject.Find(m_DelayFindTarget);
        if (obj != null) {
            target = obj.transform;
        }
        m_LastCheckTime = Time.timeSinceLevelLoad;
    }
}