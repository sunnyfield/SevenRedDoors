using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    private Image healthBar;
    private Coroutine damageTakeVingetteRoutine = null;
    private Image damageTakeVingette;
    private Image ammoBar;
    private GameObject keyImage;
    private Text coinCountText;
    private GameObject pausePanel;
    private Text gameOverText;
    private Text winText;
    private Button restartButton;
    private Button quitButton;
    private GameObject controlButtons;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        healthBar = GameObject.Find("/Canvas/HealthBar").GetComponent<Image>();
        damageTakeVingette = GameObject.Find("/Canvas/DamageTakeVinjette").GetComponent<Image>();
        damageTakeVingette.gameObject.SetActive(false);
        ammoBar = GameObject.Find("/Canvas/AmmoBar").GetComponent<Image>();
        keyImage = GameObject.Find("/Canvas/KeyFlag");
        keyImage.SetActive(false);
        coinCountText = GameObject.Find("/Canvas/CountCoin").GetComponent<Text>();
        coinCountText.text = GameController.instance.CoinCount.ToString();
        restartButton = GameObject.Find("/Canvas/PausePanel/Restart").GetComponent<Button>();
        restartButton.onClick.AddListener(() => GameController.instance.Restart());
        quitButton = GameObject.Find("/Canvas/PausePanel/MainMenu").GetComponent<Button>();
        quitButton.onClick.AddListener(() => SceneManager.LoadScene(0));
        pausePanel = GameObject.Find("/Canvas/PausePanel");
        pausePanel.SetActive(false);
        gameOverText = GameObject.Find("/Canvas/GameOverText").GetComponent<Text>();
        gameOverText.gameObject.SetActive(false);
        winText = GameObject.Find("/Canvas/WinText").GetComponent<Text>();
        winText.gameObject.SetActive(false);
        controlButtons = GameObject.Find("/Canvas/ControlButtons");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HealthBarDecrease()
    {
        healthBar.fillAmount -= 0.333f;
        DamageTakeEffect();
    }

    public void HealthBarIncrease()
    {
        healthBar.fillAmount += 0.333f;
    }

    public void HealthBarOnZero()
    {
        healthBar.fillAmount = 0;
        DamageTakeEffect();
    }

    private void DamageTakeEffect()
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
        damageTakeVingette.gameObject.SetActive(true);

        yield return StartCoroutine(AlphaToOne());
        yield return StartCoroutine(AlphaToZero());

        damageTakeVingette.gameObject.SetActive(false);
        damageTakeVingetteRoutine = null;
    }

    private IEnumerator AlphaToOne()
    {
        while (damageTakeVingette.color.a <= 0.3)
        {
            damageTakeVingette.color = Color.Lerp(damageTakeVingette.color, Color.white, Time.deltaTime * 25f);
            yield return null;
        }
        damageTakeVingette.color = Color.white;
    }

    private IEnumerator AlphaToZero()
    {
        while (damageTakeVingette.color.a >= 0.05)
        {
            damageTakeVingette.color = Color.Lerp(damageTakeVingette.color, Color.clear, Time.deltaTime * 5f);
            yield return null;
        }
        damageTakeVingette.color = Color.clear;
    }

    public void AmmoBarDecrease() { ammoBar.fillAmount -= 0.2f; }

    public void AmmoBarIncrease() { ammoBar.fillAmount += 0.2f; }

    public void GetKey() { keyImage.SetActive(true); }

    public void CoinIncrease() { coinCountText.text = GameController.instance.CoinCount.ToString(); }

    public void WinScreen()
    {
        winText.text = "YOU WIN!";
        winText.gameObject.SetActive(true);
    }

    public void GameOverScreen()
    {
        gameOverText.text = "Game Over";
        gameOverText.gameObject.SetActive(true);
    }

    public void PauseScreen()
    {
        controlButtons.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void HidePauseScreen()
    {
        controlButtons.SetActive(true);
        pausePanel.SetActive(false);
    }
}
