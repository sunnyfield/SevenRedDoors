using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ITriggerable
{
    void Activate();
}

public class TriggerableObject : MonoBehaviour, ITriggerable
{
    private Rigidbody2D platform;
    private float rotationSpeed = 15f;


    private void Start()
    {
        platform = transform.GetChild(1).GetComponent<Rigidbody2D>();
        platform.bodyType = RigidbodyType2D.Static;
    }


    public void Activate()
    {
        StartCoroutine(TurnPlatform());
    }

    private IEnumerator TurnPlatform()
    {
        platform.bodyType = RigidbodyType2D.Dynamic;

        while (platform.rotation > 0.1f)
        {
            platform.MoveRotation(Mathf.Lerp(platform.rotation, 0, Time.deltaTime * rotationSpeed));
            yield return null;
        }

        platform.rotation = 0;
        platform.bodyType = RigidbodyType2D.Static;
    }
}
