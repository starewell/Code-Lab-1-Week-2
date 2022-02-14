using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressionManager : MonoBehaviour {


    [SerializeField] List<TileButton> buttons = new List<TileButton>();
    [SerializeField] List<string> unlocked = new List<string>();


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
    public void UpdateUnlocks(List<string> list) {
        if (unlocked != list) {
            foreach(string lvl in list) { 
                if (!unlocked.Contains(lvl)) StartCoroutine(UnlockNextStage(lvl));              
            }
             unlocked = list;
        }
        
    }
//Unlocks buttons in the level select screen
    IEnumerator UnlockNextStage(string lvl) {
        yield return new WaitForSeconds(0.5f);
        //foreach(string lvl in lvls) { 
            if (buttons.Find(x => x.name == lvl)) { 
                buttons.Find(x => x.name == lvl).Unlock();
            }
        unlocked.Add(lvl);
        //}    
    }

}
