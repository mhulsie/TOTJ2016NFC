using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private GameObject currentMid;

    [System.Serializable]
    public struct playerWrapper { public List<Player> list; };
    public playerWrapper players;

    [System.Serializable]
    public struct incidentWrapper { public List<Incident> list; };
    public incidentWrapper incidents;

    public int pullTimer = -1;
    public LocalLibrary local;
    public bool myTurn = false;

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
    public int positionVillageRed;
    public int positionVillageBlue;
    public int positionVillageGreen;
    public int positionAirplane;
    public int positionCave;
    public List<int> positionLake;
    public List<int> positionOpen;
    public List<int> positionFlowers;
    public List<int> positionForest;
    public List<int> positionBirds;
    public List<int> positionPlants;
    public Quest encounteredQuest;
    public Quest tempQuest = new Quest();
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
        encounteredIncident = null;
        encounteredQuest = null;
        AndroidNFCReader.enableBackgroundScan();
        AndroidNFCReader.ScanNFC("GameController", "OnMove");

        currentMid = GameMid;
        local = new LocalLibrary();
        if (RoomState.host == PlayerState.id)
        {
            Random.InitState((int)System.DateTime.Now.Ticks);

            foreach (Incident item in local.incidents.list)
            {
                item.tile = (int)Random.Range(1f, 30f);
            }
            SQL.Instance.getData("UPDATE room set started = 'true' where roomID = " + RoomState.id);
            SQL.Instance.getData("UPDATE `board` SET `incidents`='" + JsonUtility.ToJson(local.incidents) + "' WHERE boardID = " + local.board.boardID);
        }
        
        setMap();
        MapMid.SetActive(false);

        int currentEnergy = PlayerState.energy;
        GameObject newImageObject = GameObject.Find("Energy" + currentEnergy);
        Image newImage = newImageObject.GetComponent<Image>();

        placeholderImage.sprite = newImage.sprite;

        Debug.Log("voor quest definition");
        defineCodeQuests();
        Debug.Log("na quest definition");
    }

    // Update is called once per frame
    void Update()
    {
        // Keep pulling to see if its my turn
        // Dont pull if its my turn
        Debug.Log(currentTurn);
        Debug.Log(local.players.list[currentTurn].accountID);

        DisplayEnergy();
        if (pullTimer > 120 || pullTimer == -1)
        {
            pullTimer = 0;
            if (myTurn != true)
            {
                int.TryParse(SQL.Instance.getData("SELECT turn as result FROM board WHERE boardID = " + local.board.boardID), out currentTurn);
                Debug.Log(local.players.list[currentTurn].accountID);
                if (PlayerState.id == local.players.list[currentTurn].accountID)
                {
                    myTurn = true;
                    debugtestText.text = "Currenturn = " + currentTurn;
                    MoveAction.SetActive(true);
                    local.players.list[0].currentPosition = 28;
                }
            }
        }
        else
        {
            pullTimer++;
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
    /*public void switchPanel(GameObject panel)
    {
        currentMid.gameObject.SetActive(false);
        currentMid = panel;
        panel.gameObject.SetActive(true);
    }*/

    public void endTurn()
    {
        currentTurn++;
        if (currentTurn == local.players.list.Count)
        {
            Debug.Log("DIERENDANS");
            MoveAction.SetActive(false);
            AnimalDance();
            currentTurn = 0;
        }
        myTurn = false;
        string incidentsJson = JsonUtility.ToJson(local.incidents);
        debugtestText.text = local.quests[0].name;
        SQL.Instance.getData("UPDATE board set turn = " + currentTurn + ", incidents = '" + incidentsJson + "', players = '" + JsonUtility.ToJson(local.players) + "'  where roomID = " + RoomState.id);
    }

    public void setTrap()
    {

    }

    /*public void getQuest()
    {
        //3 blue
        if(local.players.list[currentTurn].currentPosition == positionVillageBlue && PlayerState.blueQuest.tile == -1)
        {
            encounteredQuest = PlayerState.blueQuest;
        }
        //4 red
        else if (local.players.list[currentTurn].currentPosition == positionVillageRed && PlayerState.redQuest.tile == -1)
        {
            encounteredQuest = PlayerState.redQuest;
        }
        //5 green
        else if (local.players.list[currentTurn].currentPosition == positionVillageGreen && PlayerState.greenQuest.tile == -1)
        {
            encounteredQuest = PlayerState.greenQuest;
        }
        else
        {
            //TODO add popup stating quest you have to do
        }
        if(encounteredQuest != null)
        {
            switchPanel(IncidentPopup);
        }
    }

    public void doQuest()
    {
        if(local.players.list[currentTurn].currentPosition == PlayerState.blueQuest.tile)
        {
            encounteredQuest = PlayerState.blueQuest;
        }
        else if (local.players.list[currentTurn].currentPosition == PlayerState.redQuest.tile)
        {
            encounteredQuest = PlayerState.redQuest;
        }
        else if (local.players.list[currentTurn].currentPosition == PlayerState.greenQuest.tile)
        {
            encounteredQuest = PlayerState.greenQuest;
        }
        else
        {
            //TODO message stating you dont have a quest here
        }

        if(encounteredQuest != null)
        {
            switchPanel(IncidentPopup);
        }
    }*/
    #endregion

    public void questBtn()
    {
        tempQuest = null;
        tempDialogue = new Dialogue();
        int currentPosition = local.players.list[currentTurn].currentPosition;

        Quest bq = PlayerState.blueQuest;
        Quest rq = PlayerState.redQuest;
        Quest gq = PlayerState.greenQuest;
        Quest eq = PlayerState.energyQuest;

        checkQuest(bq, currentPosition);
        checkQuest(rq, currentPosition);
        checkQuest(gq, currentPosition);
        //checkQuest(eq, currentPosition);

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
            c == q.turnInPosition && q.progress == 2 && q.turnInPoint != "")
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
    }

    public void popUpBtn()
    {
        if(tempQuest != null)
        {
            advanceProgress();
            if (tempDialogue.button == "Kijk op kaart")
            {
                questCheckMap(true);
            }
            IncidentPopup.SetActive(false);
            ChooseAction.SetActive(false);
            endTurn();
        }
        else
        {
            IncidentOke();
        }

    }

    public void advanceProgress()
    {
        if(tempQuest == PlayerState.blueQuest)
        {
            PlayerState.blueQuest.progress++;
            if(PlayerState.blueQuest.progress == 2)
            {
                if (PlayerState.blueQuest.turnInPoint == "")
                {
                    PlayerState.blueQuest.progress = 3;
                }
            }
        }
        if (tempQuest == PlayerState.redQuest)
        {
            PlayerState.redQuest.progress++;
            if (PlayerState.redQuest.progress == 2)
            {
                if (PlayerState.redQuest.turnInPoint == "")
                {
                    PlayerState.redQuest.progress = 3;
                }
            }
        }
        if (tempQuest == PlayerState.greenQuest)
        {
            PlayerState.greenQuest.progress++;
            if (PlayerState.greenQuest.progress == 2)
            {
                if (PlayerState.greenQuest.turnInPoint == "")
                {
                    PlayerState.greenQuest.progress = 3;
                }
            }
        }
    }

    public void openMap()
    {
        MapMid.SetActive(true);
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

    public void questCheckMap(bool b)
    {
        MapMid.SetActive(true);
        Image tempImage = GameObject.Find("Image" + tempQuest.startPosition).GetComponent<Image>();
        if (tempQuest.progress == 1)
        {
            tempImage = GameObject.Find("Image" + tempQuest.doPosition).GetComponent<Image>();
        }
        else if (tempQuest.progress == 2)
        {
            tempImage = GameObject.Find("Image" + tempQuest.turnInPosition).GetComponent<Image>();
        }
        if (b)
        {
            if(tempQuest.type == "red")
            {
                tempImage.color = new Color(255, 0, 0);
            } else if(tempQuest.type == "green")
            {
                tempImage.color = new Color(0, 255, 0);
            } else if(tempQuest.type == "blue")
            {
                tempImage.color = new Color(0, 0, 255);
            }
            tempQuest = null;
        }
        else
        {
            tempImage.color = new Color(0, 0, 0);
            MapMid.SetActive(false);
        }
    }

    public void OnMove(string result)
    {
        if (myTurn)
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
                    debugtestText.text = " nog steeds foute tegel";
                }
            }
            else if (currentPosition != scanPosition)
            {
                // if modulo 0 niet naar rechts
                if ((currentPosition + 1) % 6 != 0)
                {
                    if (currentPosition + 1 == scanPosition)
                    {
                        debugtestText.text = "goed +1";
                        PlayerState.validMove = true;
                    }
                }
                if (currentPosition < 24)
                {
                    if (currentPosition + 6 == scanPosition)
                    {
                        debugtestText.text = "goed +6";
                        PlayerState.validMove = true;
                    }
                }
                if ((currentPosition + 1) % 6 != 1)
                {
                    if (currentPosition - 1 == scanPosition)
                    {
                        debugtestText.text = "goed -1";
                        PlayerState.validMove = true;
                    }
                }
                if (currentPosition > 5)
                {
                    if (currentPosition - 6 == scanPosition)
                    {
                        debugtestText.text = "goed -6";
                        PlayerState.validMove = true;
                    }
                }
            }
            else if(currentPosition == scanPosition)
            {
                PlayerState.validMove = true;
            }
            if (!PlayerState.validMove)
            {
                debugtestText.text = " verkeerde beweging";
                PlayerState.movedIncorrect = true;
                // show return phone to old place dialogue
            }
            else
            {
                debugtestText.text = "goede 1";
                // alter currenTile and energy
                local.players.list[currentTurn].currentPosition = scanPosition;
                changeEnergy(-1);
                PlayerState.validMove = false;
                debugtestText.text = "correcte beweging, ga nog eens";

                foreach (Incident i in local.incidents.list)
                {
                    if (i.tile == local.players.list[currentTurn].currentPosition)
                    {
                        debugtestText.text = "ROAR het is gelijk aan " + i.name;
                        if(encounteredIncident == null)
                        {
                            encounteredIncident = i;

                            IncidentPopup.SetActive(true);
                            if (i.name == "Elephant")
                            {
                                incidentImage.sprite = ElephantPlaceHolder.sprite;
                            }
                            title.text = i.title;
                            description.text = i.description;
                            incidentBtn.text = i.button;
                        }
                    }
                }
            }
        }
    }

    public void IncidentOke()
    {
        if(encounteredIncident != null)
        {
            switch (encounteredIncident.action)
            {
                case "CornerNE":
                    local.players.list[currentTurn].currentPosition = 5;
                    PlayerState.movedIncorrect = true;
                    endTurn();
                    break;
                case "CornerNW":
                    local.players.list[currentTurn].currentPosition = 0;
                    PlayerState.movedIncorrect = true;
                    endTurn();
                    break;
                case "CornerSE":
                    local.players.list[currentTurn].currentPosition = 29;
                    PlayerState.movedIncorrect = true;
                    endTurn();
                    break;
                case "CornerSW":
                    local.players.list[currentTurn].currentPosition = 24;
                    PlayerState.movedIncorrect = true;
                    endTurn();
                    break;
                case "End":
                    endTurn();
                    break;
                case "Energy-1":
                    changeEnergy(-1);
                    endTurn();
                    break;
                case "Energy-2":
                    changeEnergy(-2);
                    endTurn();
                    break;
                case "Energy-3":
                    changeEnergy(-3);
                    endTurn();
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
                IncidentPopup.SetActive(false);
                //switchPanel(GameMid);
            }
            encounteredIncident = null;
        }
        /*else if(encounteredQuest != null)
        {
            title.text = encounteredQuest.title;
            description.text = encounteredQuest.description;
            incidentBtn.text = encounteredQuest.button;

            int tempQuest;
            if(int.TryParse(encounteredQuest.result, out tempQuest))
            {
                tempQuest--;
                int randomTile = randomTileQuest();
                if (encounteredQuest == PlayerState.blueQuest)
                {
                    PlayerState.blueQuest = local.quests[tempQuest];
                    PlayerState.blueQuest.tile = randomTile;
                }
                else if (encounteredQuest == PlayerState.redQuest)
                {
                    PlayerState.redQuest = local.quests[tempQuest];
                    PlayerState.redQuest.tile = randomTile;
                }
                else if (encounteredQuest == PlayerState.greenQuest)
                {
                    PlayerState.greenQuest = local.quests[tempQuest];
                    PlayerState.greenQuest.tile = randomTile;
                }
                else if (encounteredQuest == PlayerState.energyQuest)
                {
                    PlayerState.energyQuest = local.quests[tempQuest];
                }
            }
            else
            {
                if(encounteredQuest.type == "blue")
                {
                    PlayerState.blueQuest = null;
                }
                else if(encounteredQuest.type == "red")
                {
                    PlayerState.redQuest = null;
                }
                else if(encounteredQuest.type == "green")
                {
                    PlayerState.greenQuest = null;
                }
                else if(encounteredQuest.type == "energy")
                {
                    //TODO add energy reward
                    PlayerState.energyQuest = null;
                }
            }
            encounteredQuest = null;
        }*/
    }

    public void AnimalDance()
    {
        foreach (Incident i in local.incidents.list)
        {
            if (i.name == "Elephant")
            {
                int randomTile;
                int.TryParse(UnityEngine.Random.Range(1f, 30f).ToString("0"), out randomTile);

                debugtestText.text = i.name;
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
        for (int i = 1; i < 31; i++)
        {
            Image tempImage = GameObject.Find("Image" + i).GetComponent<Image>();
            int current;
            /*if (int.TryParse(local.layout.layout[i - 1], out current))
            {
                //
            }
            else
            {
                local.layout.layout.Add("8");
            }*/
            current = int.Parse(local.layout.layout[i - 1]);
            Debug.Log("i = " + i + " current = " + current);
            if (current == 1)
            {
                tempImage.sprite = city.sprite;
                debugtestText.text += i;
                positionAirplane = i;
            }
            else if (current == 2)
            {
                tempImage.sprite = cave.sprite;
                positionCave = i;
            }
            else if (current == 3 || current == 4 || current == 5)
            {
                tempImage.sprite = village.sprite;
                if(current == 3)
                {
                    positionVillageRed = i;
                } else if(current == 4)
                {
                    positionVillageBlue = i;
                } else if(current == 5)
                {
                    positionVillageGreen = i;
                }
            }
            else if (current == 6 || current == 7)
            {
                tempImage.sprite = lake.sprite;
                positionLake.Add(i);
            }
            else if (current >= 8 && current <= 13)
            {
                tempImage.sprite = open.sprite;
                positionOpen.Add(i);
            }
            else if (current == 14 || current == 15 || current == 16)
            {
                tempImage.sprite = flowers.sprite;
                positionPlants.Add(i);
            }
            else if (current > 16)
            {
                tempImage.sprite = forest.sprite;
                positionForest.Add(i);
            }
            if(current == 23 || current == 8 || current == 9 || current == 12 || current == 20 || current == 29 || current == 15)
            {
                positionBirds.Add(i);
            }
            if(current == 17 || current == 18 || current == 19)
            {
                positionFlowers.Add(i);
            }
        }
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
            Debug.Log(turn);
        }
        Debug.Log("TURN " + turn);
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
            local.quests[i].startPosition = positionVillageRed;
        }
        else if (local.quests[i].startPoint == "4")
        {
            local.quests[i].startPosition = positionVillageBlue;
        }
        else if (local.quests[i].startPoint == "5")
        {
            local.quests[i].startPosition = positionVillageGreen;
        }
        switch (local.quests[i].doPoint)
        {
            case "cave":
                local.quests[i].doPosition = positionCave;
                break;
            case "villageBlue":
                local.quests[i].doPosition = positionVillageBlue;
                break;
            case "villageGreen":
                local.quests[i].doPosition = positionVillageGreen;
                break;
            case "airplane":
                local.quests[i].doPosition = positionAirplane;
                break;
            case "birds":
                local.quests[i].doPosition = positionBirds[(int)Random.Range(1f, positionBirds.Count) - 1];
                break;
            case "plants":
                local.quests[i].doPosition = positionPlants[(int)Random.Range(1f, positionPlants.Count) - 1];
                break;
            case "flowers":
                local.quests[i].doPosition = positionFlowers[(int)Random.Range(1f, positionFlowers.Count) - 1];
                break;
            case "water":
                local.quests[i].doPosition = positionLake[(int)Random.Range(1f, positionLake.Count) - 1];
                break;
        }
        if (local.quests[i].turnInPoint == "3")
        {
            local.quests[i].turnInPosition = positionVillageRed;
        }
        else if (local.quests[i].turnInPoint == "4")
        {
            local.quests[i].turnInPosition = positionVillageBlue;
        }
        else if (local.quests[i].turnInPoint == "5")
        {
            local.quests[i].turnInPosition = positionVillageGreen;
        }
    }

    public int randomTileQuest()
    {
        int returnValue = 0;
        string questName = encounteredQuest.name;
        if (questName == "Airplane")
        {
            returnValue = positionAirplane;
        }
        else if (questName == "Cave")
        {
            returnValue = positionCave;
        }
        else if (questName == "Village")
        {
            if (encounteredQuest.type == "blue")
            {
                returnValue = positionVillageBlue;
            }
            else if (encounteredQuest.type == "red")
            {
                returnValue = positionVillageRed;
            }
            else
            {
                returnValue = positionVillageGreen;
            }
        }
        else if (questName == "Water")
        {
            returnValue = positionLake[(int)Random.Range(1f, 2f)];
        }
        else if (questName == "Open")
        {
            returnValue = positionOpen[(int)Random.Range(1f, 6f)];
        }
        else if (questName == "Flowers")
        {
            returnValue = positionFlowers[(int)Random.Range(1f, 3f)];
        }
        else if (questName == "Forest")
        {
            returnValue = positionForest[(int)Random.Range(1f, 14f)];
        }
        return returnValue;
    }
}