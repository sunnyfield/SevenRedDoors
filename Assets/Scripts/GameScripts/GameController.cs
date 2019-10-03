using GameScripts.Pool;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameScripts
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance;

        [SerializeField]
        public GameObject[] poolablePrefabs;// = new GameObject[System.Enum.GetValues(typeof(Group)).Length];  

        public uint CoinCount { get; private set; }

        private bool _gameIsPaused;
        private bool _gameEnd;
        private bool _key;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            Time.timeScale = 1f;

            for (int i = 0; i < Pool.Pool.ObjectsToPool.Length; i++)
            {
                if (poolablePrefabs != null)
                {
                    var poolable = new PoolableObject(poolablePrefabs[i], (Group)i, 5);
                    Pool.Pool.ObjectsToPool[i] = poolable;
                }
            }
            Pool.Pool.CreatePools();
        }

        public void CoinIncrease()
        {
            CoinCount++;
            UIController.instance.CoinIncrease();
        }

        public void GetKey()
        {
            _key = true;
            UIController.instance.GetKey();
        }

        public void GameWinCheck()
        {
            if (_key)
            {
                Pause();
                _gameEnd = true;
                UIController.instance.WinScreen();   
            }
            //else "FIND A KEY"
        }

        public void GameOver()
        {
            UIController.instance.HealthBarOnZero();
            UIController.instance.GameOverScreen();
            Pause();
            _gameEnd = true;
        }

        public void PauseToggle()
        {
            if(!_gameEnd)
            {
                if(!_gameIsPaused) Pause();
                else UnPause();
            }
        }

        private void Pause()
        {
            UIController.instance.PauseScreen();
            Time.timeScale = 0f;
            _gameIsPaused = true;
        }

        private void UnPause()
        {
            UIController.instance.HidePauseScreen();
            Time.timeScale = 1f;
            _gameIsPaused = false;
        }

        public void Restart()
        {
            _gameEnd = false;    
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            System.GC.Collect();
            Time.timeScale = 1f;
        }
    }
}
