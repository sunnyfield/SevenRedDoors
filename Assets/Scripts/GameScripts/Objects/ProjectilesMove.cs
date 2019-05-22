using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilesMove : MonoBehaviour
{
    private GameObject muzzleFlash = null;
    private GameObject hitFlash = null;
    private Vector3 startPosition;


    private float speed;
    public float maxExistDistance = 100f;

    void OnEnable()
    {
        muzzleFlash = null;
        hitFlash = null;
        startPosition = transform.localPosition;
        speed = 60f;
    }

    void Update()
    {
        if(muzzleFlash == null)
            muzzleFlash = Pool.Pull(Group.VFX_Muzzle, transform.localPosition, transform.rotation, 0.3f);

        transform.localPosition += transform.right * speed * Time.deltaTime;

        if (Mathf.Abs(transform.localPosition.x - startPosition.x) >= maxExistDistance)
        {
            transform.localPosition = transform.right * maxExistDistance + startPosition;
            speed = 0;

            if (hitFlash == null)
                hitFlash = Pool.Pull(Group.VFX_Hit, transform.localPosition, transform.localRotation, 0.3f);

            Pool.Push(Group.Projectile, gameObject, 0.1f);       
        }
    }
}
