using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public static MainMenuController instance;
    private Button playButton;
    private Button reloadButton;
    private Button quitButton;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        playButton = transform.Find("Button_Play").GetComponent<Button>();
        playButton.onClick.AddListener(() => SceneManager.LoadScene(1));

        reloadButton = transform.Find("Button_Reload").GetComponent<Button>();
        reloadButton.onClick.AddListener(() => SceneManager.LoadScene(2));

        quitButton = transform.Find("Button_Quit").GetComponent<Button>();
        quitButton.onClick.AddListener(() => Application.Quit());
    }
}
