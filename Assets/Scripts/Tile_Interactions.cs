using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public enum TileBonus { DoubleWalk, Jump };

public class Tile_Interactions : MonoBehaviour {

    [Header("Script defaults")]
    public Material ToMoveMaterial; //Material to use to indicate in range tile
    public Material StartTileMaterial; //Marerial used to indicate the tile is a start one
    public Material ToMoveOnStartTile; //Material of the the start tile, when in range of a unit
    public Material EndTileMaterial; //Material to use to indicate the finishing tile
    public Material ToMoveOnEndTile; //Material of the the start tile, when in range of a unit
    public Sprite Bonus_WalkSprite; //Sprite to use on bonus tiles Double Walk
    public Sprite Bonus_JumpSprite; //Sprite to use on bonus tiles Jump

    [Header("Live feedback")]
    public GameObject Unit; //Unit currently present on this specific tile
    public int TileID; //Id of this specific tile
    public bool IsStartTile; //Indicates wether this tile is a start tile
    public bool IsEndTile; //Indicates wether this tile is a end tile
    public bool IsInRange; //Indicates wether or not this tile is in range of the selected unit
    public bool IsLocked; //Indicates if the tile has already been walked on
    public bool IsBDoubleWalk; //Indicates wether this tile is a double walk bonus tile
    public int BWalksLeft; //How many time this tile can be walked on
    public bool IsBJump; //Indicates wether this tile is a jump bonus tile
    public int BJumpToTileID; //The tile id of the tile to jump to when this tile is a bjump tile


    Vector3 OriginalScale; //Set on start, used to indicate tile interactions
    Material DefaultMaterial; //Set on start, used to indicate tile interactions
    bool IsHovered; //Indicates wether or not this tile is hovered
    bool IsPressed; //Indicates wether or not this tile is pressed
    


    // Use this for initialization
    void Start () {
        OriginalScale = transform.localScale;
        DefaultMaterial = GetComponent<MeshRenderer>().material;
        TileID = int.Parse(Regex.Replace(name, @"\D", ""));

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetAsEndTile() //Sets a tile as End Tile meaning that if all unit walks on it, they level will end
    {
        GetComponent<MeshRenderer>().material = EndTileMaterial;
        IsEndTile = true;
    }

    public void SetAsStartTile() //Sets a tile as start tile (where units spawn)
    {
        GetComponent<MeshRenderer>().material = StartTileMaterial;
        IsStartTile = true;
    }

    public void SetTileBonus(TileBonus Bonus)
    {
        switch (Bonus)
        {
            case TileBonus.DoubleWalk:

                IsBDoubleWalk = true;
                BWalksLeft = 1 ;
                GetComponentInChildren<SpriteRenderer>().sprite = Bonus_WalkSprite;

                break;

            case TileBonus.Jump:

                IsBJump = true;
                GetComponentInChildren<SpriteRenderer>().sprite = Bonus_JumpSprite;

                break;
        }
    }

    public void Hovered() //Puts the tile in Hover mode
    {
        if (!IsHovered)
        {
            transform.localScale *= 1.1f;
            IsHovered = true;
        }
    }

    public void Pressed() //Puts the tile in Pressed mode
    {
        if (!IsPressed)
        {
            transform.localScale *= 0.9f;
            IsPressed = true;
        }
    }

    public void Reset() //Used to reset the tile to its original state IF the tile isn't in range of the selected unit
    {
        if (!IsInRange)
        {
            HardReset();
        }

        if (IsInRange)
        {
            HardReset();
            ReadyToMove();
        }
    }

    public void HardReset() //Used to force the tile to its original state regardless of the unit
    {
            transform.localScale = OriginalScale;
            IsInRange = false;
            IsHovered = false;
            IsPressed = false;

        if (!IsLocked)
        {
            GetComponent<MeshRenderer>().material = DefaultMaterial;
        }
        if (IsLocked)
        {
            if (IsStartTile)
            {
                GetComponent<MeshRenderer>().material = ToMoveOnStartTile;
                return;
            }
            if (IsEndTile)
            {
                GetComponent<MeshRenderer>().material = ToMoveOnEndTile;
                return;
            }
        }
    }

    public void ReadyToMove() //Puts the tile in range to move mode
    {
        if (!IsInRange)
        {
            if (IsLocked)
            {
                return;
            }

            if (IsStartTile)
            {
                GetComponent<MeshRenderer>().material = ToMoveOnStartTile;
                return;
            }
            if (IsEndTile)
            {
                IsInRange = true;
                transform.localScale *= 0.9f;
                GetComponent<MeshRenderer>().material = ToMoveOnEndTile;
                return;
            }
            else
            {
                IsInRange = true;
                transform.localScale *= 0.9f;
                GetComponent<MeshRenderer>().material = ToMoveMaterial;
            }                
        }
    }

    public void LockTile() //Locks tile, meaning a unit walked on it.
    {
        if (!IsLocked)
        {
            IsLocked = true;
            GetComponent<MeshRenderer>().material = StartTileMaterial;
            if (GetComponentInChildren<SpriteRenderer>())
            {
                GetComponentInChildren<SpriteRenderer>().enabled = false;
            }
        }
    }

    public void UnlockTile() //Unlocks a tile
    {
        if (IsLocked)
        {
            IsLocked = false;
        }
    }

    public void Bonus_Walk()
    {
        if (BWalksLeft > 0)
        {
            BWalksLeft = BWalksLeft -1 ;
        }
        else
        {
            Debug.Log("Tile is already dead (Bonus_Walk)");
        }
    }
}

