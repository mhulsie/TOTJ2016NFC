using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class profileController : MonoBehaviour {
    public Image jeep;
    public Image hat;

    public void jeepSelectColor(string c)
    {
        string i = "Jeep";
        setColor(c, i);
    }

    public void hatSelectColor(string c)
    {
        string i = "Hat";
        setColor(c, i);
    }

    private void setColor(string c, string i)
    {
        if(i == "Jeep")
        {
            GameObject.Find(i + "BtnRed").GetComponent<Button>().interactable = true;
            GameObject.Find(i + "BtnBlue").GetComponent<Button>().interactable = true;
            GameObject.Find(i + "BtnPurple").GetComponent<Button>().interactable = true;
            GameObject.Find(i + "BtnGreen").GetComponent<Button>().interactable = true;
            jeep.sprite = GameObject.Find(i + c).GetComponent<Image>().sprite;
            PlayerState.vehicle = c;
        }
        else
        {
            GameObject.Find(i + "BtnGray").GetComponent<Button>().interactable = true;
            GameObject.Find(i + "BtnPink").GetComponent<Button>().interactable = true;
            GameObject.Find(i + "BtnYellow").GetComponent<Button>().interactable = true;
            GameObject.Find(i + "BtnGreen").GetComponent<Button>().interactable = true;
            hat.sprite = GameObject.Find(i + c).GetComponent<Image>().sprite;
            PlayerState.hat = c;
        }
        GameObject.Find(i + "Btn" + c).GetComponent<Button>().interactable = false;
    }
}