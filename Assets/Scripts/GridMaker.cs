using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;


public enum B_Jump_Direction { Up, Down, Left, Right};

[System.Serializable] public struct B_Jump { public int TileID; public B_Jump_Direction Direction; }


[ExecuteInEditMode] public class GridMaker : MonoBehaviour
{
    [Header("Script settings")]
    public bool EnableLog; //Enable logging of the different steps of the grid building
    public bool ShowTileID; //Indicates wether or not Tile ID should show
    public bool EditMode; //In edit mode tile will show

    [Header("Default settings")]
    public GameObject Cube; //GameObject that will be spawned as a tile
    public GameObject Unit; //GameObject that will be spawned as an unit
    public GameObject Player; //GameObject that contains the GameM
    public float XOffset; //X offset from the last tile
    public float YOffset;//Y offset from the last row of tiles

    [Header("Grid Customisation")]
    public int XSize; //How many tiles on the X axis
    public int YSize; //How many tiles on the Y axis

    public int[] ToDelete; //Array of Tile ID to delete
    public int[] StartTiles; //Tile ID of the starting tiles for units
    public List<int> B_DoubleWalk; //Bonus tiles: Double Walk lets the player walk twice on the specific tile
 
    public B_Jump[] B_JumpArray;

    public int EndTile; //Tile ID of the end tile

    [HideInInspector]
    public List<GameObject> SpawnedTileList; //List made from CubeArray[] for public access
    [HideInInspector]
    public GameObject[] CubeArray; //Array of all the spawned tiles
    [HideInInspector]
    public GameObject[] UnitArray; //Array of all the spawned units

    Vector3[] CubePositions; //Array of all the tile positions
    Quaternion TempRotation; //Rotation of the gameobject before spawning
    private bool PositionsCalculated; //Indicates if the positions were calculated
    private bool TilesSpawned; //Indicates if the tiles were spawned 

    void Start()
    {

        RefreshGrid();
    }

    public void RefreshGrid() //Initiate calculation followed by spawning
    {
        //Destroy previously spawned tiles
        foreach (GameObject Cube in CubeArray)
        {
            if (Cube)
            {
                DestroyImmediate(Cube);
            }
        }

        foreach (Transform Unit in gameObject.GetComponentsInChildren<Transform>())
        {
            if (Unit.name.Contains("Unit"))
            {
                DestroyImmediate(Unit.gameObject);
            }
        }

        //Rotate the parent to 0,0,0 (only for spawning, will be rotated back after spawn)
        TempRotation = transform.rotation;
        transform.rotation = new Quaternion(0, 0, 0, 0);

        //Reset variables
        PositionsCalculated = false;
        TilesSpawned = false;

            UnitArray = new GameObject[StartTiles.Length];
            CubePositions = new Vector3[XSize * YSize];
            CubeArray = new GameObject[XSize * YSize];

            CalculatePositions();


	}
	
    void CalculatePositions() //Calculates the position of all tiles based on the settings input
    {
        if (!PositionsCalculated)
        {
            int TileNo = XSize * YSize - 1;

            int CurrentX = 0;
            int CurrentY = 0;
            int CurrentTile = 0;

            if (EnableLog) { Debug.Log("Position calculations starting for " + XSize * YSize + " tiles ..."); }

            while (CurrentTile <= TileNo)
            {
                //Calculate new offset position from Current variables
                Vector3 TileOffset = new Vector3(CurrentX * XOffset, 0, CurrentY * YOffset);


                //Setting the position in the array position
                if (CurrentTile == 0)
                {
                    CubePositions[CurrentTile] = new Vector3(0, 0, 0);
                }

                else
                {
                    CubePositions[CurrentTile] = TileOffset;
                }


                //Set Current variables for next tile and check for last tile (in which case we should start spawning tiles)
                CurrentTile++;
                if (CurrentTile > TileNo)
                {
                    StopCalculatingAndStartSpawning();
                }

                CurrentX++;

                if (CurrentX >= XSize)
                {
                    CurrentX = 0;
                    CurrentY++;
                    if (CurrentY > YSize)
                    {
                        StopCalculatingAndStartSpawning();
                    }
                }
            }
        }


        else
        {
            StopCalculatingAndStartSpawning();
        }
    }

    void StopCalculatingAndStartSpawning() //Called when all positions are calculated and tiles can be spawned
    {
        if (EnableLog) { Debug.Log("Position calculations done, starting spawing tiles."); }

        PositionsCalculated = true;
        SpawnFromPositions();

        
    }

    void SpawnFromPositions() //Spawn all tiles on positions in CubePositions[]
    {
        if (PositionsCalculated)
        {
            if (!TilesSpawned)
            {

                if (EnableLog) { Debug.Log("Starting spawning cubes ..."); }
                int CurrentTileSpawned = 0;

                foreach (Vector3 TilePosition in CubePositions)
                {
                    //Spawns the tile, sets its parent and its name
                    GameObject SpawnedTile = Instantiate(Cube, gameObject.transform.localPosition + TilePosition, gameObject.transform.rotation);
                    SpawnedTile.transform.SetParent(gameObject.transform);
                    SpawnedTile.gameObject.name = "Tile " + CurrentTileSpawned;

                    //Check if tile is a Bonus Tile and if so set the tile as bonus tile
                    //Double Walk Bonus
                    if (B_DoubleWalk.Count > 0)
                    {
                        if (B_DoubleWalk.Contains(CurrentTileSpawned))
                        {
                            SpawnedTile.GetComponent<Tile_Interactions>().SetTileBonus(TileBonus.DoubleWalk);
                        }
                    }

                    //Jump bonus
                    if (B_JumpArray.Length > 0)
                    {
                        B_Jump TempBJump = CheckIfTileIsJumpBonus(CurrentTileSpawned);

                        if (B_JumpArray.Contains<B_Jump>(TempBJump))
                        {
                            SpawnedTile.GetComponent<Tile_Interactions>().SetTileBonus(TileBonus.Jump);

                            //TempBJump.Direction
                            switch (B_JumpArray[System.Array.IndexOf(B_JumpArray, TempBJump)].Direction)
                            {
                                case B_Jump_Direction.Up:
                                    if (CurrentTileSpawned + 2 >= 0 && CurrentTileSpawned + 2 <= XSize * YSize && !ToDelete.Contains<int>(CurrentTileSpawned + 2))
                                    {
                                        SpawnedTile.GetComponent<Tile_Interactions>().BJumpToTileID = CurrentTileSpawned + 2;
                                        SpawnedTile.transform.Find("BonusSprite").transform.rotation *= new Quaternion(0, 1, 0, 0);
                                    }
                                    break;

                                case B_Jump_Direction.Down:
                                    if (CurrentTileSpawned - 2 >= 0 && CurrentTileSpawned - 2 <= XSize * YSize && !ToDelete.Contains<int>(CurrentTileSpawned - 2))
                                    {
                                        SpawnedTile.GetComponent<Tile_Interactions>().BJumpToTileID = CurrentTileSpawned - 2;
                                        SpawnedTile.transform.Find("BonusSprite").transform.rotation *= new Quaternion(0, 0, 0, 1);
                                    }
                                    break;

                                case B_Jump_Direction.Left:
                                    if (CurrentTileSpawned + 2*YSize >= 0 && CurrentTileSpawned + 2*YSize <= XSize * YSize && !ToDelete.Contains<int>(CurrentTileSpawned + 2*YSize))
                                    {
                                        SpawnedTile.transform.Find("BonusSprite").transform.rotation *= new Quaternion(0, 0, 0.7071f, -0.7071f);
                                        SpawnedTile.GetComponent<Tile_Interactions>().BJumpToTileID = CurrentTileSpawned + 2*YSize;
                                    }
                                    break;

                                case B_Jump_Direction.Right:
                                    if (CurrentTileSpawned - 2*YSize >= 0 && CurrentTileSpawned - 2*YSize <= XSize * YSize && !ToDelete.Contains<int>(CurrentTileSpawned - 2*YSize))
                                    {
                                        SpawnedTile.GetComponent<Tile_Interactions>().BJumpToTileID = CurrentTileSpawned - 2*YSize;
                                        SpawnedTile.transform.Find("BonusSprite").transform.rotation *= new Quaternion(0, 0, 0.7071f, 0.7071f);
                                    }
                                    break;
                            }
                        }
                    }

                    //Sets falling animation time
                    SpawnedTile.GetComponent<Animation_MoveInDirection>().StartDelay = (Mathf.Round(CurrentTileSpawned) + 0.01f) / 10;
                    SpawnedTile.GetComponent<Animation_MoveInDirection>().AnimationSpeedInS = 0.8f;
                    SpawnedTile.GetComponent<Animation_MoveInDirection>().KeepOriginalPosition = true;
                    SpawnedTile.GetComponent<Animation_MoveInDirection>().KeepOriginalScale = true;
                    SpawnedTile.GetComponent<Animation_MoveInDirection>().ScaleAnimation = Animation_MoveInDirection.ScaleAnim.Scale_Up;
                    SpawnedTile.GetComponent<Animation_MoveInDirection>().ActivateAnimation();

                    //Displays or not the TileID in 3D world
                    if (ShowTileID) { SpawnedTile.transform.GetComponentInChildren<TextMesh>().text = CurrentTileSpawned.ToString(); }
                    else { SpawnedTile.transform.GetComponentInChildren<TextMesh>().text =" " ; }

                    //Hides the tile if EditMode is false
                    if (!EditMode) { SpawnedTile.GetComponent<MeshRenderer>().enabled = false; SpawnedTile.GetComponentInChildren<SpriteRenderer>().enabled = false; }

                    CubeArray[CurrentTileSpawned] = SpawnedTile;

                    //Sets variable for next tile and check if last tile was spawned
                    CurrentTileSpawned++;

                    //Finalise cube spawning
                    if (CurrentTileSpawned >= CubePositions.Length)
                    {
                        transform.rotation = TempRotation;
                        SpawnedTileList = new List<GameObject>(CubeArray);

                        DeleteSelectedTiles();
                        SetEndTile();
                        SpawnUnits();

                        if (EnableLog) { Debug.Log("Finished spawning cubes."); }

                    }

                }
            }
            else
            {
                if (EnableLog) { Debug.Log("Tiles are already spawned."); }
            }
        }

        else
        {
            if (EnableLog) { Debug.Log("Positions seems not be calculated (PositionsCalculated=false)."); }
        }
    }

    B_Jump CheckIfTileIsJumpBonus(int Tile) //Check if the TileID is a Bonus Jump Tile
    {
        B_Jump TempSE0 = new B_Jump();
        TempSE0.TileID = Tile;
        TempSE0.Direction = B_Jump_Direction.Up;

        if (B_JumpArray.Contains<B_Jump>(TempSE0))
        {
            return TempSE0;
        }

        else {
            TempSE0.Direction = B_Jump_Direction.Down;
            if (B_JumpArray.Contains<B_Jump>(TempSE0))
            {
                return TempSE0;
            }

            else {
                TempSE0.Direction = B_Jump_Direction.Left;
                if (B_JumpArray.Contains<B_Jump>(TempSE0))
                {
                    return TempSE0;
                }

                else {
                    TempSE0.Direction = B_Jump_Direction.Right;
                    if (B_JumpArray.Contains<B_Jump>(TempSE0))
                    {
                        return TempSE0;
                    }
                }
            }
        }
        return TempSE0;;
    } 

    void DeleteSelectedTiles() //Delete tiles in the ToDelete array
    {
        foreach (int TileNo in ToDelete)
        {
            CubeArray[TileNo].GetComponent<Renderer>().enabled=false;
            CubeArray[TileNo].GetComponent <BoxCollider>().enabled=false;
            if (EnableLog) { Debug.Log("Destroyed tile no" + TileNo); }

            DestroyImmediate(CubeArray[TileNo]);
        }
    }

    void SetEndTile()
    {
        SpawnedTileList[EndTile].GetComponent<Tile_Interactions>().SetAsEndTile();
    }

    void SpawnUnits()
    {
        foreach (int TileID in StartTiles)
        {
            if (SpawnedTileList[TileID])
            {
                int CurrentUnitSpawnedNo = 0;

                GameObject UnitSpawned = Instantiate(Unit, SpawnedTileList[TileID].transform.position, SpawnedTileList[TileID].transform.rotation, gameObject.transform); 
               
                UnitArray[CurrentUnitSpawnedNo] = UnitSpawned;
                CubeArray[TileID].GetComponent<Tile_Interactions>().SetAsStartTile();
                CurrentUnitSpawnedNo++;
                
                UnitSpawned.GetComponent<Unit_Actions>().Grid = gameObject ;
                UnitSpawned.GetComponent<Unit_Actions>().StartingTile = TileID;

                UnitSpawned.GetComponent<Unit_Actions>().SpawnUnitOnTile(TileID);

                UnitSpawned.GetComponent<Animation_MoveInDirection>().ActivateAnimation();

                //Hides the tile if EditMode is false
                if (!EditMode) { UnitSpawned.GetComponent<MeshRenderer>().enabled = false; }


                if (EnableLog) { Debug.Log("Unit "+ TileID + " Spawning"); }
            }
            else
            {
                if (EnableLog) { Debug.Log("Not tiles found to spawn units"); }
            }
        }
    }
}
