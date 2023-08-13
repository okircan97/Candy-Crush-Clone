using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    /////////////////////////////////////
    ////////////// FIELDS ///////////////
    /////////////////////////////////////

    // General fields.
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] GameObject bgTileprefab;
    MatchFinder matchFinder;
    RoundManager roundManager;

    // Gem fields.
    [SerializeField] Gem[] gemPrefabs;  
    public Gem[,] allGems;
    float gemSpeed = 10f;    

    // Enum to change the game state.
    public enum BoardState {wait, move};
    public BoardState currentState = BoardState.move;

    // Bomb fields.
    [SerializeField] Gem bomb;
    float bombChance = 5f;

    // Score multiplier fields.
    float bonusMultiplier = 0;
    float bonusAmount = .5f;

    // Custom board layout fields.
    BoardLayout boardLayout;
    Gem[,] layout;


    /////////////////////////////////////
    /////////// START & UPDATE //////////
    /////////////////////////////////////

    void Start(){
        matchFinder = FindObjectOfType<MatchFinder>();
        roundManager = FindObjectOfType<RoundManager>();
        boardLayout = GetComponent<BoardLayout>();

        allGems = new Gem[width, height];
        layout  = new Gem[width, height];
        SetupBoard();

    }

    void Update() {
        // If "S" is pressed shuffle the board.
        if(Input.GetKeyDown(KeyCode.S)){
            ShuffleTheBoard();
        }
    }


    /////////////////////////////////////
    ////////////// METHODS //////////////
    /////////////////////////////////////

    // This method is to setup the board.
    private void SetupBoard(){

        // Check if there's a custom layout.
        if(boardLayout != null){
            layout = boardLayout.GetLayout();
        }

        for(int x = 0; x < width; x++){
            for(int y = 0; y < height; y++){
                // Instantiate the background tile.
                Vector2 pos = new Vector2(x,y);
                GameObject bgTile = Instantiate(bgTileprefab, pos, Quaternion.identity);
                bgTile.transform.SetParent(gameObject.transform); 
                bgTile.name = "Tile - " + x.ToString() + ", " + y.ToString();
                // If there's a custom layout, get the gem from there.
                if(layout[x,y] != null){
                    // Spawn the gem.
                    SpawnGem(new Vector2Int(x,y), layout[x,y]);
                }
                else{
                    // If not, get a random gem.
                    int gemNum = Random.Range(0, gemPrefabs.Length);
                    // Check if there's a match on the given pos and if so,
                    // change the gem type.
                    while(MatchesAt(new Vector2Int(x,y), gemPrefabs[gemNum])){
                        gemNum = Random.Range(0, gemPrefabs.Length);
                    }
                    // Spawn the gem.
                    SpawnGem(new Vector2Int(x,y), gemPrefabs[gemNum]);
                }
            }
        }
    }

    // This method is to spawn a gem.
    private void SpawnGem(Vector2Int pos, Gem gemPrefab){

        // Spawn the bomb with "bombChance" percent chance.
        if(Random.Range(0,100) < bombChance){
            gemPrefab = bomb;
        }

        // Instantiate the gem.
        Gem gem = Instantiate(gemPrefab, new Vector2(pos.x, pos.y + height), Quaternion.identity);
        gem.transform.SetParent(gameObject.transform); 
        gem.name = "Gem - " + pos.x.ToString() + ", " + pos.y.ToString();
        // Store the gem inside the 2d array.
        allGems[pos.x, pos.y] = gem;
        gem.SetupGem(pos, this);
    }

    // This method is to check a match at a specific pos for 
    // a specific gem.
    bool MatchesAt(Vector2Int pos2Check, Gem gem2Check){
        // Check the gems on the left side.
        if(pos2Check.x > 1){
            if(allGems[pos2Check.x-1, pos2Check.y].type == gem2Check.type &&
               allGems[pos2Check.x-2, pos2Check.y].type == gem2Check.type){
                return true;
            }
        }
        // Check the gems below.
        if(pos2Check.y > 1){
            if(allGems[pos2Check.x, pos2Check.y-1].type == gem2Check.type &&
               allGems[pos2Check.x, pos2Check.y-2].type == gem2Check.type){
                return true;
            }
        }
        // If no match, return false.
        return false;
    }

    // This method is to destroy a gem at a certain pos.
    private void DestroyMatchedGemAt(Vector2Int pos){
        // If the gem is not null and matched, destroy it.
        Gem gem = allGems[pos.x, pos.y];
        if(gem != null){
            if(gem.GetIsMatched()){
                Instantiate(gem.GetDestroyEffect(), 
                            new Vector2(pos.x, pos.y), 
                            Quaternion.identity);
                UpdateScore(gem);
                Destroy(allGems[pos.x, pos.y].gameObject);
                allGems[pos.x, pos.y] = null;
            }
        }
    }

    // This method is to destroy all the matched gems.
    public void DestroyMatches(){
        List<Gem> currentMatches = matchFinder.GetCurrentMatches();
        for(int i = 0; i < currentMatches.Count; i++){
            if(currentMatches[i] != null){
                DestroyMatchedGemAt(currentMatches[i].pos);
            }
        }
        StartCoroutine(DecreaseRow());
    }

    // This coroutine is to drop the gems according the 
    // number of empty spaces below them.
    private IEnumerator DecreaseRow(){
        yield return new WaitForSeconds(.5f);
        int nullCounter = 0;
        for(int x = 0; x < width; x++){
            for(int y = 0; y < height; y++){
                // Get the number of adjacent null spaces.
                if(allGems[x,y] == null){
                    nullCounter++;
                }
                // If not null, swap it with the lowest null space.
                else if(nullCounter > 0){
                    allGems[x,y].pos.y -= nullCounter;
                    allGems[x,y-nullCounter] = allGems[x,y];
                    allGems[x,y] = null;
                }
            }
            nullCounter = 0;
        }
        // Refill the empty cells.
        StartCoroutine(WaitAndRefillBoard());
    }

    // This method is to refill the board.
    private void RefillBoard(){
        for(int x = 0; x < width; x++){
            for(int y = 0; y < height; y++){
                if(allGems[x,y] == null){
                    int gemNum = Random.Range(0,gemPrefabs.Length);
                    SpawnGem(new Vector2Int(x,y), gemPrefabs[gemNum]);
                }
            }
        }
        // Check for unassigned gems and destroy them.
        CheckMisplacedGems();
    }

    // This coroutine is to wait and call the RefillBoard().
    private IEnumerator WaitAndRefillBoard(){
        // Refill the board.
        yield return new WaitForSeconds(.5f);
        RefillBoard();
        // If there are new matches after the refill, destroy
        // them as well.
        matchFinder.FindAllMatches();
        if(matchFinder.GetCurrentMatches().Count > 0){
            bonusMultiplier++;
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        // If there are no more matches, unlock the board.
        else{
            yield return new WaitForSeconds(.5f);
            currentState = BoardState.move;
            bonusMultiplier = 0f;
        }
    }

    // This method is to check if there's any unassigned gems 
    // for any reason and remove them. It won't be needed most
    // propably. It's just to be safe.
    private void CheckMisplacedGems(){
        // Get all the gems inside a new list.
        List<Gem> foundGems = new List<Gem>();
        foundGems.AddRange(FindObjectsOfType<Gem>());
        // Remove the ones found inside allGems.
        for(int x = 0; x < width; x++){
            for(int y = 0; y < height; y++){
                if(foundGems.Contains(allGems[x,y])){
                    foundGems.Remove(allGems[x,y]);
                }
            }
        }
        // There shouldn't left any gems. If there's any,
        // remove them.
        foreach(Gem gem in foundGems){
            Destroy(gem.gameObject);
        }
    }

    // This method is to shuffle the board.
    public void ShuffleTheBoard(){
        if(currentState != BoardState.wait){
            currentState = BoardState.wait;
            List<Gem> gems = new List<Gem>();
            // Get all the gems inside a new list.
            for(int x = 0; x < width; x++){
                for(int y = 0; y < height; y++){
                    gems.Add(allGems[x,y]);
                    allGems[x,y] = null;
                }
            }
            // Put the gems back at new positions.
            for(int x = 0; x < width; x++){
                for(int y = 0; y < height; y++){
                    int gemNum = Random.Range(0,gems.Count);
                    // Put a gem to a place till is no match there.
                    // If the gem is the last one there might be a 
                    // match though. Also, don't iterate more than a 
                    // hundred times.
                    int iterationNum = 0;
                    while(MatchesAt(new Vector2Int(x,y), gems[gemNum]) 
                          && iterationNum < 100
                          && gems.Count > 1){
                        gemNum = Random.Range(0,gems.Count);
                        iterationNum++;
                    }
                    gems[gemNum].SetupGem(new Vector2Int(x,y), this);
                    allGems[x,y] = gems[gemNum];
                    gems.RemoveAt(gemNum);
                }
            }
        }
        // Destroy the gems if there're any new matches. 
        StartCoroutine(WaitAndRefillBoard());
    }

    // This method is to update the score.
    public void UpdateScore(Gem gem){
        roundManager.UpdateScore(gem.GetGemScore());
        
        if(bonusMultiplier > 0){
            float bonusToAdd = gem.GetGemScore() * bonusMultiplier * bonusAmount;
            roundManager.UpdateScore(Mathf.RoundToInt(bonusToAdd));
            Debug.Log(bonusToAdd + " bonus added");
        }
    }

    // Getter for width.
    public float GetWidth(){
        return width;
    }

    // Getter for height.
    public float GetHeight(){
        return height;
    }

    // Getter for gem speed.
    public float GetGemSpeed(){
        return gemSpeed;
    }

    // Getter for match finder.
    public MatchFinder GetMatchFinder(){
        return matchFinder;
    }
}
