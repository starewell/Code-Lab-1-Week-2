using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipGameManager : MonoBehaviour {

    SceneLoader sceneLoader;
    FlipFileIO fileIO;
    LevelStateMachine levelStateMachine;

    //Instances that are created and destroyed as scenes load
    FlipGrid grid = null;
    FlipMainMenuManager menu;
    FlipProgressionManager prog;

    FlipGridDefinition currentGrid; //passes GridDefinitions between scenes

    public List<string> prevUnlockedLvls = new List<string>(); //Used to trigger unlocking animations 
    public List<string> unlockedLvls = new List<string>(); //Data that is saved to file, updated through FlipGrid DegenerateGrid(true) string from FlipGridDefinition

    //Week 2 singleton pattern
    private static FlipGameManager instance;
    public static FlipGameManager GetInstance() {
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
        fileIO = FlipFileIO.GetInstance();

        UpdateInstances();
        ReadProgress();
    }

//Utility function to load scene, purge FlipGameManager of references, and pass GridDefinitions between scenes
    public void LoadScene(string name, FlipGridDefinition def) {
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
        UpdateInstances(); //Hook the FlipGameManager up to new instances if they exist

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
//Not saving much data here, but it saves! called from FlipGrid
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
//Who doens't like a cheatcode? Good for debugging
    void UnlockAll(string name, FlipGridDefinition def) {
        fileIO.UnlockAll();
        unlockedLvls = new List<string> { "1-1", "1-2a", "1-2b", "1-3a", "1-3b", "1-4", "2-1", "2-2a", "2-2b", "2-3a", "2-3b", "2-4", "3-1", "3-2a", "3-2b", "3-3a", "3-3b", "3-4" };
        prevUnlockedLvls = new List<string> { "1-1", "1-2a", "1-2b", "1-3a", "1-3b", "1-4", "2-1", "2-2a", "2-2b", "2-3a", "2-3b", "2-4", "3-1", "3-2a", "3-2b", "3-3a", "3-3b", "3-4" };
        LoadScene("Menu", null);
    }
//Purge save file and FlipGameManager lvl Lists, called from MenuManager
    void ResetProgress(string name, FlipGridDefinition def) { //hijacking an event and passing useless variables >.>
        fileIO.EraseProgress();
        unlockedLvls = new List<string>();
        prevUnlockedLvls = new List<string>();
        LoadScene("Menu", null);
    }
//~~

//Creates references to instanced scripts if present in the scene
    void UpdateInstances() {
        if (FlipGrid.instance != null) grid = FlipGrid.instance;
        if (FlipMainMenuManager.instance != null) menu = FlipMainMenuManager.instance;
        if (FlipProgressionManager.GetInstance() != null) prog = FlipProgressionManager.GetInstance();

        if (grid != null) grid.SceneEndCallback += ReturnToMenu;
        if (grid != null) grid.GridSolvedCallback += AddProgress;
        if (menu != null) menu.LoadSceneCallback += LoadScene;
        if (menu != null) menu.UnlockAllCallback += UnlockAll;
        if (menu != null) menu.ResetProgressCallback += ResetProgress;
        if (menu != null) menu.QuitButtonCallback += QuitApplication;
    }
//Removes references to instanced scripts if present in the class
    void RemoveInstances() {
        if (grid != null) grid.SceneEndCallback -= ReturnToMenu;
        if (grid != null) grid.GridSolvedCallback -= AddProgress;
        if (menu != null) menu.LoadSceneCallback -= LoadScene;
        if (menu != null) menu.UnlockAllCallback -= UnlockAll;
        if (menu != null) menu.ResetProgressCallback -= ResetProgress;
        if (menu != null) menu.QuitButtonCallback -= QuitApplication;

        grid = null;
        menu = null;
        prog = null;
    }
//Goodbye world!
    public void QuitApplication(string name, FlipGridDefinition def) {
        Application.Quit();
    }
}