using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Unit_Actions : MonoBehaviour
{

    [Header("Script settings")]
    public bool EnableLog; //Enables logging of the raycast process

    [Header("Unit status")]
    public GameObject Grid; //Reference to the grid
    public int CurrentTileID; //What tile id this unit is on
    public int LastTileID; //ID the tile this unit was on before the current one
    public List<GameObject> SurroundingTiles; //Surrounding tiles
    public int StartingTile; //Tile where the unit should spawn
    public bool UnitActive; //Indicates whether or not this unit can be moved again

    Vector3 OriginalScale; //Original scale of this gameObject
    bool IsSelected; //Indicates wether or not the unit is selected
    float YOffset = 1f; //Up offset above the grid
    bool IsInEditor = true; //Indicates wether the code is running in Editor or Runtime (Used to spawn units without animation)

    //Movement lerp variables
    bool Movement_MoveTrigger; //Triggers the MoveToPosition coroutine
    float Movement_Speed = 0.2f; //How long in S is the movement taking
    Vector3 Movement_NewPosition; //Position to set before calling MoveToPosition
    Vector3 Movement_OldPosition; //Start postion to lerp from
    Quaternion Movement_OldRotation; //Start rotation to lerp from
    Quaternion Movement_NewRotation; //Rotation to lerp to
    int Movement_DirectionToRollTo; //Switches direction for the rotation lerp 0:Up, 1:Down, 2:Right, 3:Left
    int[] Movement_TilesInRange = new int[4]; //Set on CheckSurroundingTiles and indicates which direction to roll to
    bool Movement_AnimtionInitialised; //Indicates wether or not the lerp values have already been set
    float Movement_TempTime; //Temp value of time for the lerp
    float Movement_LerpValue; //Temp alpha value used to lerp

    //Death lerp variables
    bool Death_DeathTrigger;
    float Death_Speed = 0.2f;
    bool Death_AnimationInitialised;
    float Death_LerpValue;
    float Death_TempTime;
    Vector3 Death_OldScale;
    Vector3 Death_OldPosition;
    bool IsOnBonusJumpTile;


    void Start()  // Use this for initialization
    {
        OriginalScale = transform.localScale;

        IsInEditor = false;
    }

    void Update() // Update is called once per frame 
    {
        if (Movement_MoveTrigger)
        {
            StartCoroutine("MoveToPosition");
        }

        if (!Movement_MoveTrigger && Death_DeathTrigger)
        {
            StartCoroutine("DeathAnimation");
        }

    }

    public void AssignUnitToTileNo(int TileNo) //Assigns this unit to tile and moves it there
    {
        if (Grid.GetComponent<GridMaker>().SpawnedTileList[CurrentTileID])
        {
            Grid.GetComponent<GridMaker>().SpawnedTileList[CurrentTileID].GetComponent<Tile_Interactions>().Unit = null;
        }


        LastTileID = CurrentTileID;
        CurrentTileID = TileNo;


        if (Grid.GetComponent<GridMaker>().SpawnedTileList[LastTileID])
        {
            if (Grid.GetComponent<GridMaker>().SpawnedTileList[LastTileID].GetComponent<Tile_Interactions>().BWalksLeft <= 0)
            {
                Grid.GetComponent<GridMaker>().Player.GetComponent<GameMode>().LockTile(LastTileID);
                Grid.GetComponent<GridMaker>().SpawnedTileList[LastTileID].GetComponent<Animation_MoveInDirection>().TileDeath();
            }
            else
            {
                Grid.GetComponent<GridMaker>().SpawnedTileList[LastTileID].GetComponent<Tile_Interactions>().HardReset();
                Grid.GetComponent<GridMaker>().SpawnedTileList[LastTileID].GetComponent<Tile_Interactions>().Bonus_Walk();
            }
        }

        Grid.GetComponent<GridMaker>().SpawnedTileList[TileNo].GetComponent<Tile_Interactions>().Unit = gameObject;

            if (IsInEditor)
        {
            transform.position = Grid.GetComponent<GridMaker>().SpawnedTileList[TileNo].transform.position + new Vector3(0, YOffset, 0);
        }
        else
        {
            Movement_DirectionToRollTo = Array.IndexOf(Movement_TilesInRange, CurrentTileID);
            Movement_MoveTrigger = true;
            Movement_NewPosition = Grid.GetComponent<GridMaker>().SpawnedTileList[TileNo].transform.position + new Vector3(0, YOffset, 0);

            MoveToPosition();
        }


        if (Grid.GetComponent<GridMaker>().SpawnedTileList[TileNo].GetComponent<Tile_Interactions>().IsBJump)
        {
            IsOnBonusJumpTile = true;
        }


        if (Grid.GetComponent<GridMaker>().SpawnedTileList[TileNo].GetComponent<Tile_Interactions>().IsEndTile) //If unit walks on a end tile
        {
            Grid.GetComponent<GridMaker>().Player.GetComponent<Camera_Raycaster>().Deselect();
            Death();
        }

    }

    public void SpawnUnitOnTile(int TileNo) //Assigns this unit to tile and moves it there
    {

        LastTileID = CurrentTileID;
        CurrentTileID = TileNo;

        Grid.GetComponent<GridMaker>().SpawnedTileList[TileNo].GetComponent<Tile_Interactions>().Unit = gameObject;

        if (IsInEditor)
        {
            transform.position = Grid.GetComponent<GridMaker>().SpawnedTileList[TileNo].transform.position + new Vector3(0, YOffset, 0);
        }
        else
        {
            Movement_DirectionToRollTo = Array.IndexOf(Movement_TilesInRange, CurrentTileID);
            Movement_MoveTrigger = true;
            Movement_NewPosition = Grid.GetComponent<GridMaker>().SpawnedTileList[TileNo].transform.position + new Vector3(0, YOffset, 0);

            MoveToPosition();
        }


    }

    public void Select() //Selects this unit
    {
        if (!IsSelected)
        {
            transform.localScale *= 1.5f;
            IsSelected = true;
            CheckSurroundingTiles();
        }
        else
        {
            if (EnableLog) { Debug.Log("Unit was already selected."); }
        }
    }

    public void UnSelect() //Unselects this unit
    {
        if (IsSelected)
        {
            transform.localScale = OriginalScale;
            IsSelected = false;

            foreach (GameObject Tile in SurroundingTiles)
            {
                Tile.GetComponent<Tile_Interactions>().HardReset();
            }
        }
        else
        {
            if (EnableLog) { Debug.Log("Unit was already unselected."); }
        }
    }

    void CheckSurroundingTiles()
    {
        //Right tile (-)
        if (CurrentTileID - Grid.GetComponent<GridMaker>().XSize >= 0) //Index check (is index bigger than 0)
        {

            Movement_TilesInRange[2] = 4;

            if (Grid.GetComponent<GridMaker>().SpawnedTileList[CurrentTileID - Grid.GetComponent<GridMaker>().XSize] != null) //Does the tile exist
            {
                if (Grid.GetComponent<GridMaker>().SpawnedTileList[CurrentTileID - Grid.GetComponent<GridMaker>().XSize].GetComponent<Tile_Interactions>().Unit == null) //Is there a unit on that tile
                {
                    Grid.GetComponent<GridMaker>().SpawnedTileList[CurrentTileID - Grid.GetComponent<GridMaker>().XSize].GetComponent<Tile_Interactions>().ReadyToMove();
                    SurroundingTiles.Add(Grid.GetComponent<GridMaker>().SpawnedTileList[CurrentTileID - Grid.GetComponent<GridMaker>().XSize]);

                    Movement_TilesInRange[2] = CurrentTileID - Grid.GetComponent<GridMaker>().XSize;
                }

            }

        }

        //Down tile (-)
        if (CurrentTileID - 1 >= 0) //Index check (is index bigger than 0)
        {
            Movement_TilesInRange[1] = 4;

            if (CurrentTileID % Grid.GetComponent<GridMaker>().XSize != 0) //Is the tile over the edge
            {
                if (Grid.GetComponent<GridMaker>().SpawnedTileList[CurrentTileID - 1] != null) //Does the tile exist
                {
                    if (Grid.GetComponent<GridMaker>().SpawnedTileList[CurrentTileID - 1].GetComponent<Tile_Interactions>().Unit == null) //Is there a unit on that tile
                    {
                        Grid.GetComponent<GridMaker>().SpawnedTileList[CurrentTileID - 1].GetComponent<Tile_Interactions>().ReadyToMove();
                        SurroundingTiles.Add(Grid.GetComponent<GridMaker>().SpawnedTileList[CurrentTileID - 1]);

                        Movement_TilesInRange[1] = CurrentTileID - 1;
                    }
                }
            }
        }

        //Up tile (+)
        if (CurrentTileID + 1 < Grid.GetComponent<GridMaker>().SpawnedTileList.ToArray().Length) //Index check (is index smaller than max index)
        {

            Movement_TilesInRange[0] = 4;

            if ((CurrentTileID + 1) % Grid.GetComponent<GridMaker>().XSize != 0) //Is the tile over the edge
            {
                if (Grid.GetComponent<GridMaker>().SpawnedTileList[CurrentTileID + 1] != null) //Does the tile exist
                {
                    if (Grid.GetComponent<GridMaker>().SpawnedTileList[CurrentTileID + 1].GetComponent<Tile_Interactions>().Unit == null) //Is there a unit on that tile
                    {
                        Grid.GetComponent<GridMaker>().SpawnedTileList[CurrentTileID + 1].GetComponent<Tile_Interactions>().ReadyToMove();
                        SurroundingTiles.Add(Grid.GetComponent<GridMaker>().SpawnedTileList[CurrentTileID + 1]);

                        Movement_TilesInRange[0] = CurrentTileID + 1;
                    }
                }
            }
        }

        //Left tile (+)
        if (CurrentTileID + Grid.GetComponent<GridMaker>().XSize < Grid.GetComponent<GridMaker>().SpawnedTileList.ToArray().Length) //Index check (is index smaller than max index)  
        {
            Movement_TilesInRange[3] = 4;

            if (Grid.GetComponent<GridMaker>().SpawnedTileList[CurrentTileID + Grid.GetComponent<GridMaker>().XSize] != null) //Does the tile exist
            {
                if (Grid.GetComponent<GridMaker>().SpawnedTileList[CurrentTileID + Grid.GetComponent<GridMaker>().XSize].GetComponent<Tile_Interactions>().Unit == null) //Is there a unit on that tile
                {
                    Grid.GetComponent<GridMaker>().SpawnedTileList[CurrentTileID + Grid.GetComponent<GridMaker>().XSize].GetComponent<Tile_Interactions>().ReadyToMove();
                    SurroundingTiles.Add(Grid.GetComponent<GridMaker>().SpawnedTileList[CurrentTileID + Grid.GetComponent<GridMaker>().XSize]);

                    Movement_TilesInRange[3] = CurrentTileID + Grid.GetComponent<GridMaker>().XSize;
                }
            }
        }

    } //Checks surrounding tile to put them "InRange"

    public void Bonus_Jump()
    {
        if (!Grid.GetComponent<GridMaker>().SpawnedTileList[Grid.GetComponent<GridMaker>().SpawnedTileList[CurrentTileID].GetComponent<Tile_Interactions>().BJumpToTileID].GetComponent<Animation_MoveInDirection>().KillTile)
        {
            int TileToJumpTo = Grid.GetComponent<GridMaker>().SpawnedTileList[CurrentTileID].GetComponent<Tile_Interactions>().BJumpToTileID;
            AssignUnitToTileNo(TileToJumpTo);
            Grid.GetComponent<GridMaker>().Player.GetComponent<Camera_Raycaster>().Select(Grid.GetComponent<GridMaker>().SpawnedTileList[TileToJumpTo].transform);
        }
    }

    IEnumerator MoveToPosition() //Coroutine to procedurally animate the unit
    {
        if (!Movement_AnimtionInitialised)
        {
            Movement_OldPosition = transform.position;
            Movement_OldRotation = transform.rotation;
            Movement_TempTime = Movement_Speed;
            Movement_LerpValue = 0;
            Movement_AnimtionInitialised = true;

            switch (Movement_DirectionToRollTo)
            {
                case 0:
                    Movement_NewRotation = transform.rotation * Quaternion.Euler(0, 0, -90);
                    break;

                case 1:
                    Movement_NewRotation = transform.rotation * Quaternion.Euler(0, 0, 90);
                    break;

                case 2:
                    Movement_NewRotation = transform.rotation * Quaternion.Euler(-90, 0, 0);
                    break;

                case 3:
                    Movement_NewRotation = transform.rotation * Quaternion.Euler(90, 0, 0);
                    break;

                case 4:
                    Movement_NewRotation = transform.rotation * Quaternion.Euler(0, 0, 0);
                    break;
            }
        }

        transform.position = Vector3.Lerp(Movement_OldPosition, Movement_NewPosition, Movement_LerpValue);
        transform.rotation = Quaternion.Lerp(Movement_OldRotation, Movement_NewRotation, Movement_LerpValue);

        Movement_TempTime -= Time.deltaTime;
        Movement_LerpValue = 1 - (Movement_TempTime / Movement_Speed);

        if (Movement_TempTime > 0)
        {
            yield return null;
        }
        else
        {
            Movement_MoveTrigger = false;
            StopCoroutine("MoveToPosition");
            Movement_AnimtionInitialised = false;
            transform.position = Movement_NewPosition;
            transform.rotation = new Quaternion(0, 0, 0, 0);

            if (IsOnBonusJumpTile)
            {
                IsOnBonusJumpTile = false;
                Bonus_Jump();
            }
        }

    }

    void Death()
    {
        Death_DeathTrigger = true;
    }

    IEnumerator DeathAnimation()
    {
        if (!Death_AnimationInitialised)
        {
            Death_TempTime = Death_Speed;
            Death_OldPosition = transform.position;
            Death_OldScale = transform.localScale;

            Death_AnimationInitialised = true;
        }

        transform.localScale = Vector3.Lerp(Death_OldScale, new Vector3(0, 0, 0), Death_LerpValue);
        transform.position = Vector3.Lerp(Death_OldPosition, new Vector3(0, 25, 0), Death_LerpValue);

        Death_TempTime -= Time.deltaTime;
        Death_LerpValue = 1 - (Death_TempTime / Death_Speed);

        if(Death_TempTime > 0)
        {
            yield return null;
        }

        else
        {
            StopCoroutine("DeathAnimation");
            Death_DeathTrigger = false;

            Grid.GetComponent<GridMaker>().Player.GetComponent<Camera_Raycaster>().Deselect();
            Grid.GetComponent<GridMaker>().Player.GetComponent<GameMode>().CheckStatus();

            Destroy(gameObject);

        }

    }
}
