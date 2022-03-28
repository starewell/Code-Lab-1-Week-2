using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexSpace : MonoBehaviour {
    /// <summary>
    /// Utility class, stores all important information of individual tiles in the grid
    /// Definitions for each hex tile in a grid, updated by the FlipGrid manager
    /// </summary>

    public enum HexTile { Red, Green, Blue };
    public HexTile hexTile;
    public Vector2 coordinate;
    public Vector3 position;
    public GameObject hexObject;
    public bool occupied;


//Utility function, updates all fields of the utility class HexSpace
    public void UpdateHexSpace(HexSpace space, HexSpace.HexTile tile, Material mat, Vector2 coord, Vector3 pos, GameObject go) {
        space.hexTile = tile;
        space.coordinate = coord;
        space.position = pos;
        space.hexObject = go;
        space.hexObject.GetComponent<Renderer>().material = mat;
    }
}