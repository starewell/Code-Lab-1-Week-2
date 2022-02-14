using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileIO : MonoBehaviour {

    const string FILE_NAME = "HexFlipSave.txt";

    //Week 2 singleton pattern
    private static FileIO instance;
    public static FileIO GetInstance() {
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
//Write to file, called from game manager
    public void SaveProgress(List<string> lvls) {
        StreamWriter writer = new StreamWriter(FILE_NAME, false);
        string saveText = "";
        foreach (string lvl in lvls) {
            saveText += lvl + ", ";
        }
        writer.Write("Levels Unlocked: " + saveText);
        writer.Close();
    }
}
