using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    [SerializeField]
    public GameObject[] poolablePrefabs;// = new GameObject[System.Enum.GetValues(typeof(Group)).Length];  

    public uint CoinCount { get; private set; } = 0;

    private bool gameIsPaused = false;
    private bool gameEnd = false;
    private bool key = false;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        Time.timeScale = 1f;

        for (int i = 0; i < Pool.objectsToPool.Length; i++)
        {
            var poolable = new PoolableObject(poolablePrefabs[i], (Group)i, 5);
            Pool.objectsToPool[i] = poolable;
        }
        Pool.CreatePools();
    }

    public void CoinIncrease()
    {
        CoinCount++;
        UIController.instance.CoinIncrease();
    }

    public void GetKey()
    {
        key = true;
        UIController.instance.GetKey();
    }

    public void GameWinCheck()
    {
        if (key)
        {
            Pause();
            gameEnd = true;
            UIController.instance.WinScreen();   
        }
        //else "FIND A KEY"
    }

    public void GameOver()
    {
        UIController.instance.HealthBarOnZero();
        UIController.instance.GameOverScreen();
        Pause();
        gameEnd = true;
    }

    public void PauseToggle()
    {
        if(!gameEnd)
        {
            if(!gameIsPaused) Pause();
            else UnPause();
        }
    }

    private void Pause()
    {
        UIController.instance.PauseScreen();
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    private void UnPause()
    {
        UIController.instance.HidePauseScreen();
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    public void Restart()
    {
        gameEnd = false;    
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        System.GC.Collect();
        Time.timeScale = 1f;
    }
}
