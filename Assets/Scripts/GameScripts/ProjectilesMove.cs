using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilesMove : MonoBehaviour
{
    public GameObject muzzlePrefab;
    private GameObject muzzleFlash;
    public GameObject hitPrefab;
    private GameObject hitFlash = null;
    private Vector3 startPosition;


    private float speed;
    public float maxExistDistance = 100f;

    void OnEnable()
    {
        //muzzleFlash = Instantiate(muzzlePrefab, transform.position, transform.rotation);
        muzzleFlash = ObjectPooler.instance.GetPooledObject("VFX_MuzzleFlash");
        if(muzzleFlash != null)
        {
            muzzleFlash.transform.position = transform.position;
            muzzleFlash.transform.rotation = transform.rotation;
            muzzleFlash.SetActive(true);
        }
        startPosition = transform.position;
        speed = 60f;
    }

    void FixedUpdate()
    {
        transform.position += transform.right * speed * Time.deltaTime;
    }

    void Update()
    {
        transform.position += transform.right * speed * Time.deltaTime;
        
        if (Mathf.Abs(transform.position.x - startPosition.x) >= maxExistDistance)
        {

            transform.position = transform.right * maxExistDistance + startPosition;
            speed = 0;
            //hitFlash = Instantiate(hitPrefab, transform.position, transform.rotation);
            if(hitFlash == null)
                hitFlash = ObjectPooler.instance.GetPooledObject("VFX_Hit");
            if(hitFlash != null)
            {
                hitFlash.transform.position = transform.position;
                hitFlash.transform.rotation = transform.rotation;
                hitFlash.SetActive(true);
            }
            Invoke("DeactivateHit", 0.3f);
            Invoke("DeactivateMuzzle", 0.3f);
            Invoke("DeactivateProjectile", 0.1f);           
        }
    }

    private void DeactivateProjectile()
    {
        gameObject.SetActive(false);
    }

    private void DeactivateHit()
    {
        hitFlash.SetActive(false);
    }

    private void DeactivateMuzzle()
    {
        muzzleFlash.SetActive(false);
    }
}
