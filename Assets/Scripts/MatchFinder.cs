using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MatchFinder : MonoBehaviour
{
    /////////////////////////////////////
    ////////////// FIELDS ///////////////
    /////////////////////////////////////
    Board board;
    [SerializeField] List<Gem> currentMatches = new List<Gem>();


    /////////////////////////////////////
    /////////// START & UPDATE //////////
    /////////////////////////////////////

    // Board is setup on start. Used awake to prevent null reference exception.
    void Awake()
    {
        board = FindObjectOfType<Board>();
    }


    /////////////////////////////////////
    ////////////// METHODS //////////////
    /////////////////////////////////////

    // This method is to find all the matches on the board.
    public void FindAllMatches(){
        // Clear the previous matches.
        currentMatches.Clear();

        // Loop around the board and look for mathces.
        for(int x = 0; x < board.GetWidth(); x++){
            for(int y = 0; y < board.GetHeight(); y++){
                Gem currentGem = board.allGems[x,y];
                // Gem is not null
                if(currentGem != null){
                    // Gem is not on the left or the right-end side of the board.
                    if(x > 0 && x < board.GetWidth()-1){
                        // Get the gems on the left and right. 
                        Gem leftGem  = board.allGems[x-1,y];
                        Gem rightGem = board.allGems[x+1,y];
                        if(leftGem != null & rightGem != null){
                            // If all have the same type we have a match.
                            if(leftGem.type == currentGem.type && rightGem.type == currentGem.type){
                                currentGem.SetIsMatched(true);
                                leftGem.SetIsMatched(true);
                                rightGem.SetIsMatched(true);
                                currentMatches.Add(currentGem);
                                currentMatches.Add(leftGem);
                                currentMatches.Add(rightGem);
                            }
                        }
                    }
                    // Gem is not on the bottom or the top of the board.
                    if(y > 0 && y < board.GetHeight()-1){
                        // Get the gems on the down and up.
                        Gem aboveGem  = board.allGems[x,y-1];
                        Gem beloveGem = board.allGems[x,y+1];
                        // If all have the same type we have a match.
                        if(beloveGem != null & aboveGem != null){
                            if(beloveGem.type == currentGem.type && aboveGem.type == currentGem.type){
                                currentGem.SetIsMatched(true);
                                beloveGem.SetIsMatched(true);
                                aboveGem.SetIsMatched(true);
                                currentMatches.Add(currentGem);
                                currentMatches.Add(beloveGem);
                                currentMatches.Add(aboveGem);
                            }
                        }
                    }
                }
            }
        }

        // Remove the same elements from the list.
        if(currentMatches.Count > 0){
            currentMatches = currentMatches.Distinct().ToList();
        }

        // Check for bombs.
        CheckBombsAndMark();
    }

    // This method is to check for bombs and mark the gems around.
    public void CheckBombsAndMark(){
        for(int i = 0; i < currentMatches.Count; i++){
            Gem gem = currentMatches[i];
            int x = gem.pos.x;
            int y = gem.pos.y;
            // Check the left side for a bomb.
            if(gem.pos.x > 0)
                CheckPosForBomb(new Vector2Int(x-1, y));
            // Check the right side for a bomb.
            if(gem.pos.x < board.GetWidth() - 1)
                CheckPosForBomb(new Vector2Int(x+1, y));
            // Check the below for a bomb.
            if(gem.pos.y > 0)
                CheckPosForBomb(new Vector2Int(x, y-1));
            // Check the above for a bomb.
            if(gem.pos.y < board.GetHeight() - 1)
                CheckPosForBomb(new Vector2Int(x, y+1));
        }
    }

    // This method is to check a specific position for a bomb.
    private void CheckPosForBomb(Vector2Int bombPos){
        if(board.allGems[bombPos.x,bombPos.y] != null){
            Gem bomb = board.allGems[bombPos.x, bombPos.y];
            if(bomb.type == Gem.GemType.bomb){
                MarkBombArea(new Vector2Int(bombPos.x, bombPos.y), bomb);
            }
        }
    }

    // This method is to mark the gems around the bomb as "matched".
    public void MarkBombArea(Vector2Int bombPos, Gem theBomb){
        // Get the blast size of the bomb.
        int blastSize = theBomb.GetBombRadius();
        // Loop around the potantial gems around the bomb.
        for(int x = bombPos.x - blastSize; x < bombPos.x + blastSize ; x++){
            for(int y = bombPos.y - blastSize; y < bombPos.y + blastSize; y++){
                // If the gem's is valid, mark it as matched.
                if(x >= 0 && x < board.GetWidth() && y >= 0 && y < board.GetHeight()){
                    Gem gem = board.allGems[x,y];
                    if(gem != null){
                        gem.SetIsMatched(true);
                        currentMatches.Add(gem);
                    }
                }
            }
        }
        currentMatches = currentMatches.Distinct().ToList();
    }

    // Getter for current matches.
    public List<Gem> GetCurrentMatches(){
        return currentMatches;
    }
}
