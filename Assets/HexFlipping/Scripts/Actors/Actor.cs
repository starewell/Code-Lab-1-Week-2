using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour {
    /// <summary>
    /// This is an inheritable class that all 'Actors' derrive from. Actors are the player's opponents that spawn in some levels.
    /// Actors need to be able to move about and even alter the grid, as well as know when they may do so.
    /// Interfaces with the FlipGrid, LevelStateMachine and HexSpaces
    /// </summary>
    [HideInInspector]
    public FlipGrid grid;

    [SerializeField]
    float moveDuration;
    [SerializeField]
    float yOrigin;

    [HideInInspector]
    public bool takingTurn;
    [HideInInspector]
    public bool moving;
    [HideInInspector]
    bool moveStarted;

    [HideInInspector]
    public HexSpace targetSpace;
    public HexSpace currentSpace;
    
    //Gets reference to active grid, sets position below grid height
    protected virtual void Start() { 
        if (FlipGrid.instance != null) {
            grid = FlipGrid.instance;
        }
        transform.position = new Vector3(
            transform.position.x,
            yOrigin,
            transform.position.z);
        StartCoroutine(SpawnActor());
    }

    //Simple animation of Actor rising below grid
    IEnumerator SpawnActor() { 
        float time = 0;
		while(time <= moveDuration) {				
			transform.position =  new Vector3(
                transform.position.x,
                Mathf.Lerp(transform.position.y, 0, time / moveDuration), 
				transform.position.z);
			time += Time.deltaTime;
			yield return null;
		}
    }
    //Simple animation of Actor falling through grid
    public IEnumerator DespawnActor() { 
        float time = 0;
		while(time <= moveDuration) {				
			transform.position =  new Vector3(
                transform.position.x,
                Mathf.Lerp(transform.position.y, yOrigin, time / moveDuration), 
				transform.position.z);
			time += Time.deltaTime;
			yield return null;
		}
        Destroy(gameObject); //Why isn't this working
    }

    public virtual void TakeTurn() { } //Inherited function to be overwritten

    
    //Private trigger of the following coroutine, only accessed by derivatives
    protected virtual void TargetSpace(HexSpace newSpace) {
        currentSpace.occupied = false;
        targetSpace = newSpace;
        targetSpace.occupied = true;
        moving = true;
    }

    //I had this function as a coroutine but couldn't figure out how to override it including base.MoveToSpace() in it's derivatives... isn't working properly as a result
    //Lerp actor position to target position, should also signal when move is complete but does not
    protected virtual void Update() {
        if (moving)
            MoveToSpace();
    }
    protected virtual void MoveToSpace() {		
		transform.position =  new Vector3(
            Mathf.Lerp(transform.position.x, targetSpace.position.x, .05f), 
			0,
            Mathf.Lerp(transform.position.z, targetSpace.position.z, .05f));	
		
        if (Vector3.Distance(transform.position, targetSpace.position) < .01f) {
            transform.position = targetSpace.position;
            currentSpace = targetSpace;
            //Debug.Log("Finished Moving");
            moving = false;
        }     
    }

    //Borrowed logic from the FlipGrid class, identifies adjacent coordinates, finds HexSpaces w/ those coords, returns list of available spaces (not occupied)
    protected virtual List<HexSpace> FindAdjacentSpaces() {
        List<HexSpace> adjacentSpaces = new List<HexSpace>();

        Vector2 originCoord = currentSpace.coordinate;
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
        foreach(Vector2 coord in adjacentCoord) {
            if (grid.hexGridContents.Find(space => space.coordinate == coord) != null) {
                if (!grid.hexGridContents.Find(space => space.coordinate == coord).occupied)
                    adjacentSpaces.Add(grid.hexGridContents.Find(space => space.coordinate == coord));
            }
        }

        return adjacentSpaces; 
    }
}
