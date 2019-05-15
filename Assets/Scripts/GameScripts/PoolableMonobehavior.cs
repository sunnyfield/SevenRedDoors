using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableMonobehavior : MonoBehaviour
{
    [SerializeField]
    private int initialPoolSize;

    public int InitialPoolSize { get { return initialPoolSize; } }

    public delegate void OnDestroyCall();
    public OnDestroyCall OnDestroyMethod;

    // Start is called before the first frame update
    void Start()
    {
        initialPoolSize = 5;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
