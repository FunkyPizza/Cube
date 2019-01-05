using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;
using UnityEngine.SceneManagement;

public class UILevelGridManager : MonoBehaviour {

    [Header("Defaults")]
    [SerializeField]
    private GameObject titleText;
    [SerializeField]
    private GameObject buttonTemplate;
    [SerializeField]
    private GridLayoutGroup buttonGrid;
    [SerializeField]
    private GameObject LevelDataParent;

    [Header("Needs manual input")]
    public string levelTitle;

    int numberOfLevels;

	// Use this for initialization
	void Start () {

        GetLevelArray();
        
        SpawnGrid();
    }
	
    void GetLevelArray()
    {
        foreach (LevelLib i in LevelDataParent.GetComponent<LevelData>().LevelsInfo)
        {
                if(i.GridSize == levelTitle)
                {
                    numberOfLevels = i.QtyOfLevels;
                }
        }

        if (numberOfLevels == 0) { Debug.Log(levelTitle + " doesn't have any levels"); }
    }

    public void SpawnGrid()
    {
        //Set grid title (3x3, 4x4 etc ..)
        titleText.GetComponent<Text>().text = levelTitle;

        //Get No of rows, and set the grid size accordingly
        float nLevels = numberOfLevels;
        int nRows = (int)Math.Ceiling( nLevels / 4 ); 
        int nButtonsLastRow = numberOfLevels - (nRows * 4); 

        if (nRows > 1)
             { buttonGrid.constraintCount = 4; }
        else { buttonGrid.constraintCount = numberOfLevels; }

        //Set minimum and preferred cell size (figures out the spacing between this grid and the one below it)
        int gridSize = 100 + (nRows * 250);
        GetComponent<LayoutElement>().minHeight = gridSize;
        GetComponent<LayoutElement>().preferredHeight = gridSize;
      
        //Spawn the buttons
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for(int i = 0; i < numberOfLevels; i++)
        {

                //Instantiate the buttons
                GameObject newButton = Instantiate(buttonTemplate) as GameObject;
                newButton.transform.SetParent(buttonGrid.transform, false);
                newButton.SetActive(true);

                //Set the button's functionnality
                int tempLevelID = i + 1; 
                newButton.GetComponentInChildren<Text>().text = tempLevelID.ToString();
                newButton.name = levelTitle + "L" + tempLevelID.ToString();
        }
    }


}
