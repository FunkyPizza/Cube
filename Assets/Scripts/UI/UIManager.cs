using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public GameObject[] AnimatedObjects; // OpenUI() will trigger bool isFocused for each AnimatedObjects
    public GameObject SettingButton; //Main menu only, toggle isSetting bool


    void Start()
    {
        OpenUI();
    }

    public void OpenUI()
    {
        foreach (GameObject CurrentObject in AnimatedObjects)
        {
            CurrentObject.GetComponent<Animator>().SetBool("isFocused", true);
        }
    }

    public void CloseUI()
    {
        CloseSettings();

        foreach (GameObject CurrentObject in AnimatedObjects)
        {
            CurrentObject.GetComponent<Animator>().SetBool("isFocused", false);
        }


    }

    public void ToggleSettings()
    {
        if (SettingButton.GetComponent<Animator>().GetBool("isFocused"))
        {
            if (SettingButton)
            {
                if (SettingButton.GetComponent<Animator>().GetBool("isSetting"))
                {
                    CloseSettings();
                }
                else
                {
                    SettingButton.GetComponent<Animator>().SetBool("isSetting", true);
                }
            }
        }
    }

    void CloseSettings()
    {
        if (SettingButton)
        {
            SettingButton.GetComponent<Animator>().SetBool("isSetting", false);
        }
    }
}