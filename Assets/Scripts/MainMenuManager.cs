using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour {
//Variables for object positioning, grid reference
    public GameObject[] sets;
    Animator lvlSelect, tutorial;

    public TileGrid grid;
//Events for activating the GameManager
    public delegate void OnLevelSelect(string name, GridDefinition gridDef);
    public event OnLevelSelect LoadSceneCallback;

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

        lvlSelect.SetBool("Left", true);
        lvlSelect.SetBool("OnScreen", true);
        tutorial.SetBool("Left", false);
        tutorial.SetBool("OnScreen", false);
        tutorial.Play("setOffscreen");
        lvlSelect.Play("setTranslateRightOnscreen");
    }
//Sets anims for transitioning to tutorial page, waits for grid gen
    public void TriggerTutorial() {
        lvlSelect.SetBool("OnScreen", false);
        tutorial.SetBool("OnScreen", true);
        AnimatorStateInfo info = tutorial.GetCurrentAnimatorStateInfo(0);
        StartCoroutine(WaitToGen(info));
    }
//Delay until grid starts generating when page is done animating
    IEnumerator WaitToGen(AnimatorStateInfo info) {
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
//...
    IEnumerator WaitToLoadLevel(GridDefinition gridDef) {
        yield return new WaitForSeconds(.85f);
        LoadSceneCallback?.Invoke("TileGenerator", gridDef);
    }

}
