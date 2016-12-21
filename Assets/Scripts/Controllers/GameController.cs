using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
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
    
    // Use this for initialization
    void Start ()
    {
        AndroidNFCReader.enableBackgroundScan();
        AndroidNFCReader.ScanNFC("GameController", "Harry");
        currentMid = GameMid;
        incidents.list = new List<Incident>();
        local = new LocalLibrary();
        foreach (Player item in local.players.list)
        {
            Debug.Log(item.accountID);
        }
        Debug.Log(local.players.list[currentTurn].nickName);
        if(RoomState.host == PlayerState.id)
        {
            UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);

            foreach (Incident item in local.incidents)
            {
                item.tile = (int)Random.Range(1f, 30f);
                Debug.Log(item.tile);
            }
            incidents.list = local.incidents;
            SQL.Instance.getData("UPDATE room set started = 'true' where roomID = " + RoomState.id);
            SQL.Instance.getData("UPDATE `board` SET `incidents`='" + JsonUtility.ToJson(incidents) + "' WHERE boardID = " + local.board.boardID);
        }
    }
	
	// Update is called once per frame
	void Update () {
        // Keep pulling to see if its my turn
        // Dont pull if its my turn
        Debug.Log(currentTurn);
        if(pullTimer > 120 || pullTimer == -1)
        {
            pullTimer = 0;
            if(myTurn != true)
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
        if(currentTurn == local.players.list.Count - 1)
        {
            Debug.Log("DIERENDANS");
            AnimalDance();            
            currentTurn = 0;
        }
        switchPanel(GameMid);
        myTurn = false;
        incidents.list = local.incidents;
        string incidentsJson = JsonUtility.ToJson(incidents);
        SQL.Instance.getData("UPDATE board set turn = " + currentTurn + ", incidents = " + incidentsJson + "where roomID = " + RoomState.id);

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

    public void Harry(string result)
    {
        nfcTestText.text = " beginscan";
        if(myTurn == false)
        {
            nfcTestText.text = " niet mijn beurd";
            return;
        }

        int scan;
        int.TryParse(result, out scan);

        nfcTestText.text = " scan = " + scan;
        Incident currentIncident = null;
        int count = 0;
        //check voor incident
        foreach (Incident i in local.incidents)
        {
            count++;
            if(scan == i.tile)
            {
                currentIncident = i;
            }
        }
        nfcTestText.text = "asdlkjasdlkajsda     " + count;
        nfcTestText.text += currentIncident.name;
        currentIncident.name = "Elephant";
        if (currentIncident != null)
        {
            if (currentIncident.name == "Trap")
            {
                endTurn();
            }
            else if (currentIncident.name == "Elephant")
            {
                switchPanel(DialogueMid);
                testText.text = "olievefant";
                //endTurn();
            }
        }
    }

    public void AnimalDance()
    {
        foreach(Incident i in local.incidents)
        {
            if(i.name == "Elephant")
            {
                int randomTile;
                int.TryParse(UnityEngine.Random.Range(1f, 30f).ToString("0"), out randomTile);

                randomTile = 4;
                i.tile = randomTile;

            }
        }
    }
}
