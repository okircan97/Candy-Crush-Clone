using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    /////////////////////////////////////
    ////////////// FIELDS ///////////////
    /////////////////////////////////////

    // General fields.
    public Vector2Int pos;
    Board board;

    // Fields to move gems.
    Vector2 firstTouchPos;
    Vector2 lastTouchPos;
    bool mousePressed;
    float swipeAngle = 0;
    Gem otherGem;
    Vector2Int previousPos;

    // Fields to match and explode the gems.
    public enum GemType {blue, green, purple, red, yellow, bomb};
    public GemType type;
    [SerializeField] bool isMatched;
    [SerializeField] GameObject destroyEffect;
    int bombRadius = 2;
    [SerializeField] int gemScore = 10;


    /////////////////////////////////////
    /////////// START & UPDATE //////////
    /////////////////////////////////////

    void Start(){
        board = FindObjectOfType<Board>();
    }

    void Update(){
        
        // If there's any change on the gem position, move it.
        if(Vector2.Distance(transform.position, pos) > .1f){
            transform.position = Vector2.Lerp(transform.position, pos, board.GetGemSpeed()*Time.deltaTime);
        }
        // Lerp will try to move the gem forever. Else statement is to stop it.
        else{
            transform.position = new Vector3(pos.x, pos.y, 0);
        }

        // If the mouse button is clicked and then released,
        // get the last touch position.
        if(mousePressed && Input.GetMouseButtonUp(0)){
            lastTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
            mousePressed = false;
        }
    }

    private void OnMouseDown(){

        if(board.currentState == Board.BoardState.move){
            // Get the first first touch pos of the mouse.
            firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePressed = true;
        }
    }


    /////////////////////////////////////
    ////////////// METHODS //////////////
    /////////////////////////////////////

    // This method is to initialize the pos and board fields
    // of the gem.
    public void SetupGem(Vector2Int pos, Board board){
        this.pos = pos;
        this.board = board;
    }

    // This method is to calculate the angle between the first 
    // and the last touch.
    private void CalculateAngle(){
        // Get the angle (as degrees) between the first and the last touch.
        swipeAngle = Mathf.Atan2(lastTouchPos.y-firstTouchPos.y, lastTouchPos.x-firstTouchPos.x);
        swipeAngle *= Mathf.Rad2Deg;
        // If the distance between the first and the last touch is greater
        // than .5, change the gems' positions.
        if(Vector3.Distance(firstTouchPos, lastTouchPos) > .5f){
            MoveGems();
        }
    }

    // This method is to move the gems.
    private void MoveGems(){

        // Get the original position.
        previousPos = pos;

        // Move right.
        if(swipeAngle < 45 && swipeAngle > -45 && pos.x < board.GetWidth()-1){
            Debug.Log("right");
            otherGem = board.allGems[pos.x+1, pos.y];
            otherGem.pos.x--;
            pos.x++;
        }
        // Move up.
        else if(swipeAngle < 135 && swipeAngle > 45 && pos.y < board.GetHeight()-1){
            Debug.Log("up");
            otherGem = board.allGems[pos.x, pos.y+1];
            otherGem.pos.y--;
            pos.y++;
        }
        // Move left. 
        else if((swipeAngle > 135 || swipeAngle < -135) && pos.x > 0){
            Debug.Log("left");
            otherGem = board.allGems[pos.x-1, pos.y];
            otherGem.pos.x++;
            pos.x--;
        }
        // Move down.
        else if(swipeAngle < -45 && swipeAngle > -135 && pos.y > 0){
            Debug.Log("down");
            otherGem = board.allGems[pos.x, pos.y-1];
            otherGem.pos.y++;
            pos.y--;
        }

        // Check if there's a match and turn them back to their original 
        // positions if not.
        if(otherGem != null){
            UpdateGemIndexes();
            StartCoroutine(CheckMove());
        }
    }

    // This method is to update the gem indexes on the list.
    private void UpdateGemIndexes(){
        this.name     = "Gem - " + pos.x.ToString() + ", " + pos.y.ToString();
        otherGem.name = "Gem - " + otherGem.pos.x.ToString() + ", " + otherGem.pos.y.ToString();
        board.allGems[pos.x, pos.y] = this;
        board.allGems[otherGem.pos.x, otherGem.pos.y] = otherGem;
    }

    // This coroutine is to check whether there's a match and 
    // turn the gems back to their original positions if not.
    public IEnumerator CheckMove(){
        // Lock the board.
        board.currentState = Board.BoardState.wait;
        // Wait and find all the matches.
        yield return new WaitForSeconds(.5f);
        board.GetMatchFinder().FindAllMatches();
        // If there's no match for either, turn them back to their 
        // original positions and unlock the board.
        if(!isMatched && !otherGem.isMatched){
            otherGem.pos = pos;
            pos = previousPos;
            UpdateGemIndexes();
            yield return new WaitForSeconds(.5f);
            board.currentState = Board.BoardState.move;
        }
        // If there's, destroy the matches.
        else{
            board.DestroyMatches();
        }
    }

    // Getter for isMatched.
    public bool GetIsMatched(){
        return isMatched;
    }

    // Setter for isMatched.
    public void SetIsMatched(bool value){
        isMatched = value;
    }

    // Getter for destroy effect.
    public GameObject GetDestroyEffect(){
        return destroyEffect;
    }

    // Getter for bomb radius.
    public int GetBombRadius(){
        return bombRadius;
    }

    // Getter for gem score.
    public int GetGemScore(){
        return gemScore;
    }
}
