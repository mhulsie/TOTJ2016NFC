
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public int currentTurn;
    public Treasure treasure;

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
        currentMid = GameMid;
        incidents.list = new List<Incident>();
        local = new LocalLibrary();
        Debug.Log(local.incidents[0].name);
        //Debug.Log(local.board.playersList[currentTurn].accountID);
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
                Debug.Log("SELECT turn as result FROM board WHERE boardID = " + local.board.boardID);
                int.TryParse(SQL.Instance.getData("SELECT turn as result FROM board WHERE boardID = " + local.board.boardID), out currentTurn);
                Debug.Log("Currenturn before turncheck :       " + currentTurn);
                Debug.Log("database player id : " + local.players.list[currentTurn].accountID);
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
        if(currentTurn == 2)
        {
            currentTurn = 0;
        }
        Debug.Log("currentturn in end func     " + currentTurn);
        switchPanel(GameMid);
        myTurn = false;
        Debug.Log("update turn    " + "UPDATE board set turn = " + currentTurn + " where roomID = " + RoomState.id);
        SQL.Instance.getData("UPDATE board set turn = " + currentTurn + " where roomID = " + RoomState.id);

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

}
