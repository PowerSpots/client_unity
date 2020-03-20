using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShakeDetector : MonoBehaviour {

    private float old_y;
    private float new_y;
    private float d_y;
    
    public float triggerInterval = 100f;    

    public UnityEvent onShake = new UnityEvent();

    private float lastTriggerTime = 0f;
    private bool canInvoke = false;

    private void OnEnable()
    {
        // 保证每次重新打开都能摇
        lastTriggerTime = -triggerInterval;
    }       

    private void Update()
    {
        new_y = Input.acceleration.y;
        d_y = new_y - old_y;
        old_y = new_y;
        if (d_y > 2 && Time.realtimeSinceStartup - lastTriggerTime > triggerInterval)
        {
            onShake.Invoke();
            lastTriggerTime = Time.realtimeSinceStartup;
        }
    }
}
