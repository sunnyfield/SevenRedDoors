using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EventTriggerButton_Right : EventTrigger
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        PlayerController.instance.MoveRight();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        PlayerController.instance.Stop();
    }
}
