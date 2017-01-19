using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// This class is the controller behind the boardscan
/// </summary>
public class BoardController : MonoBehaviour
{
    /// <summary>
    /// Default Image for the map. It is the image at the beginning and when you undo an add.
    /// </summary>
    public Image defaultImage;

    /// <summary>
    /// This is the layoutWrapper. It is a struct used to contain a list.
    /// This is done because the Json Tool in unity does not allow to set a
    /// list to json.
    /// </summary>
    [System.Serializable]
    private struct layoutWrapper { public List<string> list; };
    private layoutWrapper layout;

    /// <summary>
    /// This is the playerWrapper. It is a struct used to contain a list.
    /// This is done because the Json Tool in unity does not allow to set a
    /// list to json.
    /// </summary>
    [System.Serializable]
    private struct playerWrapper { public List<Player> list; };
    private playerWrapper players;

    /// <summary>
    /// This is the incidentWrapper. It is a struct used to contain a list.
    /// This is done because the Json Tool in unity does not allow to set a
    /// list to json.
    /// </summary>
    [System.Serializable]
    private struct incidentWrapper { public List<Incident> list; };
    private incidentWrapper incidents;

    /// <summary>
    /// The treasure used in the game.
    /// </summary>
    private Treasure treasure;

    /// <summary>
    /// The index of the layout on which Village A is positioned
    /// </summary>
    private int villageAIndex;
    
    /// <summary>
    /// The index of the layout on which Village B is positioned
    /// </summary>
    private int villageBIndex;
    
    /// <summary>
    /// The index of the layout on which Village C is positioned
    /// </summary>
    private int villageCIndex;

    /// <summary>
    /// The start function, run when this script is started.
    /// </summary>
    void Start()
    {
        //Define 1 random for the entire script
        Random.InitState((int)System.DateTime.Now.Ticks);

        //Initalise the variables
        players.list = new List<Player>();
        incidents.list = new List<Incident>();
        layout.list = new List<string>();
        treasure = new Treasure();
        
        //Iniatialise the nfcreader
        AndroidNFCReader.enableBackgroundScan();
        AndroidNFCReader.ScanNFC("GameController", "OnScan");
    }
    
    /// <summary>
    /// This function is called when you scan an NFC Tag. What it scans gets inserted as the result param.
    /// </summary>
    /// <param name="result">The data the scanned NFC tag contains.</param>
    public void OnScan(string result)
    {
        //The integer value of the param result.
        int scan;
        //If result can be parsed to int
        if (int.TryParse(result, out scan))
        {
            //If the list is full, don't do anything
            if (layout.list.Count < 30)
            {
                //Check if it is not the first element to be added to the list
                if (layout.list.Count != 0)
                {
                    //If the previous scan is equal to the new one, stop
                    if (layout.list[layout.list.Count - 1] == result)
                        return;
                }

                //Find the next image to change the sprite
                Image tempImage = GameObject.Find("Image (" + layout.list.Count + ")").GetComponent<Image>();
                //Set the sprite to the corresponding sprite according to the value on the NFC tag
                tempImage.sprite = GameObject.Find("tile " + scan).GetComponent<SpriteRenderer>().sprite;
                //Add the result to the layout list
                layout.list.Add(result);
                //If a village is scanned save the index for incident location determination
                //3 = village A
                //4 = village B
                //5 = village C
                if(scan == 3)
                {
                    villageAIndex = layout.list.Count - 1;
                }
                else if (scan == 4)
                { 
                    villageBIndex = layout.list.Count - 1;
                }
                else if (scan == 5)
                {
                    villageCIndex = layout.list.Count - 1;
                }
            }
        }
    }

    /// <summary>
    /// The undo function to undo the previous scanned tile.
    /// </summary>
    public void Undo()
    {
        //Check if there are any scanned tiles
        if (layout.list.Count > 0)
        {
            //Get the image of which the sprite needs to be changed
            Image tempImage = GameObject.Find("Image (" + (layout.list.Count - 1) + ")").GetComponent<Image>();
            //Set the sprite to default
            tempImage.sprite = defaultImage.sprite;
            //Remove the scanned tile from the list
            layout.list.Remove(layout.list[layout.list.Count - 1]);
        }
    }

    /// <summary>
    /// The function to start the game. It calculates the neccesairy data and randoms for the game to work.
    /// </summary>
    public void StartGame()
    {
        #region crappyLayoutDefault
        OnScan("11");
        OnScan("17");
        OnScan("30");
        OnScan("5");
        OnScan("25");
        OnScan("15");
        OnScan("28");
        OnScan("2");
        OnScan("13");
        OnScan("18");
        OnScan("29");
        OnScan("7");
        OnScan("14");
        OnScan("20");
        OnScan("1");
        OnScan("24");
        OnScan("26");
        OnScan("12");
        OnScan("4");
        OnScan("10");
        OnScan("19");
        OnScan("9");
        OnScan("27");
        OnScan("16");
        OnScan("21");
        OnScan("23");
        OnScan("6");
        OnScan("22");
        OnScan("3");
        OnScan("8");
        #endregion
        //Check if layout scanning is done
        if (layout.list.Count == 30)
        {
            //Set players
            setPlayers();
            //Set treasure
            setTreasure();
            //Set incidents
            setIncidents();
                        
            //Set all calculated data to Json
            string layoutJson = JsonUtility.ToJson(layout);
            string playersJson = JsonUtility.ToJson(players);
            string treasureJson = JsonUtility.ToJson(treasure);
            string incidentsJson = JsonUtility.ToJson(incidents);
            
            //Execute the query
            SQL.Instance.executeQuery("INSERT INTO `board`(`active`, `roomID`, `layout`, `turn`, `players`, `treasure`, `incidents`) VALUES('true','" + RoomState.id + "','" + layoutJson + "',0,'" + playersJson + "','" + treasureJson + "','" + incidentsJson + "')");
            SQL.Instance.executeQuery("UPDATE `room` set started = 'true' where roomID = " + RoomState.id);
            //Load the gamescene
            SceneManager.LoadScene("game");
        }
    }

    /// <summary>
    /// Sets the random tile the treasure is located on
    /// </summary>
    private void setTreasure()
    {
        treasure.tile = (int)UnityEngine.Random.Range(0f, 29f);
    }

    /// <summary>
    /// Set the players, define the turn order
    /// </summary>
    private void setPlayers()
    {
        //The ints to set the random numbers for each player
        int rand1 = 0, rand2 = 0, rand3 = 0, rand4 = 0;
        //If any random number is equal, redefine all numbers
        while (rand1 == rand2 || rand1 == rand3 || rand1 == rand4 || rand2 == rand3 || rand2 == rand4 || rand3 == rand4)
        {
            rand1 = (int)Random.Range(0f, 20f);
            rand2 = (int)Random.Range(0f, 20f);
            rand3 = (int)Random.Range(0f, 20f);
            rand4 = (int)Random.Range(0f, 20f);
        }
        //As long as not all numbers are -1, keep going
        //-1 means they have been assigned a turn
        while (rand1 != -1 || rand2 != -1 || rand3 != -1 || rand4 != -1)
        {
            //If rand 1 is the highest, set it to -1, check if player1 exists and then if he/she exist, add to list
            //This repeats for everyplayer
            if (rand1 > rand2 && rand1 > rand3 && rand1 > rand4)
            {
                rand1 = -1;
                if (RoomState.p1.nickName != "")
                {
                    players.list.Add(RoomState.p1);
                }
            }
            if (rand2 > rand1 && rand2 > rand3 && rand2 > rand4)
            {
                rand2 = -1;
                if (RoomState.p2.nickName != "")
                {
                    players.list.Add(RoomState.p2);
                }
            }
            if (rand3 > rand1 && rand3 > rand2 && rand3 > rand4)
            {
                rand3 = -1;
                if (RoomState.p3.nickName != "")
                {
                    players.list.Add(RoomState.p3);
                }
            }
            if (rand4 > rand1 && rand4 > rand2 && rand4 > rand3)
            {
                rand4 = -1;
                if (RoomState.p4.nickName != "")
                {
                    players.list.Add(RoomState.p4);
                }
            }
        }
    }

    /// <summary>
    /// The function to set the incidents
    /// </summary>
    private void setIncidents()
    {
        //Get the incidents from the database
        string incidentsResult = SQL.Instance.executeQuery("select * from incident");
        //Check if the called data is correct
        if (incidentsResult != "TRUE")
        {
            //Split the incidents to an array of json strings
            string[] incidentSplitResult = incidentsResult.Split('*');
            //Loop through the array
            foreach (string incident in incidentSplitResult)
            {
                //Set the json string to an Incident object, add it to the list
                incidents.list.Add(JsonUtility.FromJson<Incident>(incident));
                //Define random tile for the incident, between 0 and 29
                incidents.list[incidents.list.Count - 1].tile = (int)Random.Range(0f, 29f);
                //If the incident location is equal to a village, recalc till it is not
                while(incidents.list[incidents.list.Count - 1].tile == villageAIndex || 
                    incidents.list[incidents.list.Count - 1].tile == villageBIndex ||
                    incidents.list[incidents.list.Count - 1].tile == villageCIndex)
                {
                    incidents.list[incidents.list.Count - 1].tile = (int)Random.Range(0f, 29f);
                }
            }
        }
    }
}