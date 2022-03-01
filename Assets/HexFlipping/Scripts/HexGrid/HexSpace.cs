using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Utility class, stores all important information of individual tiles in the grid
public class HexSpace : MonoBehaviour {
//Definitions for each hex tile in a grid, updated by the TileGrid manager
    public enum HexTile { Red, Green, Blue };
    public HexTile hexTile;
    public Vector2 coordinate;
    public Vector3 position;
    public GameObject hexObject;


//Utility function, updates all fields of the utility class HexSpace
    public void UpdateHexSpace(HexSpace space, HexSpace.HexTile tile, Material mat, Vector2 coord, Vector3 pos, GameObject go) {
        space.hexTile = tile;
        space.coordinate = coord;
        space.position = pos;
        space.hexObject = go;
        space.hexObject.GetComponent<Renderer>().material = mat;
    }
//Function to update HexSpace in the fashion of the TileFlip mechanics -- oops this is in the TileGrid class, but I think I want it here?
/*
    public void FlipHexSpace(Material[] mat) {
        int index = (int)hexTile + 1;
        if (index > System.Enum.GetValues(typeof(HexSpace.HexTile)).Length - 1) index = 0;

        hexObject.GetComponent<Renderer>().material = mat[index];
        hexTile = (HexSpace.HexTile)index;
        hexObject.name = hexTile.ToString() + "Tile(" + coordinate.x + ", " + coordinate.y + ")";
    }
//
*/
}