using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{   
    public void LoadMainMenu(){
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadLevelSelect(){
        SceneManager.LoadScene("LevelSelect");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
