using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    private static UIController instance;
    public static UIController Instance { get { return instance; } }

    [SerializeField] GameObject PauseMenuObject;
    [SerializeField] GameObject VictoryMenuObject;
    [SerializeField] GameObject GameOverMenuObject;

    private bool canShowPauseMenu = true;

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && PauseMenuObject!=null && canShowPauseMenu)
        {
            if (PauseMenuObject.activeSelf)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        PauseMenuObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        PauseMenuObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void ShowVictoryMenu()
    {
        if (VictoryMenuObject != null)
        {
            VictoryMenuObject.SetActive(true);
            canShowPauseMenu = false;
        }
    }

    public void ShowGameOverMenu()
    {
        if (GameOverMenuObject != null)
        { 
            GameOverMenuObject.SetActive(true);
            canShowPauseMenu = false;
        }
}

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        LoadScene(0);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
