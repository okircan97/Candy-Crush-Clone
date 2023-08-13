using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{   
    /////////////////////////////////////
    ////////////// FIELDS ///////////////
    /////////////////////////////////////

    UIManager uiManager;
    Board board;
    [SerializeField] float roundTime = 60f;
    bool isEndingRound;
    int score;
    float displayScore = 0;
    public int scoreTarget1, scoreTarget2, scoreTarget3;


    /////////////////////////////////////
    /////////// START & UPDATE //////////
    /////////////////////////////////////

    void Start(){
        uiManager = FindObjectOfType<UIManager>();
        board = FindObjectOfType<Board>();
    }

    void Update(){
        CountBack();
    }


    /////////////////////////////////////
    ////////////// METHODS //////////////
    /////////////////////////////////////

    // This method is to count back from the round time.
    private void CountBack(){
        if(roundTime > 0){
            roundTime -= Time.deltaTime;

            // Prevent the display going below 0.
            if(roundTime <= 0){
                roundTime = 0;
                isEndingRound = true;
            }
        }

        if(isEndingRound && board.currentState == Board.BoardState.move){
            uiManager.GameOver(score);
            isEndingRound = false;
        }
        uiManager.SetTimeText(roundTime.ToString("0.0") + "s");

        displayScore = Mathf.Lerp(displayScore, score, 5f*Time.deltaTime);
        uiManager.SetScoreText(displayScore.ToString("0"));
    }

    // This method is to update the score.
    public void UpdateScore(int gemScore){
        score += gemScore;
    }
}
