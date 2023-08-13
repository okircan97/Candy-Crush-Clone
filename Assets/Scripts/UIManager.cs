using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{   
    /////////////////////////////////////
    ////////////// FIELDS ///////////////
    /////////////////////////////////////

    Board board;
    RoundManager roundManager;
    [SerializeField] TMP_Text timeText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text winScore;
    [SerializeField] TMP_Text winText;
    [SerializeField] GameObject stars1, stars2, stars3;
    [SerializeField] GameObject roundOverScreen;


    /////////////////////////////////////
    /////////// START & UPDATE //////////
    /////////////////////////////////////

    // Start is called before the first frame update
    void Start(){
        roundManager = FindObjectOfType<RoundManager>();
        board = FindObjectOfType<Board>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    /////////////////////////////////////
    ////////////// METHODS //////////////
    /////////////////////////////////////

    // Setter for time text;
    public void SetTimeText(string value){
        timeText.text = value;
    } 

    // Setter for score text.
    public void SetScoreText(string value){
        scoreText.text = value;
    }

    // This method is to stop the game and activate 
    // the game over screen.
    public void GameOver(int score){
        board.currentState = Board.BoardState.wait;
        // Get the final score
        winScore.text = score.ToString();
        // Choose the stars and the text to be showed and.
        // save the number of stars the player get.
        if(score >= roundManager.scoreTarget3){
            winText.text = "You've nailed it buddy!";
            stars3.gameObject.SetActive(true);
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, 3);
        } 
        else if(score >= roundManager.scoreTarget2 &&
                score <= roundManager.scoreTarget3){
            winText.text = "That was decent dude!";
            stars2.gameObject.SetActive(true);
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, 2);
        }
        else{
            winText.text = "You need lotta practice...";
            stars1.gameObject.SetActive(true);
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, 1);
        }
        roundOverScreen.SetActive(true);
    }
}
