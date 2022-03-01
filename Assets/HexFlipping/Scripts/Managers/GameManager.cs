using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    SceneLoader sceneLoader;
    FileIO fileIO;

    //Instances that are created and destroyed as scenes load
    TileGrid grid = null;
    MainMenuManager menu;
    ProgressionManager prog;

    GridDefinition currentGrid; //passes GridDefinitions between scenes

    public List<string> prevUnlockedLvls = new List<string>(); //Used to trigger unlocking animations 
    public List<string> unlockedLvls = new List<string>(); //Data that is saved to file, updated through TileGrid DegenerateGrid(true) string from GridDefinition

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
        }     
    }

    void Start() {
        sceneLoader = SceneLoader.GetInstance(); //Week 2 singleton referencing
        sceneLoader.SceneLoadedCallback += OnSceneLoaded; //Subscribe to SceneLoaded delegate
        fileIO = FileIO.GetInstance();

        UpdateInstances();
        ReadProgress();
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
        grid.DegenerateGrid(false);
    }

//Function called when a scene finishes loading
    public void OnSceneLoaded() {
        UpdateInstances(); //Hook the GameManager up to new instances if they exist

        if (currentGrid != null && currentGrid.level) 
            StartCoroutine(GenerateGrid());          
        
        if (prog != null) 
            prog.UpdateUnlocks(unlockedLvls, prevUnlockedLvls);                 
    }
//Coroutine to delay the start of grid generation so references are made
    IEnumerator GenerateGrid() {
        yield return new WaitForSeconds(.1f);
        if (grid != null) {
            grid.gridDef = currentGrid;
            grid.GenerateGrid();
        }              
    }
//~~
//WEEK 4 FUNCTIONS
//Not saving much data here, but it saves! called from TileGrid
    void AddProgress(string[] lvls) {
        prevUnlockedLvls = new List<string>();
        foreach (string lvl in unlockedLvls) {
            prevUnlockedLvls.Add(lvl);
        }
        foreach (string lvl in lvls) {
            if (unlockedLvls.Find(x => x == lvl) == null)
                unlockedLvls.Add(lvl);
        }
        fileIO.SaveProgress(unlockedLvls);
    }

//Read saved file and add saved unlocked levels to lists, called at Start
    void ReadProgress() {
        unlockedLvls = new List<string>();
        prevUnlockedLvls = new List<string>();
        List<string> fileLvls = fileIO.LoadProgress();
        foreach(string lvl in fileLvls) {
            unlockedLvls.Add(lvl);
            prevUnlockedLvls.Add(lvl);
        }
        if (prog != null)
            prog.UpdateUnlocks(unlockedLvls, prevUnlockedLvls);
    }
//Purge save file and GameManager lvl Lists, called from MenuManager
    void ResetProgress(string name, GridDefinition def) { //hijacking an event and passing useless variables >.>
        fileIO.EraseProgress();
        unlockedLvls = new List<string>();
        prevUnlockedLvls = new List<string>();
        LoadScene("Menu", null);
    }
//~~

//Creates references to instanced scripts if present in the scene
    void UpdateInstances() {
        if (TileGrid.instance != null) grid = TileGrid.instance;
        if (MainMenuManager.instance != null) menu = MainMenuManager.instance;
        if (ProgressionManager.GetInstance() != null) prog = ProgressionManager.GetInstance();

        if (grid != null) grid.LevelEndCallback += ReturnToMenu;
        if (grid != null) grid.GridSolvedCallback += AddProgress;
        if (menu != null) menu.LoadSceneCallback += LoadScene;
        if (menu != null) menu.ResetProgressCallback += ResetProgress;
        if (menu != null) menu.QuitButtonCallback += QuitApplication;
    }
//Removes references to instanced scripts if present in the class
    void RemoveInstances() {
        if (grid != null) grid.LevelEndCallback -= ReturnToMenu;
        if (grid != null) grid.GridSolvedCallback -= AddProgress;
        if (menu != null) menu.LoadSceneCallback -= LoadScene;
        if (menu != null) menu.ResetProgressCallback -= ResetProgress;
        if (menu != null) menu.QuitButtonCallback -= QuitApplication;

        grid = null;
        menu = null;
        prog = null;
    }
//Goodbye world!
    public void QuitApplication(string name, GridDefinition def) {
        Application.Quit();
    }
}