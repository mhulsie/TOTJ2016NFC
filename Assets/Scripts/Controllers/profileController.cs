using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class profileController : MonoBehaviour {
    public Image jeep;
    public Image hat;

    public Image hatGray;
    public Image hatPink;
    public Image hatYellow;
    public Image hatGreen;

    public Image jeepRed;
    public Image jeepBlue;
    public Image jeepPurple;
    public Image jeepGreen;

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
            jeepRed.gameObject.SetActive(false);
            jeepBlue.gameObject.SetActive(false);
            jeepPurple.gameObject.SetActive(false);
            jeepGreen.gameObject.SetActive(false);
            if(c == "Red")
            {
                jeepRed.gameObject.SetActive(true);
            }
            if (c == "Blue")
            {
                jeepBlue.gameObject.SetActive(true);
            }
            if (c == "Purple")
            {
                jeepPurple.gameObject.SetActive(true);
            }
            if (c == "Green")
            {
                jeepGreen.gameObject.SetActive(true);
            }
            jeep.sprite = GameObject.Find(i + c).GetComponent<Image>().sprite;
            PlayerState.vehicle = c;
        }
        else
        {
            hatGray.gameObject.SetActive(false);
            hatPink.gameObject.SetActive(false);
            hatYellow.gameObject.SetActive(false);
            hatGreen.gameObject.SetActive(false);
            if (c == "Gray")
            {
                hatGray.gameObject.SetActive(true);
            }
            if (c == "Pink")
            {
                hatPink.gameObject.SetActive(true);
            }
            if (c == "Yellow")
            {
                hatYellow.gameObject.SetActive(true);
            }
            if (c == "Green")
            {
                hatGreen.gameObject.SetActive(true);
            }
            hat.sprite = GameObject.Find(i + c).GetComponent<Image>().sprite;
            PlayerState.hat = c;
        }
    }
}