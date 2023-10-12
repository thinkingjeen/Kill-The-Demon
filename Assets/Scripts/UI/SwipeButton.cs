using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class SwipeButton : MonoBehaviour, IPointerDownHandler ,IPointerUpHandler,IDragHandler
{
    Vector2 pointerDownPosition;
    Vector2 pointerCurrentPosition;
    public float minSwipeDistance = 100;
    public bool buttonActive;

    public float coolTime;
    float coolTimeCheck;
    public Image cooldown;
    public Image directionArrow;

    eDirection direction;
    public UnityAction onButtonDownAction;
    public UnityAction<eDirection> onButtonUpAction;

    public void Init(float coolTime)
    {
        this.coolTime = coolTime;
    }
    private void Start()
    {
        directionArrow.gameObject.SetActive(false);
    }
    public void CoolTimeReset()
    {
        coolTimeCheck = coolTime;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (buttonActive && coolTimeCheck >= coolTime)
        {
            onButtonDownAction();
            pointerDownPosition = eventData.position;
            pointerCurrentPosition = eventData.position;
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (buttonActive && coolTimeCheck >= coolTime && pointerDownPosition.magnitude > 1)
        {
            pointerCurrentPosition = eventData.position;

            Vector2 dirVector = pointerCurrentPosition - pointerDownPosition;
            direction = eDirection.none;

            if (dirVector.magnitude >= minSwipeDistance)
            {
                float degree = Mathf.Atan2(dirVector.y, dirVector.x) * Mathf.Rad2Deg; // 스와이프 방향각을 -180° ~ 180° 까지 받아옴
                directionArrow.gameObject.SetActive(true);
                if (45 <= degree && degree < 135)
                { direction = eDirection.up; directionArrow.transform.eulerAngles = Vector3.zero; }
                else if (-135 <= degree && degree < -45)
                { direction = eDirection.down; directionArrow.transform.eulerAngles = Vector3.forward * 180; }
                else if (-180 <= degree && degree < -135 || 135 <= degree && degree <= 180)
                { direction = eDirection.left; directionArrow.transform.eulerAngles = Vector3.forward * 90; }
                else if (-45 <= degree && degree < 45)
                { direction = eDirection.right; directionArrow.transform.eulerAngles = Vector3.forward * 270; }
            }
            else
            {
                directionArrow.gameObject.SetActive(false);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (buttonActive && coolTimeCheck >= coolTime && pointerDownPosition.magnitude > 1)
        {
            coolTimeCheck = 0;
            onButtonUpAction(direction);

            directionArrow.gameObject.SetActive(false);
            direction = eDirection.none;
            pointerDownPosition = Vector2.zero;
            pointerCurrentPosition = Vector2.zero;
        }
    }

    void Update()
    {
        coolTimeCheck += Time.deltaTime;

        cooldown.fillAmount = coolTimeCheck >= coolTime ? 0 : 1 - coolTimeCheck / coolTime;
    }
}
