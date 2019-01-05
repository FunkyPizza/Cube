using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable] public struct LevelLib { public string GridSize; public int QtyOfLevels; }

public class LevelData : MonoBehaviour {

    public LevelLib[] LevelsInfo;

}
