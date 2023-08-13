using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectButton : MonoBehaviour
{   
    /////////////////////////////////////
    ////////////// FIELDS ///////////////
    /////////////////////////////////////

    public string levelToLoad;
    public GameObject star1, star2, star3;


    /////////////////////////////////////
    /////////// START & UPDATE //////////
    /////////////////////////////////////

    void Start()
    {
        HandleStars();
    }


    /////////////////////////////////////
    ////////////// METHODS //////////////
    /////////////////////////////////////

    // This method is to load the given scene.
    public void LoadGivenLevel(){
        SceneManager.LoadScene(levelToLoad);
    }

    // This method is to show the stars according
    // to the score the player get on the relevant 
    // level.
    private void HandleStars(){
        int starNo = PlayerPrefs.GetInt(levelToLoad, 0);
        switch(starNo){
            case 0:
                star1.SetActive(false);
                star2.SetActive(false);
                star3.SetActive(false);
                break;
            case 1:
                star1.SetActive(true);
                star2.SetActive(false);
                star3.SetActive(false);
                break;
            case 2:
                star1.SetActive(true);
                star2.SetActive(true);
                star3.SetActive(false);
                break;
            case 3:
                star1.SetActive(true);
                star2.SetActive(true);
                star3.SetActive(true);
                break;
        }
    }
}
