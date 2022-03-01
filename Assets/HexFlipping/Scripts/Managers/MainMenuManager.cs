using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Script used to manage main menu 'sets' or pages which have animated transitions that are hardcoded into delays here
public class MainMenuManager : MonoBehaviour {
//Variables for object positioning, grid reference
    public GameObject[] sets;
    Animator lvlSelect, tutorial;
    public enum MenuState { LevelSelect, Tutorial, LevelSelected, ResetProgress, Quit };
    [SerializeField] MenuState currentState;

    public TileGrid grid;
    GridDefinition passedGridDef = null;

//Events for activating the GameManager
    public delegate void OnLevelSelect(string name, GridDefinition gridDef);
    public event OnLevelSelect LoadSceneCallback;
    public event OnLevelSelect QuitButtonCallback;
    public event OnLevelSelect ResetProgressCallback;

    public static MainMenuManager instance;
    void Awake() { 
		if (instance != null) { 
			Debug.Log("More than one instance of MainMenuManager found!");
    		return;
    	}
    	instance = this;
	}
//Scene load animator references, default animation states
    void Start() {
        lvlSelect = sets[0].GetComponent<Animator>();
        tutorial = sets[1].GetComponent<Animator>();

        TriggerMenuChange(0);
    }
//Sets anims for transitioning to tutorial page, waits for grid gen
    public void TriggerTutorial() {
        lvlSelect.SetBool("OnScreen", false);
        tutorial.SetBool("OnScreen", true);
        StartCoroutine(WaitToGen());
    }
//Delay until grid starts generating when page is done animating
    IEnumerator WaitToGen() {
        yield return new WaitForSeconds(.85f);

        grid.GenerateGrid();
    }
//Starts grid degen and wait until it finishes
     public void TriggerReturnToLevelSelect() {
        grid.DegenerateGrid(false);
        StartCoroutine(WaitToReturn());
     }
//Sets anims for transitioning to level select page after waiting
    IEnumerator WaitToReturn() {
        yield return new WaitForSeconds(.85f);
        lvlSelect.SetBool("OnScreen", true);
        tutorial.SetBool("OnScreen", false);
    }
//Sets anims for transitioning to levels then waits to load level
    public void TriggerLevelSelection(GridDefinition gridDef) {
        lvlSelect.SetBool("Left", false);
        lvlSelect.SetBool("OnScreen", false);
        StartCoroutine(WaitToLoadLevel(gridDef));
    }
//Calls a callback for either the GameManager to LoadScene(string, GridDef) or to ResetProgress()
    IEnumerator WaitToLoadLevel(GridDefinition gridDef) {
        yield return new WaitForSeconds(.85f);
        if (gridDef == null)
            ResetProgressCallback?.Invoke(null, null);
        else             
            LoadSceneCallback?.Invoke("TileGenerator", gridDef);
          
    }
//Call to GameManager to QuitApplication()
    public void TriggerQuitButton() {
        lvlSelect.SetBool("Left", false);
        lvlSelect.SetBool("OnScreen", false);
        StartCoroutine(WaitToQuit());
    }
//Waits for set to finish animating to call
    IEnumerator WaitToQuit() {
        yield return new WaitForSeconds(.85f);
        QuitButtonCallback?.Invoke(null, null);
    }
//Call to GameManager to ResetProgress()
    public void ResetProgressButton() {
        lvlSelect.SetBool("Left", false);
        lvlSelect.SetBool("OnScreen", false);
        StartCoroutine(WaitToLoadLevel(null));
    }

    public void PassGridDefinition(GridDefinition gridDef) {
        passedGridDef = gridDef;
    }

    public void TriggerMenuChange(int stateIndex) {
        StartCoroutine(ChangeMenuState(stateIndex));
    }

    IEnumerator ChangeMenuState(int state) { 
        switch(state) {
            default:
                break;
            case 0:
                if (!lvlSelect.GetBool("OnScreen") && tutorial.GetBool("OnScreen")) {
                    grid.DegenerateGrid(false);
                    yield return new WaitForSeconds(0.85f);
                    lvlSelect.SetBool("Left", true);
                    lvlSelect.SetBool("OnScreen", true);
                    tutorial.SetBool("Left", false);
                    tutorial.SetBool("OnScreen", false);
                } else if (!lvlSelect.GetBool("OnScreen") && !tutorial.GetBool("OnScreen")) {
                    lvlSelect.SetBool("Left", false);
                    lvlSelect.SetBool("OnScreen", true);
                    lvlSelect.Play("setOffscreen");
                    tutorial.SetBool("Left", false);
                    tutorial.SetBool("OnScreen", false);
                    tutorial.Play("setOffscreen");
                }
                break;
            case 1:
                if (lvlSelect.GetBool("OnScreen")  && !tutorial.GetBool("OnScreen")) {
                    lvlSelect.SetBool("Left", true);
                    lvlSelect.SetBool("OnScreen", false);
                    tutorial.SetBool("Left", false);
                    tutorial.SetBool("OnScreen", true);                   
                    yield return new WaitForSeconds(0.85f);
                    grid.GenerateGrid();
                }

                break;
            case 2:
            case 3:
            case 4:
                if (lvlSelect.GetBool("OnScreen") && !tutorial.GetBool("OnScreen")) {
                    lvlSelect.SetBool("Left", false);
                    lvlSelect.SetBool("OnScreen", false);
                }
                yield return new WaitForSeconds(0.85f);
                if (state == 2 && passedGridDef != null)
                    LoadSceneCallback?.Invoke("TileGenerator", passedGridDef);
                if (state == 3)
                    ResetProgressCallback?.Invoke(null, null);
                if (state == 4)
                    QuitButtonCallback?.Invoke(null, null);
                break;             
        }
    }
}