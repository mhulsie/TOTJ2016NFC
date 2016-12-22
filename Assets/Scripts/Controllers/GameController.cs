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

    // Use this for initialization
    void Start()
    {
        AndroidNFCReader.enableBackgroundScan();
        AndroidNFCReader.ScanNFC("GameController", "OnMove");

        currentMid = GameMid;
        incidents.list = new List<Incident>();
        local = new LocalLibrary();

        PlayerState.energy = 15;

        if (RoomState.host == PlayerState.id)
        {
            UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);

            foreach (Incident item in local.incidents)
            {
                item.tile = (int)Random.Range(1f, 30f);
            }
            incidents.list = local.incidents;
            SQL.Instance.getData("UPDATE room set started = 'true' where roomID = " + RoomState.id);
            SQL.Instance.getData("UPDATE `board` SET `incidents`='" + JsonUtility.ToJson(incidents) + "' WHERE boardID = " + local.board.boardID);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Keep pulling to see if its my turn
        // Dont pull if its my turn
        Debug.Log(currentTurn);
        if (pullTimer > 120 || pullTimer == -1)
        {
            pullTimer = 0;
            if (myTurn != true)
            {
                int.TryParse(SQL.Instance.getData("SELECT turn as result FROM board WHERE boardID = " + local.board.boardID), out currentTurn);
                if (PlayerState.id == local.players.list[currentTurn].accountID)
                {
                    myTurn = true;
                    switchPanel(MoveAction);
                }
            }
        }
        else
        {
            pullTimer++;
        }

        //Start action panel sequence

        //If last player, animal dance
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
            AnimalDance();
            currentTurn = 0;
        }
        switchPanel(GameMid);
        myTurn = false;
        incidents.list = local.incidents;
        string incidentsJson = JsonUtility.ToJson(incidents);
        SQL.Instance.getData("UPDATE board set turn = " + currentTurn + ", incidents = '" + incidentsJson + "', players = '" + JsonUtility.ToJson(local.players) + "'  where roomID = " + RoomState.id);
    }

    public void setTrap()
    {

    }

    public void getQuest()
    {

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

                // playerTile = local.players.list[currentTurn].currentTile.ToString();
                // int currentIndex = local.layout.layout.IndexOf(playerTile);

                debugtestText.text = " 2nd currentposition = " + currentPosition;

                //Calculate possible tiles

                if (PlayerState.movedIncorrect)
                {
                    if (scanPosition == currentPosition)
                    {
                        PlayerState.movedIncorrect = false;
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
                    if((currentPosition + 1) % 6 != 0)
                    {
                        if (currentPosition + 1 == scanPosition)
                        {
                            debugtestText.text = "goed +1";
                            //local.players.list[currentTurn].currentPosition++;
                            PlayerState.validMove = true;
                        }
                    }
                    /*if (currentPosition < 29 && (currentPosition + 1 % 6 != 0))
                    {
                        if (currentPosition + 1 == scanPosition)
                        {
                            debugtestText.text = "goed +1";
                            //local.players.list[currentTurn].currentPosition++;
                            PlayerState.validMove = true;
                        }
                    }*/
                    if (currentPosition < 24)
                    {
                        if (currentPosition + 6 == scanPosition)
                        {
                            debugtestText.text = "goed +6";
                            //local.players.list[currentTurn].currentPosition += 6;
                            PlayerState.validMove = true;
                        }
                    }
                    if ((currentPosition + 1) % 6 != 1)
                    {
                        if (currentPosition - 1 == scanPosition)
                        {
                            debugtestText.text = "goed -1";
                            //local.players.list[currentTurn].currentPosition--;
                            PlayerState.validMove = true;
                        }
                    }
                    /*if (currentPosition > 0 && (currentPosition + 1 % 6 != 1)) 
                    {
                        if (currentPosition - 1 == scanPosition)
                        {
                            debugtestText.text = "goed -1";
                            //local.players.list[currentTurn].currentPosition--;
                            PlayerState.validMove = true;
                        }
                    }*/
                    if (currentPosition > 5)
                    {
                        if (currentPosition - 6 == scanPosition)
                        {
                            debugtestText.text = "goed -6";
                            //local.players.list[currentTurn].currentPosition -= 6;
                            PlayerState.validMove = true;
                        }
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
                        PlayerState.energy--;
                        PlayerState.validMove = false;

                        //check incidents
                        /*Incident currentIncident = null;
                        //check voor incident

                        debugtestText.text = "goed 2";
                        foreach (Incident i in local.incidents)
                        {
                            if (scan == i.tile)
                            {
                                currentIncident = i;
                            }
                        }

                        debugtestText.text = "goed 3";

                        // If player stepped on an incident
                        if (currentIncident != null)
                        {
                            if (currentIncident.name == "Trap")
                            {
                                endTurn();
                            }
                            else if (currentIncident.name == "Elephant")
                            {
                                debugtestText.text = "OLIFANTEN";
                                //switchPanel(DialogueMid);
                                //endturn in dialoguemid
                            }
                        }
                        else
                        {*/
                        //}

                        //debugtestText.text = "goed 4";
                        // if no incident player can move again
                    }
                }
            }
            debugtestText.text = "correcte beweging, ga nog eens";
        }


    public void AnimalDance()
    {
        foreach (Incident i in local.incidents)
        {
            if (i.name == "Elephant")
            {
                int randomTile;
                int.TryParse(UnityEngine.Random.Range(1f, 30f).ToString("0"), out randomTile);

                randomTile = 4;
                i.tile = randomTile;

            }
        }
    }
}