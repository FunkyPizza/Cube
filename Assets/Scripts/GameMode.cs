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
    public int CurrentLevel; //Current Level ID
    public int NextLevel; //Next Level ID
    public string CurrentLevelSize; //Current grid size

    string LevelToOpen; //Level name to open when calling OpenLevel()


    void Start() 
    {
        Application.targetFrameRate = 60;

        if (!Grid) { Debug.Log("Please set the grid reference on the camera object"); }
        else
        {
            //Create an empty array to keep track of which cube has been locked.
            LockedTiles = new GameObject[Grid.GetComponent<GridMaker>().CubeArray.Length];

            //Get how many tiles there are to lock & how many units that have to reach the end tile.
            TilesToLock = Grid.GetComponent<GridMaker>().CubeArray.Length - Grid.GetComponent<GridMaker>().ToDelete.Length - 1; //-1 accounts for the finishing tile
            UnitsToFinish = Grid.GetComponent<GridMaker>().StartTiles.Length;

            //Get current & Next level ID
            if (SceneManager.GetActiveScene().name != "MainMenu")
            {
                // int.Parse(Regex.Replace(SceneManager.GetActiveScene().name, @"\D", "")), get int from scene name
                CurrentLevel = int.Parse( SceneManager.GetActiveScene().name.ToString().Substring(4));
                CurrentLevelSize = SceneManager.GetActiveScene().name.ToString().Substring(0,3);
            }
            else { CurrentLevel = 1; }

            NextLevel = CurrentLevel + 1;
            LockStartTiles();
        }
    }

    public void LockTile(int TileID) //Locks a tile of the ID TileID, meaning a unit walked on it.
    {
        LockedTiles[TileID] = Grid.GetComponent<GridMaker>().SpawnedTileList[TileID];
        LockedTiles[TileID].GetComponent<Tile_Interactions>().LockTile();

        TileLocked++;
    }

    public void CheckStatus() //Triggered whenever a unit reaches the end tile, checks if player won.
    {
        UnitsFinished++;
        
        if (UnitsFinished >= Grid.GetComponent<GridMaker>().StartTiles.Length)
        {
            if (TileLocked >= TilesToLock)
            {
                Debug.Log("You win!");
                OpenLevelDelay(NextLevel);
            }
            else
            {
                Debug.Log("Not all tiles have been activated! Restarting ...");
                RestartLevel();

            }
        }
        else
        {

            Debug.Log("Some units are still on the board, level not finished.");
        }
    }

    void LockStartTiles() //Locks the tiles where a unit has spawned.
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

    public void OpenLevelDelay(int ID)
    {
        foreach (LevelLib i in GetComponent<LevelData>().LevelsInfo)
        {
            if (i.GridSize == CurrentLevelSize)
            {
                if(ID <= i.QtyOfLevels)
                {
                    LevelToOpen = CurrentLevelSize + "L" + ID;
                    Invoke("OpenLevel", 1);
                }
                else { Invoke("OpenMainMenu", 1); }
            }
        }

    }

    public void OpenFirstLevel()
    {
        SceneManager.LoadScene("3x3L1");
    }

    void OpenLevel()
    {
        Debug.Log("Trying to open" + LevelToOpen);

        SceneManager.LoadScene(LevelToOpen);

    }

    public void OpenMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
