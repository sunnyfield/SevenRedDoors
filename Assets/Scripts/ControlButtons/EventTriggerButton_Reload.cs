using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EventTriggerButton_Reload : EventTrigger
{
    public override void OnPointerDown(PointerEventData eventData)
    {
            PlayerController.instance.Reload();
    }
}
