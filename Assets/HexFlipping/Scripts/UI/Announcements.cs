using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Quick class to trigger UI callouts through events
public class Announcements : MonoBehaviour {

    FlipGrid grid;
    LevelUI totals;

    public GameObject[] barkPanels; //BarkPanel prefabs, different announcments

    void Start() {
        grid = FlipGrid.instance;
        grid.GridGeneratedCallback += OnGridGenerated;
        totals = LevelUI.instance;
        totals.OnWinCallback += OnWin;
        //totals.OnLoseCallback += OnLose;
        foreach (GameObject go in barkPanels) {
            go.SetActive(true);
        }
    }

    void OnGridGenerated(float r, float g, float b) {
        StartCoroutine(Bark(0));    
    }

    void OnWin() {
        StartCoroutine(Bark(1));
    }

    void OnLose() {
        StartCoroutine(Bark(2));
    }
    
    //play bark animation at index
    public IEnumerator Bark(int index) {
        barkPanels[index].GetComponent<Animator>().SetBool("Bark", true);
        yield return new WaitForSeconds(1);
        barkPanels[index].GetComponent<Animator>().SetBool("Bark", false);
    }

}
