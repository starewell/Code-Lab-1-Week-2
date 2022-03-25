using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Rehoused a lot of functions from the TileGenerator class, this class stores HexSpaces and updates them
[RequireComponent(typeof(FlipTileGenerator))]
public class FlipGrid : MonoBehaviour {

    public FlipGridDefinition gridDef; //We'll find a more clever way to pass this in the future

//Whole lotta events for when anything about the grid changes
    public delegate void OnGridChange(float r, float g, float b);
    public event OnGridChange GridCalculatedCallback;
    public event OnGridChange GridGeneratedCallback;
    public event OnGridChange TotalsChangeCallback;
    public event OnGridChange LevelEndCallback;

    public delegate void OnGridCondition(string[] lvl);
    public event OnGridCondition GridSolvedCallback;
//
    public List<HexSpace> hexGridContents = new List<HexSpace>();  //Stored list of all tiles in grid; HexSpaces

    FlipTileGenerator generator;

//My singleton which does not completely remove human error, but is it egregious? 
    public static FlipGrid instance;
    void Awake() {
        if (instance != null) {
            Debug.Log("More than one instance of FlipGrid found!");
            return;
        }
        instance = this;
    }
//
    private void Start() {
        generator = GetComponent<FlipTileGenerator>();
    }
//Trigger generator, invoke TotalsDisplay to update
    public void GenerateGrid() {
        GridCalculatedCallback?.Invoke(gridDef.goalPercentRed, gridDef.goalPercentGreen, gridDef.goalPercentBlue);
        generator.StartGridGeneration(gridDef);

    }
//
//Unlock grid and bark if grid is a level
    public void GridGenerated() { 
        if(gridDef.level) {
            GridGeneratedCallback?.Invoke(0, 0, 0);
        }
        UnlockHexGrid();
    }
//
//Trigger generator, lock grid
    public void DegenerateGrid(bool win) {
        LockHexGrid();
        generator.StartGridDegeneration(gridDef);

        if (win) {
            string[] lvls = new string[gridDef.unlocks.Length];
            for (int i = 0; i < gridDef.unlocks.Length; i++) {
                lvls[i] = gridDef.unlocks[i];
            }
            GridSolvedCallback?.Invoke(lvls);
        }
    }
//
//Trigger FlipGameManager from generator call
    public void GridDegenerated() { 
        if(gridDef.level) {
            LevelEndCallback?.Invoke(0, 0, 0);
        }
    }
//
//Append new HexSpace to list, subscribe for updates
    public void AddHexSpace(HexSpace space) {
        hexGridContents.Add(space);

        space.GetComponent<TileFlip>().FlipCallback += FlipHexSpace;
        space.GetComponent<TileFlip>().OriginCallback += UpdateAdjacent;
    }
//
//Updates HexSpace to flipped values when called from TileFlip.Interact() or from being flipped adjacently
    void FlipHexSpace(HexSpace space) {
    	int index = (int)space.hexTile + 1;
    	if (index > System.Enum.GetValues(typeof(HexSpace.HexTile)).Length - 1) index = 0;

    	space.hexObject.GetComponent<Renderer>().material = gridDef.colors[index];
    	space.hexTile = (HexSpace.HexTile)index;
        space.hexObject.name = space.hexTile.ToString() + "Tile(" + space.coordinate.x + ", " + space.coordinate.y + ")";

        UpdateHexGrid();
    }

    IEnumerator FlipAdjacentHexes(Vector2[] adjacentCoord) { 
        foreach (Vector2 coord in adjacentCoord) {
            if (hexGridContents.Find(space => space.coordinate == coord) != null) { //Found this constructor thru microsoft docs, have never used it before
                HexSpace adjSpace = hexGridContents.Find(space => space.coordinate == coord);
                StartCoroutine(adjSpace.GetComponent<TileFlip>().FlipTile(false));
                yield return new WaitForSeconds(.1f);
            }
        }
    }

//
//Function is executed through delegate event from the TileFlip class, calculates adjacent hex spaces and exectues TileFlip.FlipTile();
    void UpdateAdjacent(HexSpace space) {

        Vector2 originCoord = space.coordinate;
        Vector2[] adjacentCoord;
        if (originCoord.y % 2 == 0) { //Even rows
            adjacentCoord = new Vector2[] {
            new Vector2(originCoord.x, originCoord.y-1),
            new Vector2(originCoord.x-1, originCoord.y),
            new Vector2(originCoord.x, originCoord.y+1),
            new Vector2(originCoord.x+1, originCoord.y-1),
            new Vector2(originCoord.x+1, originCoord.y+1),
            new Vector2(originCoord.x+1, originCoord.y)};
        }
        else { //Odd rows
            adjacentCoord = new Vector2[] {
            new Vector2(originCoord.x-1, originCoord.y-1),
            new Vector2(originCoord.x-1, originCoord.y),
            new Vector2(originCoord.x-1, originCoord.y+1),
            new Vector2(originCoord.x, originCoord.y-1),
            new Vector2(originCoord.x, originCoord.y+1),
            new Vector2(originCoord.x+1, originCoord.y)};
        }
        StartCoroutine(FlipAdjacentHexes(adjacentCoord));
    }
//
//Useless middleman, maybe one day!
    public void UpdateHexGrid() {


        UpdateHexGridTotals();
    }
                   
//Temporarily store grid totals, call for UI to refresh after tiles flip
    void UpdateHexGridTotals() {
        int amntRed = 0, amntGreen = 0, amntBlue = 0;
        foreach (HexSpace space in hexGridContents) {
            if (space.hexTile == HexSpace.HexTile.Red) amntRed++;
            else if (space.hexTile == HexSpace.HexTile.Green) amntGreen++;
            else if (space.hexTile == HexSpace.HexTile.Blue) amntBlue++;
        }
        TotalsChangeCallback?.Invoke(amntRed, amntGreen, amntBlue);
    }
//
//Changes base class interactable's active bool 
    public void UnlockHexGrid() {
        foreach(HexSpace space in hexGridContents) {
            space.GetComponent<TileFlip>().active = true;
                
        }
    }
    public void LockHexGrid() {
        foreach (HexSpace space in hexGridContents) {
            space.GetComponent<TileFlip>().active = false;
        }           
    }
//
}

