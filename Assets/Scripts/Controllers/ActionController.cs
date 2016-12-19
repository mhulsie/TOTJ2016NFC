using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionController : MonoBehaviour {

    public GameObject MoveAction;
    public GameObject DuringMove;
    public GameObject ChooseAction;

    private GameObject currentMid;

	// Use this for initialization
	void Start () {
        currentMid = MoveAction;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void switchPanel(GameObject panel)
    {
        currentMid.gameObject.SetActive(false);
        currentMid = panel;
        panel.gameObject.SetActive(true);
    }

    public void startMove()
    {
        switchPanel(DuringMove);
    }

    public void startAction()
    {
        switchPanel(ChooseAction);
    }

    public void chooseAction()
    {

    }
}
