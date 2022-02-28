	using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Quick UI script for placeholder game mechanics
[RequireComponent(typeof(Animator))]
public class LevelUI : MonoBehaviour {
 //Define variables used to display UI and store values
	public TileGrid grid;

	public Slider[] sliders;
	public Text[] valuesText;
	public Text[] goalsText;

	bool[] hasGoal = new bool[3];

	Animator totalsAnim;

	int[] values = new int[3];
	int[] goals = new int[3];
//Event for announcments, win
	public delegate void OnEndCondition();
	public event OnEndCondition OnWinCallback;
	bool called = false;

//My singleton
	public static LevelUI instance;
	void Awake() { 
		if (instance != null) { 
			Debug.Log("More than one instance of TotalsDisplay found!");
    		return;
    	}
    	instance = this;
	}

//Reference instance, subscribe to events
	void Start() {		
		grid = TileGrid.instance; //Grab instance ref for event callbacks to update UI from TileGenerator
		grid.GridCalculatedCallback += TogglePanel;	//Events which trigger UI updates
		grid.GridCalculatedCallback += UpdateGoals;
		grid.TotalsChangeCallback += UpdateTotals;

		totalsAnim = GetComponent<Animator>();
	}
//Plays UI animation when level start and end is triggered
	void TogglePanel(float r, float g, float b) { //still passing useless variables >.>
		bool current = totalsAnim.GetBool("Open");
		totalsAnim.SetBool("Open", !current);
	}

//Update the totals required to win in the UI when grid is calculated
	void UpdateGoals(float r, float g, float b) {
		int totalTiles = (int)(grid.gridDef.gridDim.x * grid.gridDef.gridDim.y);
		goals[0] = (int)(totalTiles * r);
		goals[1] = (int)(totalTiles * g);
		goals[2] = (int)(totalTiles * b);

		if (goals[0] == 0) hasGoal[0] = false;
		else hasGoal[0] = true;
		if (goals[1] == 0) hasGoal[1] = false;
		else hasGoal[1] = true;
		if (goals[2] == 0) hasGoal[2] = false;
		else hasGoal[2] = true;

		DestroyUnusedSliders();
		UpdateDisplay();
	}
	//

//Update current values stored in each colors slider each time a hex is flipped in TileGrid
bool firstCheck; //Used so that a win is not called on the origin TileFlip
	void UpdateTotals(float r, float g, float b) {
		values[0] = (int)r;
		values[1] = (int)g;
		values[2] = (int)b;

		UpdateDisplay();

        if (values[0] >= goals[0] && values[1] >= goals[1] && values[2] >= goals[2]) {
			if (firstCheck) firstCheck = false; // this feels very sneaky
			else if (!called) {
				called = true;
				OnWinCallback?.Invoke();
				grid.DegenerateGrid(true);
				StartCoroutine(WaitToToggle());
			}
        } else {
			firstCheck = true;
		}
    }
//
//
//Update all fields of the UI elements which display win conditions
	void UpdateDisplay() {
		for (int i = 0; i < sliders.Length; i++) {
			if (hasGoal[i]) {
				sliders[i].maxValue = goals[i];
				sliders[i].value = values[i];
				goalsText[i].text = "/" + goals[i].ToString();
				valuesText[i].text = values[i].ToString();
			}			
		}
	}
//Utilizing LayoutGrid component, if a color is not required to win a level it's slider can be destroyed
	void DestroyUnusedSliders() { 
		for (int i = 0; i < hasGoal.Length; i++) { 
			if (!hasGoal[i]) {
				Destroy(sliders[i].gameObject);
			}
		}
	}

	public void ExitButton() {
		grid.DegenerateGrid(false);
		StartCoroutine(WaitToToggle());
	}

	IEnumerator WaitToToggle() {
		yield return new WaitForSeconds(1);
		TogglePanel(0, 0, 0);
	}
}
