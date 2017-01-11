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
    public GameObject DialogueMid;
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
    public int villageRed;
    public int villageBlue;
    public int villageGreen;
    public Quest encounteredQuest;


    // Use this for initialization
    void Start()
    {
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

        MapMid.SetActive(true);
        setMap();
        MapMid.SetActive(false);

        int currentEnergy = PlayerState.energy;
        GameObject newImageObject = GameObject.Find("Energy" + currentEnergy);
        Image newImage = newImageObject.GetComponent<Image>();

        placeholderImage.sprite = newImage.sprite;

        defineCodeQuests();
    }

    // Update is called once per frame
    void Update()
    {
        // Keep pulling to see if its my turn
        // Dont pull if its my turn
        Debug.Log(currentTurn);
        Debug.Log(local.players.list[currentTurn].accountID);
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
                    switchPanel(MoveAction);
                }
            }
        }
        else
        {
            pullTimer++;
        }

        DisplayEnergy();

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
    public void switchPanel(GameObject panel)
    {
        currentMid.gameObject.SetActive(false);
        currentMid = panel;
        panel.gameObject.SetActive(true);
    }

    public void endTurn()
    {
        currentTurn++;
        if (currentTurn == local.players.list.Count)
        {
            Debug.Log("DIERENDANS");
            switchPanel(GameMid);
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

    public void getQuest()
    {
        //3 blue
        if(local.players.list[currentTurn].currentPosition == villageBlue)
        {
            encounteredQuest = PlayerState.blueQuest;
        }
        //4 red
        else if (local.players.list[currentTurn].currentPosition == villageRed)
        {
            encounteredQuest = PlayerState.redQuest;
        }
        //5 green
        else if (local.players.list[currentTurn].currentPosition == villageGreen)
        {
            encounteredQuest = PlayerState.greenQuest;
        }
        if(encounteredQuest != null)
        {
            switchPanel(IncidentPopup);
        }
    }

    public void doQuest()
    {

    }
    public void reportQuest()
    {

    }

    public void steal()
    {

    }

    public void digTreasure()
    {

    }
    #endregion

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

                            switchPanel(IncidentPopup);
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
                switchPanel(MoveAction);
            }
            else
            {
                switchPanel(GameMid);
            }
            encounteredIncident = null;
        }
        else if(encounteredQuest != null)
        {
            title.text = encounteredQuest.title;
            description.text = encounteredQuest.description;
            incidentBtn.text = encounteredQuest.button;

            int tempQuest;
            if(int.TryParse(encounteredQuest.result, out tempQuest))
            {
                if (encounteredQuest == PlayerState.blueQuest)
                {
                    PlayerState.blueQuest = local.quests[tempQuest];
                }
                else if (encounteredQuest == PlayerState.redQuest)
                {
                    PlayerState.redQuest = local.quests[tempQuest];
                }
                else if (encounteredQuest == PlayerState.greenQuest)
                {
                    PlayerState.greenQuest = local.quests[tempQuest];
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
            int current = int.Parse(local.layout.layout[i - 1]);
            if (current == 1)
            {
                tempImage.sprite = city.sprite;
                debugtestText.text += i;
            }
            else if (current == 2)
            {
                tempImage.sprite = cave.sprite;
            }
            else if (current == 3 || current == 4 || current == 5)
            {
                tempImage.sprite = village.sprite;
                if(current == 3)
                {
                    villageBlue = i - 1;
                } else if(current == 4)
                {
                    villageRed = i - 1;
                } else if(current == 5)
                {
                    villageGreen = i - 1;
                }
            }
            else if (current == 6 || current == 7)
            {
                tempImage.sprite = lake.sprite;
            }
            else if (current >= 8 && current <= 13)
            {
                tempImage.sprite = open.sprite;
            }
            else if (current == 14 || current == 15 || current == 16)
            {
                tempImage.sprite = flowers.sprite;
            }
            else if (current > 16)
            {
                tempImage.sprite = forest.sprite;
            }
        }
    }

    public void defineCodeQuests()
    {
        PlayerState.blueQuest = local.blueQuests[(int)Random.Range(1f, local.blueQuests.Count) -1];
        PlayerState.redQuest = local.redQuests[(int)Random.Range(1f, local.redQuests.Count) - 1];
        PlayerState.greenQuest = local.greenQuests[(int)Random.Range(1f, local.greenQuests.Count) - 1];        
    }
}