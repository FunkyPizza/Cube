using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class GameMode : MonoBehaviour {

    [Header("Script Defaults")]
    public GameObject Grid; //Reference to the grid object.

    [Header("Live feedback")]
    public GameObject[] LockedTiles; //Array of all the tiles that have been walked on.
    public int TilesToLock; //Quantity of tiles to lock
    public int TileLocked; //How many tiles have been locked
    public int UnitsFinished; //How many units have walked on the finishing tile
    public int UnitsToFinish; //Quantity of units that are on the board
    public int CurrentLevel; //Current Level
    public string NextLevelName; //NameOfNextLevel


    void Start() // Use this for initialization
    {
        if (!Grid) { Debug.Log("Please set the grid reference on the camera object"); }
        else
        {
            LockedTiles = new GameObject[Grid.GetComponent<GridMaker>().CubeArray.Length];

            TilesToLock = Grid.GetComponent<GridMaker>().CubeArray.Length - Grid.GetComponent<GridMaker>().ToDelete.Length - 1; //-1 accounts for the finishing tile
            UnitsToFinish = Grid.GetComponent<GridMaker>().StartTiles.Length;

            LockStartTiles();

            CurrentLevel = int.Parse(Regex.Replace(SceneManager.GetActiveScene().name, @"\D", ""));
            NextLevelName = "Level" + (CurrentLevel + 1);
        }
    }

    public void LockTile(int TileID) //Locks a tile of the ID TileID
    {
        LockedTiles[TileID] = Grid.GetComponent<GridMaker>().SpawnedTileList[TileID];
        LockedTiles[TileID].GetComponent<Tile_Interactions>().LockTile();

        TileLocked++;
    }

    public void CheckStatus()
    {
        UnitsFinished++;
        
        if (UnitsFinished >= Grid.GetComponent<GridMaker>().StartTiles.Length)
        {
            if (TileLocked >= TilesToLock)
            {
                Debug.Log("You win! Restarting");
                SceneManager.LoadScene(NextLevelName) ;
            }
            else
            {
                Debug.Log("Not all tiles have been activated! Restarting ...");
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            }
        }
        else
        {

            Debug.Log("Some units are still on the board, level not finished.");
        }
    }

    void LockStartTiles() //Locks start tiles, as unit spawn on them
    {
        foreach (int StartTileID in Grid.GetComponent<GridMaker>().StartTiles)
        {
            LockTile(StartTileID);
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
