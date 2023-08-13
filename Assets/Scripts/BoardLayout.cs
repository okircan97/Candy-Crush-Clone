using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardLayout : MonoBehaviour
{
    /////////////////////////////////////
    ////////////// FIELDS ///////////////
    /////////////////////////////////////

    // (0,0) on the real board will be represented
    // as (4,0) in this array of LayoutRows.
    public LayoutRow[] allRows;


    /////////////////////////////////////
    ////////////// METHODS //////////////
    /////////////////////////////////////

    public Gem[,] GetLayout(){
        Gem[,] layout = new Gem[allRows[0].gemsInRow.Length, allRows.Length];

        // for(int y = 0; y < allRows.Length; y++){
        //     for(int x = 0; x < allRows[y].gemsInRow.Length; x++){
        //         // x should be smaller than the width. This if 
        //         // statement is to prevent possible problems
        //         // that might happen if the width and the height
        //         // are different values. 
        //         // layout.GetLength(0) is the same with allRows[0].gemsInRow.Length
        //         if(x < allRows[0].gemsInRow.Length){
        //             if(allRows[y].gemsInRow[x] != null){
        //                 // Convert the layout to the board style.
        //                 layout[x, allRows.Length - 1 - y] = allRows[y].gemsInRow[x];
        //             }
        //         }
        //     }
        // }

        for(int y = 0; y < allRows.Length; y++){
            for(int x = 0; x < allRows[y].gemsInRow.Length; x++){
                if(allRows[y].gemsInRow[x] != null){
                    // Convert the layout to the board style.
                    layout[x, allRows.Length - 1 - y] = allRows[y].gemsInRow[x];
                }
            }
        }

        return layout;
    }
}

// This class is to show the gems as rows on
// the inspector. (Since, it is not possible 
// to show 2D arrays.)
[System.Serializable]
public class LayoutRow{
    public Gem[] gemsInRow;
}
