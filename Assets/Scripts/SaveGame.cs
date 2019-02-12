using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveGame : MonoBehaviour {

    [Header("SettingsButtons")]
    [SerializeField]
    GameObject SoundButton; //Used to swap sprite on press
    [SerializeField]
    GameObject VibesButton; //Used to swap sprite on press

    [Header("SettingsSprites")]
    [SerializeField]
    Sprite SoundOn;
    [SerializeField]
    Sprite SoundOff;
    [SerializeField]
    Sprite VibesOn;
    [SerializeField]
    Sprite VibesOff;

    bool isSoundOn;
    bool isVibesOn;

    


    // Use this for initialization
    void Start()
    {

    }


    public void ToggleSound()
    {
        if (isSoundOn)
        {
            isSoundOn = false;
            SoundButton.GetComponent<Image>().sprite = SoundOff;
        }
        else
        {
            isSoundOn = true;
            SoundButton.GetComponent<Image>().sprite = SoundOn;
        }
    }

    public void ToggleVibes()
    {
        if (isVibesOn)
        {
            isVibesOn = false;
            VibesButton.GetComponent<Image>().sprite = VibesOff;
        }
        else
        {
            isVibesOn = true;
            VibesButton.GetComponent<Image>().sprite = VibesOn;
        }
    }
}
