using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class UIButtonLevel : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        string LevelID = this.name;

        GetComponent<Button>().onClick.AddListener(delegate { OpenLevel(LevelID); });
    }


    public void OpenLevel(string ID)
    {
        Debug.Log("Opening Level" + ID);
        SceneManager.LoadScene(ID);
    }
}
