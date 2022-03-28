using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PadlockActor : Actor {
    /// <summary>
    /// 
    /// </summary>
     
    //Override to make Actor specific actions, in this case move to an adjacent space and occupy it, preventing the player from flipping it
    public override void TakeTurn() {
        base.TakeTurn();
        
        List<HexSpace> legalSpaces = FindAdjacentSpaces();
        legalSpaces.Shuffle();
        HexSpace targetSpace = legalSpaces[0];

        TargetSpace(targetSpace);
        takingTurn = false;
    }

    //This was meant to be the overriden Coroutine that could signal when the Actor is done with it's actions
    //protected override void MoveToSpace() {
    //    base.MoveToSpace();

    //    takingTurn = false;
    //}

}
