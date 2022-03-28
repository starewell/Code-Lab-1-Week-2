using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjacentFlipActor : Actor {
    /// <summary>
    /// This actor flips all adjacent tiles just like the player
    /// </summary>

    //Override to make Actor specific actions, in this case move and flip adjacent spaces
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
        StartCoroutine(currentSpace.GetComponent<TileFlip>().FlipTile(true, false));
        Debug.Log("Fliped Tile");
        takingTurn = false;
    }
}
