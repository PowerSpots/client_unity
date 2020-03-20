using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class LongPressTrigger : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public class LongReleaseEvent : UnityEvent<bool, float>
    {
    }

    [Header("按下后多久产生长按事件")]
    private float Threshold = 0.5f;

    public void SetThreshold(float value)
    {
        Threshold = value;
    }

    [Header("最大响应间隔")]
    private float MaxInterval = 0.1f;

    [Header("最小响应间隔")]
    private float MinInterval = 0.02f;

    [Header("加速阈值")]
    private float SpeedUpThreshold = 3.0f;

    [Header("加速总时间")]
    private float SpeedUpDuration = 3.0f;

    public UnityEvent onLongPress = new UnityEvent();
    public UnityEvent onLongPressStay = new UnityEvent();

    public UnityEvent<bool, float> onLongRelease = new LongReleaseEvent();
    public UnityEvent onPointerDown = new UnityEvent();

    private bool isPointerDown = false;
    private bool longPressTriggered = false;
    private float timePressStarted = 0;
    private float lastInvokeTime = -1;

    private GameObject pressedObject;

    private void Update()
    {
        if (isPointerDown && !longPressTriggered)
        {
            if (Time.time - timePressStarted > Threshold)
            {
                longPressTriggered = true;
                onLongPress.Invoke();
            }
        }

        if (isPointerDown && longPressTriggered)
        {
            if (Time.time - lastInvokeTime > GetCurrentInterval())
            {
                lastInvokeTime = Time.time;
                onLongPressStay.Invoke();
            }
        }
    }

    float GetCurrentInterval()
    {
        float elapsedTime = Time.time - timePressStarted;
        float speedUpTime = elapsedTime - SpeedUpThreshold;
        float amount = Mathf.Clamp01(speedUpTime/SpeedUpDuration);
        return Mathf.Lerp(MaxInterval, MinInterval, amount);
    }

    void OnDisable()
    {
        isPointerDown = false;
        longPressTriggered = false;
        timePressStarted = 0;
        lastInvokeTime = -1;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        timePressStarted = Time.time;
        isPointerDown = true;
        longPressTriggered = false;

        pressedObject = eventData.pointerCurrentRaycast.gameObject;
        onPointerDown.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;

        float elapsedTime = Time.time - timePressStarted;
        onLongRelease.Invoke(eventData.pointerCurrentRaycast.gameObject == pressedObject, elapsedTime);
        pressedObject = null;        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerDown = false;
    }
}