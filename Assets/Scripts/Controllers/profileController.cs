using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// This class handles the profile making.
/// </summary>
public class profileController : MonoBehaviour {
    //The shown images for current selected colors
    public Image jeep;
    public Image hat;

    //Preset images to set the selected border of the chosen color.
    public Image hatGray;
    public Image hatPink;
    public Image hatYellow;
    public Image hatGreen;
    public Image jeepRed;
    public Image jeepBlue;
    public Image jeepPurple;
    public Image jeepGreen;

    /// <summary>
    /// Called when a jeep button is pressed to select a color.
    /// </summary>
    /// <param name="c">The chosen color</param>
    public void jeepSelectColor(string c)
    {
        string i = "Jeep";
        setColor(c, i);
    }

    /// <summary>
    /// Called when a hat button is pressed to select a color.
    /// </summary>
    /// <param name="c">The chosen color</param>
    public void hatSelectColor(string c)
    {
        string i = "Hat";
        setColor(c, i);
    }

    /// <summary>
    /// This functions changes the shown images.
    /// </summary>
    /// <param name="c">The selected color</param>
    /// <param name="i">Is it jeep or hat?</param>
    private void setColor(string c, string i)
    {
        //The jeep path has commentary, the hat part not but it works the same.
        //If jeep is selected
        if(i == "Jeep")
        {
            //Set all old image edges inactive.
            jeepRed.gameObject.SetActive(false);
            jeepBlue.gameObject.SetActive(false);
            jeepPurple.gameObject.SetActive(false);
            jeepGreen.gameObject.SetActive(false);
            //Based on color, set the right one active
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
            //Find the correct sprite according to selected color and hat or jeep
            jeep.sprite = GameObject.Find(i + c).GetComponent<Image>().sprite;
            //Set the color in the playerstate so the value can be used later
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