using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ITriggerable
{
    void Activate();
}

public class TriggerableObject : MonoBehaviour, ITriggerable
{
    private Transform platform;

    private void Start()
    {
        platform = transform.GetChild(1);
    }


    public void Activate()
    {
        StartCoroutine(TurnPlatform());
    }

    private IEnumerator TurnPlatform()
    {
        while (platform.eulerAngles.z != 0)
        {
            platform.eulerAngles = Vector3.Lerp(platform.eulerAngles, Vector3.zero, Time.deltaTime * 2f);
            if (platform.eulerAngles.z < 0.01f)
                platform.eulerAngles = Vector3.zero;
            yield return null;
        }
    }
}
