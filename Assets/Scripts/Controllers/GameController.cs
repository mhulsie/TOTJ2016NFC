using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public int currentTurn;
    public Treasure treasure;

    public Text testText;
    public Text nfcTestText;

    public GameObject MoveAction;
    public GameObject DuringMove;
    public GameObject ChooseAction;
    public GameObject GameMid;
    public GameObject MapMid;
    public GameObject IncidentPopup;

    //private GameObject currentMid;

    [System.Serializable]
    public struct playerWrapper { public List<Player> list; };
    public playerWrapper players;

    [System.Serializable]
    public struct incidentWrapper { public List<Incident> list; };
    public incidentWrapper incidents;

    public int pullTimer = -1;
    public LocalLibrary local;
    public bool myTurn = false;
    public bool hasMoved = false;

    public Text debugtestText;

    //Incidnets
    public Text title;
    public Text description;
    public Text incidentBtn;
    public Image incidentImage;
    public Image ElephantPlaceHolder;
    public Incident encounteredIncident;

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

    //Quests
    public int positionVillageC;
    public int positionVillageB;
    public int positionVillageA;
    public int positionAirplane;
    public int positionCave;
    public List<int> positionLake;
    public List<int> positionOpen;
    public List<int> positionFlowers;
    public List<int> positionForest;
    public List<int> positionBirds;
    public List<int> positionPlants;
    public Quest tempQuest;
    public Dialogue tempDialogue = new Dialogue();


    // Use this for initialization
    void Start()
    {        
        positionLake = new List<int>();
        positionOpen = new List<int>();
        positionFlowers = new List<int>();
        positionForest = new List<int>();
        positionBirds = new List<int>();
        positionPlants = new List<int>();
        PlayerState.energy = 15;
        PlayerState.clues = 0;
        encounteredIncident = null;
        AndroidNFCReader.enableBackgroundScan();
        AndroidNFCReader.ScanNFC("GameController", "OnMove");

        //currentMid = GameMid;
        local = new LocalLibrary();
        if (RoomState.host == PlayerState.id)
        {
            Random.InitState((int)System.DateTime.Now.Ticks);

            foreach (Incident item in local.incidents.list)
            {
                item.tile = (int)Random.Range(1f, 30f);
            }
            SQL.Instance.getData("UPDATE `board` SET `incidents`='" + JsonUtility.ToJson(local.incidents) + "' WHERE boardID = " + local.board.boardID);
            SQL.Instance.getData("UPDATE room set started = 'true' where roomID = " + RoomState.id);
        }
        
        setMap();
        MapMid.SetActive(false);

        int currentEnergy = PlayerState.energy;
        GameObject newImageObject = GameObject.Find("Energy" + currentEnergy);
        Image newImage = newImageObject.GetComponent<Image>();

        placeholderImage.sprite = newImage.sprite;

        Image jeep = GameObject.Find("JeepImage").GetComponent<Image>();
        Image hat = GameObject.Find("HatImage").GetComponent<Image>();
        
        jeep.sprite = GameObject.Find("Jeep" + PlayerState.vehicle).GetComponent<Image>().sprite;
        hat.sprite = GameObject.Find("Hat" + PlayerState.hat).GetComponent<Image>().sprite;

        defineCodeQuests();
    }

    // Update is called once per frame
    void Update()
    {
        // Keep pulling to see if its my turn
        // Dont pull if its my turn

        DisplayEnergy();
        pullTimer++;
        if ((pullTimer > 120 || pullTimer == -1) && !myTurn)
        {
            pullTimer = 0;
            int.TryParse(SQL.Instance.getData("SELECT turn as result FROM board WHERE boardID = " + local.board.boardID), out currentTurn);
            if(currentTurn == 100)
            {
                tempDialogue.title = "Te laat";
                tempDialogue.description = "Ai, een van je tegenstanders heeft de schat al gevonden. Helaas je verliest het spel.";
                tempDialogue.button = "Ga naar het hoofdmenu";
                tempDialogue.image = "TreasureL";

            }
            if (PlayerState.id == local.players.list[currentTurn].accountID)
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
                    myTurn = true;
                    MoveAction.SetActive(true);
                }
            }
        }

        //Start action panel sequence
    }

    public void createGameState()
    {

    }

    public void submitGameState()
    {
        SQL.Instance.getData("UPDATE `totj`.`board` SET `Gamestate` = '" + UnityEngine.JsonUtility.ToJson(this) + "' where roomID = " + RoomState.id);
    }

    public GameState getGameState()
    {
        return JsonUtility.FromJson<GameState>(SQL.Instance.getData("select Gamestate from board where roomID = " + RoomState.id));
    }

    #region ActionPanels

    public void endTurn()
    {
        if (!hasMoved)
        {
            changeEnergy(3);
        }

        hasMoved = false;

        currentTurn++;
        if (currentTurn == local.players.list.Count)
        {
            AnimalDance();
            currentTurn = 0;
        }
        MoveAction.SetActive(false);
        IncidentPopup.SetActive(false);
        ChooseAction.SetActive(false);
        MapMid.SetActive(false);
        GameMid.SetActive(true);
        
        myTurn = false;
        string incidentsJson = JsonUtility.ToJson(local.incidents);
        SQL.Instance.getData("UPDATE board set turn = " + currentTurn + ", incidents = '" + incidentsJson + "', players = '" + JsonUtility.ToJson(local.players) + "'  where roomID = " + RoomState.id);
    }
    #endregion

    public void questBtn()
    {
        tempQuest = null;
        tempDialogue = new Dialogue();
        int currentPosition = local.players.list[currentTurn].currentPosition;

        Quest bq = PlayerState.blueQuest;
        Quest rq = PlayerState.redQuest;
        Quest gq = PlayerState.greenQuest;
        //Quest eq = PlayerState.energyQuest;

        checkQuest(bq, currentPosition);
        checkQuest(rq, currentPosition);
        checkQuest(gq, currentPosition);
        //checkQuest(eq, currentPosition);
        
        if(tempQuest != null)
        {
            debugtestText.text += "  " + tempQuest.type;
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
        incidentImage.sprite = GameObject.Find(tempDialogue.image).GetComponent<Image>().sprite;
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
                    PlayerState.clues++;
                }
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
                    PlayerState.clues++;
                }
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
                    PlayerState.clues++;
                }
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
        if((PlayerState.blueQuest.progress + PlayerState.redQuest.progress + PlayerState.greenQuest.progress) == 9 && local.players.list[currentTurn].currentPosition == local.board.treasureT.tile)
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
            SQL.Instance.getData("UPDATE `board` SET `turn`= 100 WHERE roomID = " + RoomState.id);
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
            Debug.Log(i);
            tempImage = GameObject.Find("Image" + i).GetComponent<Image>();
            tempImage.color = Color.white;
        }


        int treasureCol = local.board.treasureT.tile % 6;
        float treasureRow = Mathf.Floor(local.board.treasureT.tile / 6);

        Debug.Log(treasureCol);
        Debug.Log(treasureRow);

        //clues
        //first clues
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
                if (PlayerState.clues > 2)
                {
                    setColor(treasure.tile, Color.black);
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
        int currentpos = local.players.list[currentTurn].currentPosition;
        if (currentpos != -1)
        {
            setColor(local.players.list[currentTurn].currentPosition, Color.grey);
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
        if (myTurn && IncidentPopup.activeSelf == false)
        {
            // Set Values
            int scan;
            int.TryParse(result, out scan);
            int scanPosition = local.layout.layout.IndexOf(scan.ToString());
            int currentPosition = local.players.list[currentTurn].currentPosition;
            // If you scan a different tile

            //Find tile position in layout
            if (currentPosition == -1)
            {
                currentPosition = local.layout.layout.IndexOf(scan.ToString());
                PlayerState.validMove = true;
            }
            else if (PlayerState.movedIncorrect)
            {
                if (scanPosition == currentPosition)
                {
                    PlayerState.movedIncorrect = false;
                    PlayerState.validMove = true;
                    endTurn();
                }
                else
                {
                    //debugtestText.text = " nog steeds foute tegel";
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
                PlayerState.validMove = true;
            }
            if (!PlayerState.validMove && PlayerState.energy > 0)
            {
                debugtestText.text = " verkeerde beweging";
                PlayerState.movedIncorrect = true;
                // show return phone to old place dialogue
            }
            else if (PlayerState.validMove && PlayerState.energy > 0)
            {
                // alter currenTile and energy
                local.players.list[currentTurn].currentPosition = scanPosition;
                changeEnergy(-1);
                PlayerState.validMove = false;
                hasMoved = true;
                foreach (Incident i in local.incidents.list)
                {
                    if (i.tile == local.players.list[currentTurn].currentPosition)
                    {
                        encounteredIncident = i;

                        tempDialogue.title = encounteredIncident.title;
                        tempDialogue.description = encounteredIncident.description;
                        tempDialogue.button = encounteredIncident.button;
                        tempDialogue.image = "Elephant";
                        togglePopUp();
                    }
                }
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
                    debugtestText.text = "correcte beweging, ga nog eens" + PlayerState.blueQuest.progress + " " + PlayerState.redQuest.progress + " " + PlayerState.greenQuest.progress + "          " + PlayerState.energy;
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

    public void setMap()
    {
        //local.layout.layout.RemoveAt(0);
        //local.layout.layout[0] = "11";
        //local.layout.layout.Add("8");

        MapMid.SetActive(true);
        for (int i = 0; i < 30; i++)
        {
            int j = i + 1;
            Image tempImage = GameObject.Find("Image" + j).GetComponent<Image>();
            int current = int.Parse(local.layout.layout[i]);
            tempImage.sprite = GameObject.Find("tile " + current).GetComponent<SpriteRenderer>().sprite;

            if (current == 1)
            {
                positionAirplane = i;
            }
            else if (current == 2)
            {
                positionCave = i;
            }
            else if (current == 3 || current == 4 || current == 5)
            {
                if(current == 3)
                {
                    positionVillageA = i;
                }
                else if(current == 4)
                {
                    positionVillageB = i;
                }
                else if(current == 5)
                {
                    positionVillageC = i;
                }
            }
            else if (current == 6 || current == 7)
            {
                positionLake.Add(i);
            }
            else if (current >= 8 && current <= 13)
            {
                positionOpen.Add(i);
            }
            else if (current == 14 || current == 15 || current == 16)
            {
                positionPlants.Add(i);
            }
            else if (current > 16)
            {
                positionForest.Add(i);
            }

            if(current == 8 || current == 9 || current == 10 || current == 14 || current == 20|| current == 22 || current == 30)
            {
                positionBirds.Add(i);
            }
            if(current == 17 || current == 18 || current == 19)
            {
                positionFlowers.Add(i);
            }
        }
        MapMid.SetActive(false);
    }

    public void defineCodeQuests()
    {
        int turn = 0;
        foreach (Player item in local.players.list)
        {
            if (item.accountID != PlayerState.id)
            {
                turn++;
            }
        }

        setRandomTile(turn);
        setRandomTile(turn + 4);
        setRandomTile(turn + 8);

        PlayerState.redQuest = local.quests[turn];
        PlayerState.greenQuest = local.quests[turn + 4];
        PlayerState.blueQuest = local.quests[turn + 8];

        PlayerState.redQuest.progress = 0;
        PlayerState.greenQuest.progress = 0;
        PlayerState.blueQuest.progress = 0;

        PlayerState.redQuest.startDialogueD = JsonUtility.FromJson<Dialogue>(PlayerState.redQuest.startDialogue);
        PlayerState.redQuest.doDialogueD = JsonUtility.FromJson<Dialogue>(PlayerState.redQuest.doDialogue);
        PlayerState.redQuest.turnInDialogueD = JsonUtility.FromJson<Dialogue>(PlayerState.redQuest.turnInDialogue);

        PlayerState.blueQuest.startDialogueD = JsonUtility.FromJson<Dialogue>(PlayerState.blueQuest.startDialogue);
        PlayerState.blueQuest.doDialogueD = JsonUtility.FromJson<Dialogue>(PlayerState.blueQuest.doDialogue);
        PlayerState.blueQuest.turnInDialogueD = JsonUtility.FromJson<Dialogue>(PlayerState.blueQuest.turnInDialogue);

        PlayerState.greenQuest.startDialogueD = JsonUtility.FromJson<Dialogue>(PlayerState.greenQuest.startDialogue);
        PlayerState.greenQuest.doDialogueD = JsonUtility.FromJson<Dialogue>(PlayerState.greenQuest.doDialogue);
        PlayerState.greenQuest.turnInDialogueD = JsonUtility.FromJson<Dialogue>(PlayerState.greenQuest.turnInDialogue);
    }

    public void setRandomTile(int i)
    {
        if(local.quests[i].startPoint == "3")
        {
            local.quests[i].startPosition = positionVillageA;
        }
        else if (local.quests[i].startPoint == "4")
        {
            local.quests[i].startPosition = positionVillageB;
        }
        else if (local.quests[i].startPoint == "5")
        {
            local.quests[i].startPosition = positionVillageC;
        }

        switch (local.quests[i].doPoint)
        {
            case "cave":
                local.quests[i].doPosition = positionCave;
                break;
            case "villageBlue":
                local.quests[i].doPosition = positionVillageB;
                break;
            case "villageGreen":
                local.quests[i].doPosition = positionVillageA;
                break;
            case "airplane":
                local.quests[i].doPosition = positionAirplane;
                break;
            case "birds":
                local.quests[i].doPosition = positionBirds[(int)Random.Range(0f, positionBirds.Count -1)];
                break;
            case "plants":
                local.quests[i].doPosition = positionPlants[(int)Random.Range(0f, positionPlants.Count - 1)];
                break;
            case "flowers":
                local.quests[i].doPosition = positionFlowers[(int)Random.Range(0f, positionFlowers.Count - 1)];
                break;
            case "water":
                local.quests[i].doPosition = positionLake[(int)Random.Range(0f, positionLake.Count - 1)];
                break;
        }
        if (local.quests[i].turnInPoint == "3")
        {
            local.quests[i].turnInPosition = positionVillageA;
        }
        else if (local.quests[i].turnInPoint == "4")
        {
            local.quests[i].turnInPosition = positionVillageB;
        }
        else if (local.quests[i].turnInPoint == "5")
        {
            local.quests[i].turnInPosition = positionVillageC;
        }
        Debug.Log(JsonUtility.ToJson(local.quests[i]));
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
}