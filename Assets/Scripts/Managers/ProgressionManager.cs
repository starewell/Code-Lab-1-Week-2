using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Instanced manager on the main menu which updates progression display/UI according to GameManager's saved progress
public class ProgressionManager : MonoBehaviour {

    [SerializeField] List<TileButton> buttons = new List<TileButton>();

//Week 2 Singleton Pattern
    private static ProgressionManager instance;
    public static ProgressionManager GetInstance() {
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
//Called from the Game Manager when this script is loaded in a scene
    public void UpdateUnlocks(List<string> list, List<string> prevList) {
        
        //Start scene with previously unlocked levels unlocked so they don't animate
        foreach(string lvl in prevList) {
            Debug.Log(lvl);
            if (buttons.Find(x => x.name == lvl)) { 
                buttons.Find(x => x.name == lvl).UnlockOnStart();
            }        
        }
        if (prevList != list) { //Animate newly unlocked levels
            foreach(string lvl in list) { 
                if (prevList.Find(x => x == lvl) == null) {
                    StartCoroutine(UnlockNextStage(lvl));
                }              
            }
        }
        
    }
//Unlocks buttons in the level select screen
    IEnumerator UnlockNextStage(string lvl) {
        yield return new WaitForSeconds(0.5f);
            if (buttons.Find(x => x.name == lvl)) { 
                buttons.Find(x => x.name == lvl).ChangeLockState(false);
            } 
    }

}
