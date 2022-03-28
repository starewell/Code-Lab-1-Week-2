using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStateMachine : MonoBehaviour {
    /// <summary>
    /// Turn based gameplay! This state machine is responsible for locking and unlocking the FlipGrid when it's the player's turn or Actor's
    /// In the future, this would be the class I would want to refactor a lot of my in-game management code that's scattered about, such as Setup and Cleanup
    /// </summary>
    [HideInInspector]
    public FlipGameManager gameManager;
    [HideInInspector]
    public FlipGrid grid;

    public enum LevelState { Setup, PlayerTurn, ActorTurn, Cleanup };
    public LevelState currentState;

    #region Singleton
    //Week 2 singleton pattern
    private static LevelStateMachine instance;
    public static LevelStateMachine GetInstance() {
        return instance;
    }
    void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }     
    }
    #endregion

    //Get refs, like and subscribe
    private void Start() {
        gameManager = FlipGameManager.GetInstance();
        if (FlipGrid.instance != null) 
            grid = FlipGrid.instance;
        grid.GridGeneratedCallback += StartLevel;
        grid.TurnTakenCallback += TransitionStates;
        grid.LevelExitCallback += EndLevel;


        currentState = LevelState.Setup;
        StartCoroutine(RunCurrentState());
    }

    //I hate teeny functions like these, but I guess they're cheap
    private void TransitionStates(LevelState state) {
        currentState = state;
        StartCoroutine(RunCurrentState());
    }

    //Hardly a coroutine, state switch machine w/ one case w/ a delay. Otherwise locks and unlocks grid while player or Actor's are playing
    private IEnumerator RunCurrentState() { 
        switch(currentState) {
            default:
                break;
            case LevelState.Setup:
                grid.LockHexGrid();
                break;
            case LevelState.PlayerTurn:
                grid.UnlockHexGrid();
                break;
            case LevelState.ActorTurn:
                grid.LockHexGrid();
                foreach(Actor actor in grid.gridActors) {
                    actor.TakeTurn();
                    yield return new WaitForEndOfFrame(); //Delay so that all Actors don't execute at once, causing them to occasionally occupy the same spaces !
                }
                break;
            case LevelState.Cleanup:
                grid.LockHexGrid();
                break;
        }
        yield return null;
    }

    //Setup and Cleanup looking sad down here
    void StartLevel(float r, float g, float b) {
        currentState = LevelState.PlayerTurn;
        RunCurrentState();
    }
    private void EndLevel(LevelState state) {
        currentState = LevelState.Cleanup;
        RunCurrentState();
    }


}
