using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
//WEEK 4 HW; script to save and load string Lists and sort them for the FlipGameManager
public class FlipFileIO : MonoBehaviour {

    const string FILE_NAME = "HexFlipSave.txt";

//Week 2 singleton pattern
    private static FlipFileIO instance;
    public static FlipFileIO GetInstance() {
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

    public void UnlockAll() {
        StreamWriter writer = new StreamWriter(FILE_NAME, false);
        writer.Write("1-1\n1-2a\n1-2b\n1-3a\n1-3b\n1-4\n2-1\n2-2a\n2-2b\n2-3a\n2-3b\n2-4\n3-1\n3-2a\n3-2b\n3-3a\n3-3b\n3-4");
        writer.Close();
    }
}
