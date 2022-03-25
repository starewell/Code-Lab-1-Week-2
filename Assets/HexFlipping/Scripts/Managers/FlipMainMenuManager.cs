using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Script used to manage main menu 'sets' or pages which have animated transitions that are hardcoded into delays here
//Menu has states that are toggled by button scripts. Superfluous functions here to accommodate the button script functionality
public class FlipMainMenuManager : MonoBehaviour {
//Variables for object positioning, grid reference
    public GameObject[] sets;
    Animator lvlSelect, tutorial;
    public enum MenuState { LevelSelect, Tutorial, LevelSelected, ResetProgress, Quit };
    [SerializeField] MenuState currentState;

    public FlipGrid grid;
    FlipGridDefinition passedGridDef = null;

//Events for activating the FlipGameManager
    public delegate void OnLevelSelect(string name = null, FlipGridDefinition gridDef = null);
    public event OnLevelSelect LoadSceneCallback;
    public event OnLevelSelect QuitButtonCallback;
    public event OnLevelSelect ResetProgressCallback;

//My singleton
    public static FlipMainMenuManager instance;
    void Awake() { 
		if (instance != null) { 
			Debug.Log("More than one instance of MainMenuManager found!");
    		return;
    	}
    	instance = this;
	}

//On scene load, get animator references, set default animation states
    void Start() {
        lvlSelect = sets[0].GetComponent<Animator>();
        tutorial = sets[1].GetComponent<Animator>();

        TriggerMenuChange(0);
    }
//had to create a second function to pass a GridDef for the FlipGameManager, as the button script only accepts one overload
    public void PassGridDefinition(FlipGridDefinition gridDef) {
        passedGridDef = gridDef;
    }
//public function to change state, currently hooked up to buttons so passes an int instead of the enum >.>
    public void TriggerMenuChange(int stateIndex) {
        StartCoroutine(ChangeMenuState(stateIndex));
    }
//Switch statement for my state machine. Currently switches an int index bc of butons, but the int corresponds to the enum index
//Is also a coroutine so that I can delay the execution for animations
    IEnumerator ChangeMenuState(int state) { 
        switch(state) {
            default:
                break;
            case 0: //Switch to LevelSelect
                if (currentState == MenuState.Tutorial) {
                    grid.DegenerateGrid(false);
                    yield return new WaitForSeconds(0.85f); //Wait for animation
                    lvlSelect.SetBool("Left", true);
                    lvlSelect.SetBool("OnScreen", true);
                    tutorial.SetBool("Left", false);
                    tutorial.SetBool("OnScreen", false);
                } else if (currentState != MenuState.Tutorial) {
                    lvlSelect.SetBool("Left", false);
                    lvlSelect.SetBool("OnScreen", true);
                    lvlSelect.Play("setOffscreen");
                    tutorial.SetBool("Left", false);
                    tutorial.SetBool("OnScreen", false);
                    tutorial.Play("setOffscreen");
                }
                currentState = MenuState.LevelSelect;
                break;
            case 1://Switch to Tutorial
                if (currentState == MenuState.LevelSelect) {
                    lvlSelect.SetBool("Left", true);
                    lvlSelect.SetBool("OnScreen", false);
                    tutorial.SetBool("Left", false);
                    tutorial.SetBool("OnScreen", true);                   
                    yield return new WaitForSeconds(0.85f); //Wait for animation
                    grid.GenerateGrid();
                }
                currentState = MenuState.Tutorial;
                break;
            case 2:
            case 3:
            case 4://Exit the main menu
                if (currentState == MenuState.LevelSelect) {
                    lvlSelect.SetBool("Left", false);
                    lvlSelect.SetBool("OnScreen", false);
                }
                yield return new WaitForSeconds(0.85f); //Wait for animation
                //Trigger action specific events
                if (state == 2 && passedGridDef != null) { 
                    LoadSceneCallback?.Invoke("TileGenerator", passedGridDef);
                    currentState = MenuState.LevelSelected;
                }
                if (state == 3) { 
                    ResetProgressCallback?.Invoke();
                    currentState = MenuState.ResetProgress;
                }
                if (state == 4) { 
                    QuitButtonCallback?.Invoke();
                    currentState = MenuState.Quit;
                }
                break;             
        }
    }
}

/* Refactored the following scripts into the neat little switch state machine above. 
 * Looks a lot cleaner above and easier to parse, and with that affordance I made it more explicit so it still looks the same length anyways >.>

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
    public void TriggerLevelSelection(FlipGridDefinition gridDef) {
        lvlSelect.SetBool("Left", false);
        lvlSelect.SetBool("OnScreen", false);
        StartCoroutine(WaitToLoadLevel(gridDef));
    }
//Calls a callback for either the FlipGameManager to LoadScene(string, GridDef) or to ResetProgress()
    IEnumerator WaitToLoadLevel(FlipGridDefinition gridDef) {
        yield return new WaitForSeconds(.85f);
        if (gridDef == null)
            ResetProgressCallback?.Invoke(null, null);
        else             
            LoadSceneCallback?.Invoke("TileGenerator", gridDef);
          
    }
//Call to FlipGameManager to QuitApplication()
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
//Call to FlipGameManager to ResetProgress()
    public void ResetProgressButton() {
        lvlSelect.SetBool("Left", false);
        lvlSelect.SetBool("OnScreen", false);
        StartCoroutine(WaitToLoadLevel(null));
    }

*/