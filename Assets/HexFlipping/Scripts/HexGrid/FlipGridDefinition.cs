using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Scriptable object which determines the attributes of every level loaded into the TileGenerator scene
//Create levels by creating GridDefinitions in the assets folder and pass it through FlipGameManager's LoadScene function
[CreateAssetMenu(fileName = "New Grid", menuName = "Hex Grid")]
public class FlipGridDefinition : ScriptableObject {

    //Definitions for the grid generation reference
    public bool level;
    public string[] unlocks;

    public GameObject[] prefabs;
    public Material[] colors;
    public Vector2 gridDim; //Odd y values generate rounded grid shapes
    public float tileSize;
    //% of grid for each tile type, procgen variables and game manager
    [Range(0, 1)]
    public float spawnPercentRed;
    [Range(0, 1)]
    public float spawnPercentGreen;
    [Range(0, 1)]
    public float spawnPercentBlue;
    [Range(0, 1)]
    public float goalPercentRed;
    [Range(0, 1)]
    public float goalPercentGreen;
    [Range(0, 1)]
    public float goalPercentBlue;
    //


}
