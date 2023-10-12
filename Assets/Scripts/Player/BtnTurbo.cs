using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BtnTurbo : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool isBtnDown = false;
    private float btnTime;
    public System.Action onBtnAction;

    private void Update()
    {
        if (isBtnDown)
        {
            this.btnTime += Time.deltaTime;
            if(this.btnTime >= 0.25f)
            {
                this.onBtnAction();
                this.btnTime = 0;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isBtnDown = true;
        this.onBtnAction();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isBtnDown = false;
    }
}
