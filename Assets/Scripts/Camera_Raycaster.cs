using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Raycaster : MonoBehaviour {

    [Header("Script settings")]
    public bool EnableLog; //Enables logging of the raycast process

    [Header("Live feedback")]
    public GameObject SelectedTile; //Transform of the selected tile (dynamically changes)
    public GameObject SelectedUnit; //Transform of the unit on the selected tile (dynamically changes)

    Camera Currentcamera; //Camera used to raycast from
    Transform CurrentTile; //The tile currently being affected by either a Press or a Hover

	
	void Start () // Use this for initialization
    {
        Currentcamera = gameObject.GetComponent<Camera>();
	}
	
	void Update () // Update is called once per frame
    {

        MouseRayCast();
        
	}

    void MouseRayCast() //Raycasts and calls the corresponding function
    {
        RaycastHit hit;
        Ray MouseRay = Currentcamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(MouseRay, out hit) && Input.GetMouseButton(0)) //Check if a tile is being pressed
        {
            if (hit.transform.tag == "Tile")
            {
                if (hit.transform.GetComponent<Tile_Interactions>().IsInRange)
                {
                    MoveUnitTo(hit.transform, hit.transform.GetComponent<Tile_Interactions>().TileID);
                    return;
                }
                else
                {
                    Select(hit.transform);
                }
            }
            return;
        }

        if (Physics.Raycast(MouseRay, out hit) && Input.GetMouseButton(1)) //Check if interaction with tile
        {
            if (SelectedUnit)
            {
                if (hit.transform.GetComponent<Tile_Interactions>())
                {
                    MoveUnitTo(hit.transform, hit.transform.GetComponent<Tile_Interactions>().TileID);
                }
            }
        }

        if (Physics.Raycast(MouseRay, out hit) && !Input.GetMouseButton(0)) //Check if the mouse is hovering a tile
        {
            if (hit.transform.tag == "Tile")
            {
                Hovered(hit.transform);
            }
            return;
        }

        if (!Input.GetMouseButtonDown(0)) //Check if button is released
        {
            UnHover();
            return;
        }

        if (!Physics.Raycast(MouseRay, out hit) && Input.GetMouseButton(0)) //Check if clicked on background
        {
            Deselect();
            SelectedTile = null;
            SelectedUnit = null;
            return;
        }
    }

    void Hovered(Transform Tile) //Hoveres the current tile
    {
        if (CurrentTile) { UnHover(); }

        CurrentTile = Tile;
        if (EnableLog) { Debug.Log(Tile.transform.name); }

        Tile.transform.gameObject.GetComponent<Tile_Interactions>().Hovered();
    } 

    public void Select(Transform Tile) //Presses the current tile
    {
        if (CurrentTile) { Deselect(); }
        
            CurrentTile = Tile;
            if (EnableLog) { Debug.Log(Tile.transform.name); }

        //Press logic call to Tile and Unit
        SelectedTile = Tile.gameObject;
        SelectedTile.transform.gameObject.GetComponent<Tile_Interactions>().Pressed();

        if (SelectedTile.GetComponent<Tile_Interactions>().Unit)
        {
            SelectedUnit = SelectedTile.GetComponent<Tile_Interactions>().Unit;
            SelectedUnit.GetComponent<Unit_Actions>().Select();
        }
    }

    void UnHover() //Resets the last pressed tile
    {
        if (CurrentTile)
        {
            CurrentTile.transform.gameObject.GetComponent<Tile_Interactions>().Reset();
            CurrentTile = null;
        }
    }

    public void Deselect()
    {
        UnHover();
        if (SelectedUnit) { SelectedUnit.GetComponent<Unit_Actions>().UnSelect(); }

        SelectedTile = null;
        SelectedUnit = null;
    }

    void MoveUnitTo(Transform Tile, int TileToMoveTo) //Triggers a unit move, checking if the tile is available
    {
        //Check moves
        if (Tile.transform.GetComponent<Tile_Interactions>().IsInRange)
        {
            if (!Tile.transform.GetComponent<Tile_Interactions>().IsLocked)
            {
                SelectedUnit.GetComponent<Unit_Actions>().AssignUnitToTileNo(TileToMoveTo);
                Deselect();

                if (!Tile.transform.GetComponent<Tile_Interactions>().IsEndTile) //Check if the tile is a EndTile
                {
                    CurrentTile = Tile.transform;
                    SelectedUnit = CurrentTile.GetComponent<Tile_Interactions>().Unit;
                    Select(Tile.transform);
                }


                return;
            }

        }
        else
        {
            if (EnableLog) { if (Tile.transform.GetComponent<Tile_Interactions>().IsLocked) { Debug.Log("You already walked on that tile!"); } }
            return;
        }
    }
}
