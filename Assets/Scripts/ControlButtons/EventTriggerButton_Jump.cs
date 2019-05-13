using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EventTriggerButton_Jump : EventTrigger
{
    public override void OnPointerDown(PointerEventData eventData)
    {
            PlayerController.instance.Jump();
    }
}
