using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// This class controlls the game. Everything that happens in the game is called to from here.
/// </summary>
public class GameController : MonoBehaviour
{
    /// <summary>
    /// The treasure object.
    /// </summary>
    public Treasure treasure;
    
    /// <summary>
    /// The panel for during move
    /// </summary>
    public GameObject MoveAction;
    /// <summary>
    /// The panel with all the actions
    /// </summary>
    public GameObject ChooseAction;
    /// <summary>
    /// The panel for what you see when waiting for the next turn.
    /// </summary>
    public GameObject GameMid;
    /// <summary>
    /// The minimap
    /// </summary>
    public GameObject MapMid;
    /// <summary>
    /// The incidentpopup
    /// </summary>
    public GameObject IncidentPopup;

    /// <summary>
    /// The request pulltimer.
    /// </summary>
    public int pullTimer = -1;
    /// <summary>
    /// The locallibrary with all local info
    /// </summary>
    public LocalLibrary local;
    /// <summary>
    /// Did the player move this turn?
    /// </summary>
    public bool hasMoved = false;

    /// <summary>
    /// The shown text when its the players turn in the overlay
    /// </summary>
    public Text popupText;

    /// <summary>
    /// The lement that contains data to fill the incidentPopUp
    /// </summary>
    public Text title;
    public Text description;
    public Text incidentBtn;
    public Image incidentImage;
    public Image ElephantPlaceHolder;
    public Incident encounteredIncident = null;

    //Energy Images
    public Image Energy0;
    public Image Energy1;
    public Image Energy2;
    public Image Energy3;
    public Image Energy4;
    public Image Energy5;
    public Image Energy6;
    public Image Energy7;
    public Image Energy8;
    public Image Energy9;
    public Image Energy10;
    public Image Energy11;
    public Image Energy12;
    public Image Energy13;
    public Image Energy14;
    public Image Energy15;
    public Image placeholderImage;
    
    /// <summary>
    /// Is it my turn?
    /// </summary>
    public bool myTurn;

    //Quests
    /// <summary>
    /// The encountered quest
    /// </summary>
    public Quest tempQuest;
    /// <summary>
    /// A default dialogue, will be filled with all the incident info to fill the incidentPopUp
    /// </summary>
    public Dialogue tempDialogue = new Dialogue();


    /// <summary>
    /// The start function
    /// </summary>
    void Start()
    {
        //Initialise NFC
        AndroidNFCReader.enableBackgroundScan();
        AndroidNFCReader.ScanNFC("GameController", "OnMove");
        //Set the mapmid active so the map can be filled
        MapMid.SetActive(true);
        // Create the locallibrary
        local = new LocalLibrary();
        //Set the mapmid inactive
        MapMid.SetActive(false);

        //Set the energy sprite so it shows correctly
        GameObject newImageObject = GameObject.Find("Energy" + PlayerState.energy);
        Image newImage = newImageObject.GetComponent<Image>();
        placeholderImage.sprite = newImage.sprite;

        //Set the correct Jeep Image
        Image jeep = GameObject.Find("JeepImage").GetComponent<Image>();
        jeep.sprite = GameObject.Find("Jeep" + PlayerState.vehicle).GetComponent<Image>().sprite;
        //Set the correct Hat Image
        Image hat = GameObject.Find("HatImage").GetComponent<Image>();
        hat.sprite = GameObject.Find("Hat" + PlayerState.hat).GetComponent<Image>().sprite;
    }

    // Update is called once per frame
    void Update()
    {
        //Update the energy shown
        DisplayEnergy();
        // Up the timer
        pullTimer++;
        if ((pullTimer > 120 || pullTimer == -1))
        {
            //Select currentturn
            int turn = int.Parse(SQL.Instance.executeQuery("SELECT turn as `result` FROM `board` WHERE roomID =" + RoomState.id));
            //Update locallibrary
            local.updateData();
            //Reset timer
            pullTimer = 0;
            
            //Is it myturn?
            if (local.players.list[turn].accountID == PlayerState.id)
            {
                //If so start my turn
                myTurn = true;
                startTurn();
            }

            //Has someone else won?
            if (local.turn == 100)
            {
                //if so, show the popup
                tempDialogue.title = "Te laat";
                tempDialogue.description = "Ai, een van je tegenstanders heeft de schat al gevonden. Helaas je verliest het spel.";
                tempDialogue.button = "Ga naar het hoofdmenu";
                //tempDialogue.image = "TreasureL";
                togglePopUp();
            }
        }
    }

    /// <summary>
    /// This function is called upon when a player starts his turn.
    /// After this function, if the player gets his turn. The OnMove function awaits to be triggered
    /// </summary>
    public void startTurn()
    {
        //If the player has no energy, show popup
        if (PlayerState.energy == 0)
        {
            tempDialogue.title = "Benzine op";
            tempDialogue.description = "Oh nee, je brandstof is op.";
            tempDialogue.button = "Beeindig beurt";
            tempDialogue.image = "Elephant";
            togglePopUp();
        }
        //Else start the players turn
        else
        {
            MoveAction.SetActive(true);
        }
    }
    
    /// <summary>
    /// This function ends the players turn
    /// </summary>
    public void endTurn()
    {
        // If the player hasnt moved, increase his energy
        if (!hasMoved)
        {
            changeEnergy(3);
        }
        hasMoved = false;
        //Increase the turn
        local.turn++;
        //If it was the last turn, move around the animals and reset turn to 0
        if (local.turn == local.players.list.Count)
        {
            AnimalDance();
            local.turn = 0;
        }
        //Reset the active panels
        MoveAction.SetActive(false);
        IncidentPopup.SetActive(false);
        ChooseAction.SetActive(false);
        MapMid.SetActive(false);
        GameMid.SetActive(true);

        //Set myTurn to false
        myTurn = false;
        
        //Write away the changed data
        SQL.Instance.executeQuery("UPDATE `board` set `turn` = " + local.turn + ", `incidents` = '" + JsonUtility.ToJson(local.incidents) + "', `players` = '" + JsonUtility.ToJson(local.players) + "'  where `roomID` = " + RoomState.id);
    }

    /// <summary>
    /// The function that is called when the player presses a quest button
    /// </summary>
    public void questBtn()
    {
        tempQuest = null;
        tempDialogue = new Dialogue();
        popupText.text = local.players.list[local.turn].currentPosition + "";

        Quest bq = PlayerState.blueQuest;
        Quest rq = PlayerState.redQuest;
        Quest gq = PlayerState.greenQuest;

        //Check which quest the player encountered
        checkQuest(bq, local.players.list[local.turn].currentPosition);
        checkQuest(rq, local.players.list[local.turn].currentPosition);
        checkQuest(gq, local.players.list[local.turn].currentPosition);
        
        //If a quest is encountered
        if(tempQuest != null)
        {
            //Progress 0 means quest has yet to be accepted
            if(tempQuest.progress == 0)
            {
                tempDialogue = tempQuest.startDialogueD;
            }
            //Progress 1 means quest has yet to be done
            else if (tempQuest.progress == 1)
            {
                tempDialogue = tempQuest.doDialogueD;
            }
            //Progress 2 means quest has yet to be turned in
            else if (tempQuest.progress == 2)
            {
                tempDialogue = tempQuest.turnInDialogueD;
            }
            //Progress 3 means quest is done
            else if (tempQuest.progress == 3)
            {
                return;
            }
            // Show the quest popup
            togglePopUp();
        }
    }

    /// <summary>
    /// Check if the given quest and position of the player match
    /// </summary>
    /// <param name="q">Given quest</param>
    /// <param name="c">Current position of the player</param>
    public void checkQuest(Quest q, int c)
    {
        if (c == q.startPosition && q.progress == 0 ||
            c == q.doPosition && q.progress == 1 ||
            c == q.turnInPosition && q.progress == 2)
        {
            //If a match is found, set tempquest to the encountered quest
            tempQuest = q;
        }
    }

    public void togglePopUp()
    {
        IncidentPopup.SetActive(true);
        title.text = tempDialogue.title;
        description.text = tempDialogue.description;
        incidentBtn.text = tempDialogue.button;
        //incidentImage.sprite = GameObject.Find(tempDialogue.image).GetComponent<Image>().sprite;
    }

    /// <summary>
    /// Called when the button in incidentPopUp is pressed.
    /// </summary>
    public void popUpBtn()
    {
        //If the game was won or lost
        if(tempDialogue.image == "Treasure" || tempDialogue.image == "TreasureL")
        {
            SceneManager.LoadScene("main");
        }
        //If an incident was encountered
        else if(encounteredIncident != null)
        {
            //Based on the incidents action, do something
            switch (encounteredIncident.action)
            {
               /* case "CornerNE":
                    local.players.list[currentTurn].currentPosition = 5;
                    PlayerState.movedIncorrect = true;
                    break;
                case "CornerNW":
                    local.players.list[currentTurn].currentPosition = 0;
                    PlayerState.movedIncorrect = true;
                    break;
                case "CornerSE":
                    local.players.list[currentTurn].currentPosition = 29;
                    PlayerState.movedIncorrect = true;
                    break;
                case "CornerSW":
                    local.players.list[currentTurn].currentPosition = 24;
                    PlayerState.movedIncorrect = true;
                    break;*/
                case "Energy-1":
                    changeEnergy(-1);
                    break;
                case "Energy-2":
                    changeEnergy(-2);
                    break;
                case "Energy-3":
                    changeEnergy(-3);
                    break;
                case "Energy+3":
                    changeEnergy(3);
                    break;
            }
            //If player may continue after incident
            if (encounteredIncident.action == "Energy+3" || encounteredIncident.action.Contains("Corner"))
            {
                IncidentPopup.SetActive(false);
            }
            //If player has to end turn after incident
            else
            {
                endTurn();
            }
            encounteredIncident = null;
        }
        //If a quest was encountered
        else if (tempQuest != null)
        {
            IncidentPopup.SetActive(false);
            advanceProgress();
            endTurn();
        }
        //If the player ran out of energy
        else if (PlayerState.energy == 0)
        {
            endTurn();
        }
    }

    public void advanceProgress()
    {
        if(tempQuest.type == PlayerState.blueQuest.type)
        {
            PlayerState.blueQuest.progress++;
            if(PlayerState.blueQuest.progress == 2)
            {
                if (PlayerState.blueQuest.turnInPoint == "")
                {
                    PlayerState.blueQuest.progress = 3;
                }
            }
            if(PlayerState.blueQuest.progress == 3)
            {
                PlayerState.clues++;
            }
        }
        if (tempQuest.type == PlayerState.redQuest.type)
        {
            PlayerState.redQuest.progress++;
            if (PlayerState.redQuest.progress == 2)
            {
                if (PlayerState.redQuest.turnInPoint == "")
                {
                    PlayerState.redQuest.progress = 3;
                }
            }
            if (PlayerState.redQuest.progress == 3)
            {
                PlayerState.clues++;
            }
        }
        if (tempQuest.type == PlayerState.greenQuest.type)
        {
            PlayerState.greenQuest.progress++;
            if (PlayerState.greenQuest.progress == 2)
            {
                if (PlayerState.greenQuest.turnInPoint == "")
                {
                    PlayerState.greenQuest.progress = 3;
                }
            }
            if (PlayerState.greenQuest.progress == 3)
            {
                PlayerState.clues++;
            }
        }
        tempQuest = null;
    }

    /// <summary>
    /// Open and close the minimap
    /// </summary>
    public void openMap()
    {
        if (MapMid.activeSelf)
        {
            MapMid.SetActive(false);
            return;
        }
        MapMid.SetActive(true);
        updateMiniMap();
    }

    /// <summary>
    /// Open the MoveActionPanel
    /// </summary>
    public void openMove()
    {
        MoveAction.SetActive(true);
    }

    /// <summary>
    /// Close the MoveActionPanel
    /// </summary>
    public void closeMove()
    {
        MoveAction.SetActive(false);
    }

    /// <summary>
    /// Opens the action Panel
    /// </summary>
    public void openAction()
    {
        ChooseAction.SetActive(true);
    }

    /// <summary>
    /// Close the actionpanel
    /// </summary>
    public void closeAction()
    {
        ChooseAction.SetActive(false);
    }

    /// <summary>
    /// This functions digs up the treasure
    /// </summary>
    public void digTreasure()
    {
        // Has the player completed all quests and is on the correct position?
        if((PlayerState.blueQuest.progress + PlayerState.redQuest.progress + PlayerState.greenQuest.progress) == 9 && local.players.list[local.turn].currentPosition == local.treasure.tile)
        {
            MoveAction.SetActive(false);
            ChooseAction.SetActive(false);
            MapMid.SetActive(false);
            //Show win dialogue
            tempDialogue.title = "Je hebt de schat gevonden";
            tempDialogue.description = "Hoera! Je hebt de schat gevonden en het spel gewonnen!";
            tempDialogue.button = "Ga naar het hoofdmenu";
            tempDialogue.image = "Treasure";
            SQL.Instance.executeQuery("UPDATE `board` SET `turn`= 100 WHERE roomID = " + RoomState.id);
            togglePopUp();
        }
    }
        
    /// <summary>
    /// This function updates the hints on the minimap.
    /// </summary>
    public void updateMiniMap()
    {
        //Reset all markers
        Image tempImage;
        for (int i = 1; i < 31; i++)
        {
            tempImage = GameObject.Find("Image" + i).GetComponent<Image>();
            tempImage.color = Color.white;
        }

        // Calc on which column the treasure is
        int treasureCol = local.treasure.tile % 6;
        // Calc on which row the treasure is
        float treasureRow = Mathf.Floor(local.treasure.tile / 6);

        // Has the player finished any quests?
        if (PlayerState.clues > 0)
        {
            if (treasureCol == 0)
            {
                colorColumn(0);
                colorColumn(1);
                colorColumn(2);
            }

            if(treasureCol>0 && treasureCol < 5)
            {
                colorColumn(treasureCol - 1);
                colorColumn(treasureCol);
                colorColumn(treasureCol + 1);
            }

            if(treasureCol == 5)
            {
                colorColumn(3);
                colorColumn(4);
                colorColumn(5);
            }

            // Has the player completed more than 1 quest
            if(PlayerState.clues > 1)
            {

                if(treasureRow == 0)
                {
                    colorRow(0);
                    colorRow(1);
                    colorRow(2);
                }

                if(treasureRow>0 && treasureRow < 4)
                {
                    colorRow(treasureRow - 1);
                    colorRow(treasureRow);
                    colorRow(treasureRow + 1);
                }

                if(treasureRow == 4)
                {
                    colorRow(2);
                    colorRow(3);
                    colorRow(4);
                }

                //Has the player finished all quests?
                if (PlayerState.clues > 2)
                {
                    setColor(local.treasure.tile, Color.black);
                }
            }
        }


        //Set the quest markers for each quests, based on the progress of the quest
        if(PlayerState.blueQuest.progress == 1)
        {
            setColor(PlayerState.blueQuest.doPosition, Color.blue);
        }
        else if (PlayerState.blueQuest.progress == 2)
        {
            setColor(PlayerState.blueQuest.doPosition, Color.blue);
        }

        if(PlayerState.redQuest.progress == 1)
        {
            setColor(PlayerState.redQuest.doPosition, Color.red);
        } else if (PlayerState.redQuest.progress == 1)
        {
            setColor(PlayerState.redQuest.doPosition, Color.red);
        }

        if (PlayerState.greenQuest.progress == 1)
        {
            setColor(PlayerState.greenQuest.doPosition, Color.green);
        }
        else if(PlayerState.greenQuest.progress == 2)
        {
            setColor(PlayerState.greenQuest.turnInPosition, Color.green);
        }

        //Set the marker on the current position of the player
        int currentpos = local.players.list[local.turn].currentPosition;
        if (currentpos != 31)
        {
            setColor(local.players.list[local.turn].currentPosition, Color.grey);
        }
    }

    /// <summary>
    /// Set the correct color on the given position
    /// </summary>
    /// <param name="p">The given position</param>
    /// <param name="c">The color</param>
    public void setColor(int p, Color c)
    {
        p++;
        Image i = GameObject.Find("Image" + p).GetComponent<Image>();
        i.color = c;
    }

    /// <summary>
    /// Color an entire column
    /// </summary>
    /// <param name="col">Which column</param>
    public void colorColumn(int col)
    {
        Color color = Color.green;

        if(col == 0)
        {
            for(int i = 0; i<=24; i += 6)
            {
                setColor(i, color);
            }
        }

        if(col == 1)
        {
            for (int i = 1; i <= 25; i += 6)
            {
                setColor(i, color);
            }
        }

        if(col == 2)
        {
            for (int i = 2; i <= 26; i += 6)
            {
                setColor(i, color);
            }
        }

        if (col == 3)
        {
            for (int i = 3; i <= 27; i += 6)
            {
                setColor(i, color);
            }
        }

        if (col == 4)
        {
            for (int i = 4; i <= 28; i += 6)
            {
                setColor(i, color);
            }
        }

        if (col == 5)
        {
            for (int i = 5; i <= 29; i += 6)
            {
                setColor(i, color);
            }
        }
    }

    /// <summary>
    /// Color an entire row
    /// </summary>
    /// <param name="row">Which row</param>
    public void colorRow(float row)
    {
        Color color = Color.green;

        if(row == 0)
        {
            for(int i=0; i<=5; i++)
            {
                setColor(i, color);
            }
        }

        if (row == 1)
        {
            for (int i = 6; i <= 11; i++)
            {
                setColor(i, color);
            }
        }

        if (row == 2)
        {
            for (int i = 12; i <= 17; i++)
            {
                setColor(i, color);
            }
        }

        if (row == 3)
        {
            for (int i = 18; i <= 23; i++)
            {
                setColor(i, color);
            }
        }

        if (row == 4)
        {
            for (int i = 24; i <= 29; i++)
            {
                setColor(i, color);
            }
        }
    }

    /// <summary>
    /// This function is called when the player scans a tile
    /// </summary>
    /// <param name="result">The scanned value of the NFC Tag</param>
    public void OnMove(string result)
    {
        //If the locallibrary hasnt been made yet, do nothing
        if(local == null)
        {
            return;
        }
        // If it is the players turn and the incident popup is not active
        if (myTurn && IncidentPopup.activeSelf == false)
        {
            // Set the scanned value to the correct position
            int scan;
            int.TryParse(result, out scan);
            int scanPosition = local.layout.list.IndexOf(scan.ToString());
            int currentPosition = local.players.list[local.turn].currentPosition;

            //Is it the players first turn?
            if (currentPosition == 31)
            {
                // set position and validate move
                currentPosition = local.layout.list.IndexOf(scan.ToString());
                PlayerState.validMove = true;
            }
            //Has the player moved inccorect in a previous turn?
            else if (PlayerState.movedIncorrect)
            {
                //if the new scanned tile is correct with the position the player should be on
                // Show pop up and end
                if (scanPosition == currentPosition)
                {
                    popupText.text = "Klaar met bewegen";
                    PlayerState.movedIncorrect = false;
                    PlayerState.validMove = true;
                    endTurn();
                    return;
                }
            }
            //If player scanned a different tile
            //Calculate if its a valid move
            else if (currentPosition != scanPosition && PlayerState.energy > 0)
            {
                // if modulo 0 niet naar rechts
                if ((currentPosition + 1) % 6 != 0)
                {
                    if (currentPosition + 1 == scanPosition)
                    {
                        PlayerState.validMove = true;
                    }
                }
                if (currentPosition < 24)
                {
                    if (currentPosition + 6 == scanPosition)
                    {
                        PlayerState.validMove = true;
                    }
                }
                if ((currentPosition + 1) % 6 != 1)
                {
                    if (currentPosition - 1 == scanPosition)
                    {
                        PlayerState.validMove = true;
                    }
                }
                if (currentPosition > 5)
                {
                    if (currentPosition - 6 == scanPosition)
                    {
                        PlayerState.validMove = true;
                    }
                }
            }
            // If Player scanned the same tile where he already was at
            else if (currentPosition == scanPosition && PlayerState.energy > 0)
            {
                PlayerState.validMove = true;
            }
            // If the playe scanned a wrong tile
            // Show it to the player and set movedincorrect to false
            if (!PlayerState.validMove && PlayerState.energy > 0)
            {
                popupText.text = "Ga terug naar je goede positie";  
                PlayerState.movedIncorrect = true;
                // show return phone to old place dialogue
            }
            //If the player moved correctly, update energy and postiion
            else if (PlayerState.validMove && PlayerState.energy > 0)
            {
                // alter currenTile and energy
                local.players.list[local.turn].currentPosition = scanPosition;
                changeEnergy(-1);
                PlayerState.validMove = false;
                hasMoved = true;

                //Check if player encountered an incident
                // If so set the popup and randomise the new position of the incident
                foreach (Incident i in local.incidents.list)
                {
                    if (i.tile == local.players.list[local.turn].currentPosition)
                    {
                        encounteredIncident = i;

                        tempDialogue.title = encounteredIncident.title;
                        tempDialogue.description = encounteredIncident.description;
                        tempDialogue.button = encounteredIncident.button;
                        tempDialogue.image = "Elephant";
                        togglePopUp();
                        int randomTile;
                        int.TryParse(UnityEngine.Random.Range(1f, 30f).ToString("0"), out randomTile);
                        i.tile = randomTile;
                    }
                }
                // If the player ran out of energy, show and end turn
                if (PlayerState.energy == 0)
                {
                    tempDialogue.title = "Benzine op";
                    tempDialogue.description = "Oh nee, je brandstof is op.";
                    tempDialogue.button = "Beeindig beurt";
                    tempDialogue.image = "Elephant";
                    togglePopUp();
                }
                //If player has moved correctly, still has energy and didnt encounter an incident
                else
                {
                    popupText.text = local.players.list[local.turn].currentPosition.ToString();
                }
            }
        }
    }

    /// <summary>
    /// Moves around the animal incidents. Called when every player has had their turn
    /// </summary>
    public void AnimalDance()
    {
        foreach (Incident i in local.incidents.list)
        {
            if (i.name == "Elephant")
            {
                int randomTile;
                int.TryParse(UnityEngine.Random.Range(1f, 30f).ToString("0"), out randomTile);
                //Animals may not be on a village tile
                while (randomTile == local.positionVillageA || randomTile == local.positionVillageB || randomTile == local.positionVillageC)
                {
                    int.TryParse(UnityEngine.Random.Range(1f, 30f).ToString("0"), out randomTile);
                }
                i.tile = randomTile;
            }
        }
    }

    /// <summary>
    /// Update the shown energy image.
    /// </summary>
    public void DisplayEnergy()
    {
        int currentEnergy = PlayerState.energy;
        GameObject newImageObject = GameObject.Find("Energy" + currentEnergy);
        Image newImage = newImageObject.GetComponent<Image>();

        placeholderImage.sprite = newImage.sprite;
    }

    /// <summary>
    /// Update the players energy
    /// </summary>
    /// <param name="points">How much does the energy need to change</param>
    public void changeEnergy(int points)
    {
        PlayerState.energy += points;
        if(PlayerState.energy < 0)
        {
            PlayerState.energy = 0;
        } else if (PlayerState.energy > 15)
        {
            PlayerState.energy = 15;
        }
    }
        
    /// <summary>
    /// Exit the game
    /// </summary>
    public void exitGame()
    {
        SceneManager.LoadScene("main");
    }

    /// <summary>
    /// Toggle sound on and off
    /// </summary>
    public void toggleSound()
    {
        Image soundImage = GameObject.Find("SoundBtn").GetComponent<Image>();
        if (PlayerState.sound)
        {
            PlayerState.sound = false;
            soundImage.sprite = GameObject.Find("SoundOff").GetComponent<Image>().sprite;
        }
        else if (!PlayerState.sound)
        {
            PlayerState.sound = true;
            soundImage.sprite = GameObject.Find("SoundOn").GetComponent<Image>().sprite;
        }
    }
    /// <summary>
    /// Add a trap to the game
    /// </summary>
    public void setTrap()
    {
        //Current player can only set 1 trap
        if(PlayerState.activeTrap == false)
        {
            PlayerState.activeTrap = true;

            // Create the trap incident
            Incident trap = new Incident();
            trap.tile = local.players.list[local.turn].currentPosition;
            trap.name = "Trap" + PlayerState.id;
            trap.action = "End";
            trap.title = "Oh nee, een val!";
            trap.description = "Je bent in een val gereden. Gebruik de rest van je beurt om vrij te komen.";
            trap.button = "Bevrijd jezelf";

            //Add to the local incidents list so it gets written away at the end of turn
            local.incidents.list.Add(trap);
        }
    }
}