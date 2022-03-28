using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleFlipActor : Actor {
    /// <summary>
    /// This actor flips the tile it lands on
    /// </summary>

    //Override to make Actor specific actions, in this case move and flip its space
    public override void TakeTurn() {
        base.TakeTurn();
        
        List<HexSpace> legalSpaces = FindAdjacentSpaces();
        legalSpaces.Shuffle();
        HexSpace targetSpace = legalSpaces[0];

        TargetSpace(targetSpace);
        StartCoroutine(WaitToFlip());
    }

    //Wait until the actor has arrived at their new HexSpace
    IEnumerator WaitToFlip() {
        while(moving)
            yield return new WaitForEndOfFrame();
        StartCoroutine(currentSpace.GetComponent<TileFlip>().FlipTile(false, false));
        Debug.Log("Fliped Tile");
        takingTurn = false;
    }

}
