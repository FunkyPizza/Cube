using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISwitcher : MonoBehaviour
{
    int ActivePanel; //Current Active Panel index
    int PanelIDToSwitchTo; //Set when switching panels

    public GameObject[] PanelToSwitchTo; //Array of all panels to switch between

    // Use this for initialization
    void Start () {

        PanelIDToSwitchTo = 0;
        FinishSwitch();

	}
	
    public void SwitchToPanel(int ID)
    {
        PanelIDToSwitchTo = ID;

        PanelToSwitchTo[ActivePanel].GetComponent<UIManager>().CloseUI();

        Invoke("FinishSwitch", 0.40f);
    }

    void FinishSwitch()
    {

        PanelToSwitchTo[ActivePanel].SetActive(false);

        PanelToSwitchTo[PanelIDToSwitchTo].SetActive(true);
        PanelToSwitchTo[PanelIDToSwitchTo].GetComponent<UIManager>().OpenUI();

        ActivePanel = PanelIDToSwitchTo;
    }

}
