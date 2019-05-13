using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class GameController : MonoBehaviour
{
    public static GameController instance;
    [SerializeField]
    private GameObject pausePanel;
    [SerializeField]
    private Button restartButton;
    [SerializeField]
    private Button quitButton;
    [SerializeField]
    private GameObject keyImageUI;
    [SerializeField]
    private Image damageTakeVingetteUI;
    private Coroutine damageTakeVingetteRoutine = null;
    [SerializeField]
    private Text gameOverText;
    [SerializeField]
    private Text winText;
    [SerializeField]
    private Text coinCountText;
    [SerializeField]
    private Image healthBar;
    [SerializeField]
    private Image ammoBar;
    [SerializeField]

    private GameObject controlButtons;


    private uint coinCount = 0;

    public bool gameIsPaused = false;
    private bool gameOver = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //UI
        keyImageUI = GameObject.Find("/Canvas/KeyFlag");
        keyImageUI.SetActive(false);

        coinCountText = GameObject.Find("/Canvas/CountCoin").GetComponent<Text>();
        coinCountText.text = coinCount.ToString();

        healthBar = GameObject.Find("/Canvas/HealthBar").GetComponent<Image>();

        damageTakeVingetteUI = GameObject.Find("/Canvas/DamageTakeVinjette").GetComponent<Image>();
        damageTakeVingetteUI.gameObject.SetActive(false);

        ammoBar = GameObject.Find("/Canvas/AmmoBar").GetComponent<Image>();

        restartButton = GameObject.Find("/Canvas/PausePanel/Restart").GetComponent<Button>();
        restartButton.onClick.AddListener(Restart);

        quitButton = GameObject.Find("/Canvas/PausePanel/MainMenu").GetComponent<Button>();
        quitButton.onClick.AddListener(() => SceneManager.LoadScene(0));

        pausePanel = GameObject.Find("/Canvas/PausePanel");
        pausePanel.SetActive(false);

        gameOverText = GameObject.Find("/Canvas/GameOverText").GetComponent<Text>();
        gameOverText.gameObject.SetActive(false);

        winText = GameObject.Find("/Canvas/WinText").GetComponent<Text>();
        winText.gameObject.SetActive(false);

        controlButtons = GameObject.Find("/Canvas/ControlButtons");

        Time.timeScale = 1f;
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape) && !gameIsPaused && !gameOver)
            Pause();
        else if (Input.GetKeyDown(KeyCode.Escape) && gameIsPaused && !gameOver)
            UnPause();
    }

    public void CoinIncrease()
    {
        coinCount++;
        coinCountText.text = coinCount.ToString();
    }

    public void HealthBarDecreaseUI()
    {
        healthBar.fillAmount -= 0.333f;
        DamageTakeUIEffect();
    }

    public void HealthBarIncreaseUI()
    {
        healthBar.fillAmount += 0.333f;
    }

    public void HealthBarOnZeroUI()
    {
        healthBar.fillAmount = 0;
        DamageTakeUIEffect();
    }

    private void DamageTakeUIEffect()
    {
        if (damageTakeVingetteRoutine == null)
            damageTakeVingetteRoutine = StartCoroutine(DamageTakeVingetteFade());
        else
        {
            StopCoroutine(damageTakeVingetteRoutine);
            damageTakeVingetteRoutine = StartCoroutine(DamageTakeVingetteFade());
        }
    }

    private IEnumerator DamageTakeVingetteFade()
    {
        damageTakeVingetteUI.gameObject.SetActive(true);

        yield return StartCoroutine(AlphaToOne());
        yield return StartCoroutine(AlphaToZero());

        damageTakeVingetteUI.gameObject.SetActive(false);
        damageTakeVingetteRoutine = null;
    }

    private IEnumerator AlphaToOne()
    {
        while (damageTakeVingetteUI.color.a <= 0.3)
        {
            damageTakeVingetteUI.color = Color.Lerp(damageTakeVingetteUI.color, Color.white, Time.deltaTime * 25f);
            yield return null;
        }
        damageTakeVingetteUI.color = Color.white;
    }

    private IEnumerator AlphaToZero()
    {
        while (damageTakeVingetteUI.color.a >= 0.05)
        {
            damageTakeVingetteUI.color = Color.Lerp(damageTakeVingetteUI.color, Color.clear, Time.deltaTime * 5f);
            yield return null;
        }
        damageTakeVingetteUI.color = Color.clear;
    }

    public void AmmoBarDecreaseUI()
    {
        ammoBar.fillAmount -= 0.2f;
    }

    public void AmmoBarIncreaseUI()
    {
        ammoBar.fillAmount += 0.2f;
    }

    public void GetKey()
    {
        keyImageUI.SetActive(true);
    }

    public void GameWinCheck()
    {
        if (keyImageUI.activeInHierarchy)
        {
            Pause();
            winText.text = "YOU WIN!";
            winText.gameObject.SetActive(true);
        }
        //else "FIND A KEY"
    }

    public void GameOver()
    {
        Pause();
        gameOver = true;
        gameOverText.text = "Game Over";
        gameOverText.gameObject.SetActive(true);
    }

    public void Pause()
    {
        controlButtons.SetActive(false);
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public void UnPause()
    {
        controlButtons.SetActive(true);
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    public void Restart()
    {
        gameOver = false;    
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        System.GC.Collect();
        Time.timeScale = 1f;
        //controlButtons.SetActive(true);
    }
}
