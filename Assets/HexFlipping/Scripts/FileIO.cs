using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
//WEEK 4 HW; script to save and load string Lists and sort them for the GameManager
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
            saveText += lvl + '\n';
        }
        writer.Write("Levels Unlocked: " + '\n' + saveText);
        writer.Close();
    }

//Read from file, called from the game manager at Start()
    public List<string> LoadProgress() {
        StreamReader reader = new StreamReader(FILE_NAME);
        string fileContent = reader.ReadToEnd();
        reader.Close();

        char[] newLineChar = { '\n' };
        string[] fileLvls = fileContent.Split(newLineChar);

        List<string> lvls = new List<string>();
        foreach(string lvl in fileLvls) {
            lvls.Add(lvl);
        }

        return lvls;
    }

//Erase save file
    public void EraseProgress() {
        StreamWriter writer = new StreamWriter(FILE_NAME, false);
        writer.Write("");
        writer.Close();
    }
}
