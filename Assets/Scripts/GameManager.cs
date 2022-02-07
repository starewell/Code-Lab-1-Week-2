using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    SceneLoader sceneLoader;

//Instances that are created and destroyed as scenes load
    TileGrid grid = null;
    MainMenuManager menu;

    GridDefinition currentGrid; //passes GridDefinitions between scenes

    public GameObject[] returnButtons; //Quit and Exit buttons always attached to GameManager so they can call it's functions

//Week 2 singleton pattern
    private static GameManager instance;
    public static GameManager GetInstance() {
        return instance;
    }
    void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    void Start() {
        sceneLoader = SceneLoader.GetInstance(); //Week 2 singleton referencing
        sceneLoader.SceneLoadedCallback += OnSceneLoaded; //Subscribe to SceneLoaded delegate
        returnButtons[0].SetActive(true); //toggle buttons
        returnButtons[1].SetActive(false);

        UpdateInstances();
    }

//Utility function to load scene, purge GameManager of references, and pass GridDefinitions between scenes
    public void LoadScene(string name, GridDefinition def) {
        RemoveInstances();
        currentGrid = def;

        sceneLoader.ChangeScene(name);      
    }   
//Specific function for returning to menu so that no GridReference is passed
    public void ReturnToMenu(float r, float g, float b) {
        RemoveInstances();
        currentGrid = null;

        sceneLoader.ChangeScene("Menu");
    }
    public void ExitLevel() {
        grid.DegenerateGrid();
    }
//Function called when a scene finishes loading
    public void OnSceneLoaded() {
        UpdateInstances(); //Hook the GameManager up to new instances if they exist

        if (currentGrid != null && currentGrid.level) {
            StartCoroutine(GenerateGrid());
            returnButtons[0].SetActive(false); //toggle buttons
            returnButtons[1].SetActive(true);
        } else {
            returnButtons[0].SetActive(true); //toggle buttons
            returnButtons[1].SetActive(false);
        }
    }
//Coroutine to delay the start of grid generation so references are made
    IEnumerator GenerateGrid() {
        yield return new WaitForSeconds(.1f);
        if (grid != null) {
            grid.gridDef = currentGrid;
            grid.GenerateGrid();
        }              
    }

//Creates references to instanced scripts if present in the scene
    void UpdateInstances() {
        if (TileGrid.instance != null) grid = TileGrid.instance;
        if (MainMenuManager.instance != null) menu = MainMenuManager.instance;

        if (grid != null) grid.LevelEndCallback += ReturnToMenu;
        if (menu != null) menu.LoadSceneCallback += LoadScene;
    }
//Removes references to instanced scripts if present in the class
    void RemoveInstances() {
        if (grid != null) grid.LevelEndCallback -= ReturnToMenu;
        if (menu != null) menu.LoadSceneCallback -= LoadScene;

        grid = null;
        menu = null;
    }
//Goodbye world!
    public void QuitApplication() {
        Application.Quit();
    }
}