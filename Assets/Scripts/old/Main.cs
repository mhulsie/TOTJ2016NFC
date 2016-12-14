using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour {

    public Text myText;
	// Use this for initialization
	void Start () {
        AndroidNFCReader.enableBackgroundScan();
        AndroidNFCReader.ScanNFC("NFCtext", "OnScan");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnScan(string result)
    {
        myText.text = result;
    }
}
