using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public Treasure treasure;

    public Text testText;
    public Text nfcTestText;

    public GameObject MoveAction;
    public GameObject DuringMove;
    public GameObject ChooseAction;
    public GameObject GameMid;
    public GameObject MapMid;
    public GameObject IncidentPopup;

    public int pullTimer = -1;
    public LocalLibrary local;
    public bool hasMoved = false;

    public Text popupText;
    //Incidnets
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

    //Map
    public Image city;
    public Image cave;
    public Image village;
    public Image lake;
    public Image open;
    public Image flowers;
    public Image forest;

    public bool myTurn;

    //Quests
    public Quest tempQuest;
    public Dialogue tempDialogue = new Dialogue();


    // Use this for initialization
    void Start()
    {        
        AndroidNFCReader.enableBackgroundScan();
        AndroidNFCReader.ScanNFC("GameController", "OnMove");
        MapMid.SetActive(true);
        local = new LocalLibrary();
        MapMid.SetActive(false);

        GameObject newImageObject = GameObject.Find("Energy" + PlayerState.energy);
        Image newImage = newImageObject.GetComponent<Image>();
        placeholderImage.sprite = newImage.sprite;

        Image jeep = GameObject.Find("JeepImage").GetComponent<Image>();
        Image hat = GameObject.Find("HatImage").GetComponent<Image>();
        
        jeep.sprite = GameObject.Find("Jeep" + PlayerState.vehicle).GetComponent<Image>().sprite;
        hat.sprite = GameObject.Find("Hat" + PlayerState.hat).GetComponent<Image>().sprite;
    }

    // Update is called once per frame
    void Update()
    {
        pullTimer++;
        if ((pullTimer > 120 || pullTimer == -1))
        {
            int turn = int.Parse(SQL.Instance.executeQuery("SELECT turn as `result` FROM `board` WHERE roomID =" + RoomState.id));
            local.updateData();
            pullTimer = 0;

            if (local.players.list[turn].accountID == PlayerState.id)
            {
                myTurn = true;
                startTurn();
            }

            if (local.turn == 100)
            {
                tempDialogue.title = "Te laat";
                tempDialogue.description = "Ai, een van je tegenstanders heeft de schat al gevonden. Helaas je verliest het spel.";
                tempDialogue.button = "Ga naar het hoofdmenu";
                tempDialogue.image = "TreasureL";
                togglePopUp();
            }
        }
    }

    public void startTurn()
    {
        if (PlayerState.energy == 0)
        {
            tempDialogue.title = "Benzine op";
            tempDialogue.description = "Oh nee, je brandstof is op.";
            tempDialogue.button = "Beeindig beurt";
            tempDialogue.image = "Elephant";
            togglePopUp();
        }
        else
        {
            MoveAction.SetActive(true);
        }
    }
    
    public void endTurn()
    {
        if (!hasMoved)
        {
            changeEnergy(3);
        }

        hasMoved = false;

        local.turn++;
        if (local.turn == local.players.list.Count)
        {
            AnimalDance();
            local.turn = 0;
        }
        MoveAction.SetActive(false);
        IncidentPopup.SetActive(false);
        ChooseAction.SetActive(false);
        MapMid.SetActive(false);
        GameMid.SetActive(true);

        myTurn = false;
        
        Debug.Log("UPDATE board set turn = " + local.turn + ", incidents = '" + JsonUtility.ToJson(local.incidents) + "', players = '" + JsonUtility.ToJson(local.players) + "'  where roomID = " + RoomState.id);
        SQL.Instance.executeQuery("UPDATE `board` set `turn` = " + local.turn + ", `incidents` = '" + JsonUtility.ToJson(local.incidents) + "', `players` = '" + JsonUtility.ToJson(local.players) + "'  where `roomID` = " + RoomState.id);

        //SQL.Instance.executeQuery("UPDATE board set turn = " + local.turn + ", incidents = 'k', players = 'k'  where roomID = " + RoomState.id);
    }

    public void questBtn()
    {
        tempQuest = null;
        tempDialogue = new Dialogue();
        popupText.text = local.players.list[local.turn].currentPosition + "";

        Quest bq = PlayerState.blueQuest;
        Quest rq = PlayerState.redQuest;
        Quest gq = PlayerState.greenQuest;

        checkQuest(bq, local.players.list[local.turn].currentPosition);
        checkQuest(rq, local.players.list[local.turn].currentPosition);
        checkQuest(gq, local.players.list[local.turn].currentPosition);
        
        if(tempQuest != null)
        {
            if(tempQuest.progress == 0)
            {
                tempDialogue = tempQuest.startDialogueD;
            } else if (tempQuest.progress == 1)
            {
                tempDialogue = tempQuest.doDialogueD;
            } else if (tempQuest.progress == 2)
            {
                tempDialogue = tempQuest.turnInDialogueD;
            } else if (tempQuest.progress == 3)
            {
                return;
            }
            togglePopUp();
        }
    }

    public void checkQuest(Quest q, int c)
    {
        if (c == q.startPosition && q.progress == 0 ||
            c == q.doPosition && q.progress == 1 ||
            c == q.turnInPosition && q.progress == 2)
        {
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

    public void popUpBtn()
    {
        if(tempDialogue.image == "Treasure" || tempDialogue.image == "TreasureL")
        {
            SceneManager.LoadScene("main");
        }
        else if(encounteredIncident != null)
        {
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
            if (encounteredIncident.action == "Energy+3" || encounteredIncident.action.Contains("Corner"))
            {
                IncidentPopup.SetActive(false);
            }
            else
            {
                endTurn();
            }
            encounteredIncident = null;
        }
        else if (tempQuest != null)
        {
            advanceProgress();
            endTurn();
        }
        else if (PlayerState.energy == 0)
        {
            endTurn();
        }
        else
        {
            GameMid.SetActive(false);
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

    public void closeMap()
    {
        MapMid.SetActive(false);
    }

    public void openMove()
    {
        MoveAction.SetActive(true);
    }

    public void closeMove()
    {
        MoveAction.SetActive(false);
    }

    public void openAction()
    {
        ChooseAction.SetActive(true);
    }

    public void closeAction()
    {
        ChooseAction.SetActive(false);
    }

    public void digTreasure()
    {
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
            togglePopUp();
            //Update database
            SQL.Instance.executeQuery("UPDATE `board` SET `turn`= 100 WHERE roomID = " + RoomState.id);
        }
    }

    public void questCheckMap(bool b)
    {
        MapMid.SetActive(true);
        /*int startPos = tempQuest.startPosition + 1;
        int doPos = tempQuest.doPosition + 1;
        int turnPos = tempQuest.turnInPosition + 1;
        Image tempImage = GameObject.Find("Image" + startPos).GetComponent<Image>();
        if (tempQuest.progress == 1)
        {
            tempImage = GameObject.Find("Image" + doPos).GetComponent<Image>();
        }
        else if (tempQuest.progress == 2)
        {
            tempImage = GameObject.Find("Image" + turnPos).GetComponent<Image>();
        }*/

        if (b)
        {
            //updateMiniMap();
            
        }
        else
        {
            MapMid.SetActive(false);
        }
    }

    /// <summary>
    /// This function updates the hints on the minimap.
    /// </summary>
    public void updateMiniMap()
    {
        //reset colors
        Image tempImage;
        for (int i = 1; i < 31; i++)
        {
            //Debug.Log(i);
            tempImage = GameObject.Find("Image" + i).GetComponent<Image>();
            tempImage.color = Color.white;
        }


        int treasureCol = local.treasure.tile % 6;
        float treasureRow = Mathf.Floor(local.treasure.tile / 6);

        //Debug.Log(treasureCol);
        //Debug.Log(treasureRow);

        //clues
        //first clues
        PlayerState.clues = 3;
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

            //second clue
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

                //third clue
                PlayerState.clues = 3;
                if (PlayerState.clues > 2)
                {
                    setColor(local.treasure.tile, Color.black);
                }
            }
        }


        //set next quest point to color of quest
        if(PlayerState.blueQuest.progress == 1)
        {
            setColor(PlayerState.blueQuest.doPosition, Color.blue);
        }
        else if(PlayerState.blueQuest.progress == 2)
        {
            if(PlayerState.blueQuest.turnInPoint != "")
            {
                setColor(PlayerState.blueQuest.turnInPosition, Color.blue);
            }
        }

        if(PlayerState.redQuest.progress == 1)
        {
            setColor(PlayerState.redQuest.doPosition, Color.red);
        }
        else if(PlayerState.redQuest.progress == 2)
        {
            if(PlayerState.redQuest.turnInPoint != "")
            {
                setColor(PlayerState.redQuest.turnInPosition, Color.red);
            }
        }

        if (PlayerState.greenQuest.progress == 1)
        {
            setColor(PlayerState.greenQuest.doPosition, Color.green);
        }
        else if (PlayerState.greenQuest.progress == 2)
        {
            if (PlayerState.greenQuest.turnInPoint != "")
            {
                setColor(PlayerState.greenQuest.turnInPosition, Color.green);
            }
        }

        //set color to white on current position
        int currentpos = local.players.list[local.turn].currentPosition;
        if (currentpos != 31)
        {
            setColor(local.players.list[local.turn].currentPosition, Color.grey);
        }
    }

    public void setColor(int p, Color c)
    {
        p++;
        Image i = GameObject.Find("Image" + p).GetComponent<Image>();
        i.color = c;
    }

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

    public void OnMove(string result)
    {
        if(local == null)
        {
            return;
        }
        if (myTurn && IncidentPopup.activeSelf == false)
        {
            // Set Values
            int scan;
            int.TryParse(result, out scan);
            int scanPosition = local.layout.list.IndexOf(scan.ToString());
            int currentPosition = local.players.list[local.turn].currentPosition;
            // If you scan a different tile

            //Find tile position in layout
            if (currentPosition == 31)
            {
                currentPosition = local.layout.list.IndexOf(scan.ToString());
                PlayerState.validMove = true;
            }
            else if (PlayerState.movedIncorrect)
            {
                if (scanPosition == currentPosition)
                {
                    popupText.text = "Klaar met bewegen";
                    PlayerState.movedIncorrect = false;
                    PlayerState.validMove = true;
                    endTurn();
                    return;
                }
            }
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
            else if (currentPosition == scanPosition && PlayerState.energy > 0)
            {
                changeEnergy(1);
                PlayerState.validMove = true;
            }
            if (!PlayerState.validMove && PlayerState.energy > 0)
            {
                popupText.text = "Ga terug naar je goede positie";  
                PlayerState.movedIncorrect = true;
                // show return phone to old place dialogue
            }
            else if (PlayerState.validMove && PlayerState.energy > 0)
            {
                // alter currenTile and energy
                local.players.list[local.turn].currentPosition = scanPosition;
                Debug.Log("" + local.players.list[local.turn].currentPosition);
                changeEnergy(-1);
                PlayerState.validMove = false;
                hasMoved = true;
                /*foreach (Incident i in local.incidents.list)
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
                }*/
                if (PlayerState.energy == 0)
                {
                    tempDialogue.title = "Benzine op";
                    tempDialogue.description = "Oh nee, je brandstof is op.";
                    tempDialogue.button = "Beeindig beurt";
                    tempDialogue.image = "Elephant";
                    togglePopUp();
                }
                else
                {
                    popupText.text = local.players.list[local.turn].currentPosition.ToString();
                }
            }
        }
    }

    public void AnimalDance()
    {
        foreach (Incident i in local.incidents.list)
        {
            if (i.name == "Elephant")
            {
                int randomTile;
                int.TryParse(UnityEngine.Random.Range(1f, 30f).ToString("0"), out randomTile);
                while (randomTile == local.positionVillageA || randomTile == local.positionVillageB || randomTile == local.positionVillageC)
                {
                    int.TryParse(UnityEngine.Random.Range(1f, 30f).ToString("0"), out randomTile);
                }
                i.tile = randomTile;
            }
        }
    }

    public void DisplayEnergy()
    {
        int currentEnergy = PlayerState.energy;
        GameObject newImageObject = GameObject.Find("Energy" + currentEnergy);
        Image newImage = newImageObject.GetComponent<Image>();

        placeholderImage.sprite = newImage.sprite;
    }

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
        
    public void exitGame()
    {
        SceneManager.LoadScene("main");
    }

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

    public void setTrap()
    {
        if(PlayerState.activeTrap == false)
        {
            PlayerState.activeTrap = true;
            Incident trap = new Incident();
            trap.tile = local.players.list[local.turn].currentPosition;
            trap.name = "Trap" + PlayerState.id;
            trap.action = "End";
            trap.title = "Oh nee, een val!";
            trap.description = "Je bent in een val gereden. Gebruik de rest van je beurt om vrij te komen.";
            trap.button = "Bevrijd jezelf";
            local.incidents.list.Add(trap);
        }
    }
}