
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
    
    // Use this for initialization
    void Start ()
    {
        currentMid = MoveAction;
        incidents.list = new List<Incident>();
        LocalLibrary local = new LocalLibrary();
        Debug.Log(local.incidents[0].name);
        UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);

        foreach (Incident item in local.incidents)
        {
            item.tile = (int) Random.Range(1f, 30f);
            Debug.Log(item.tile);
        }
        incidents.list = local.incidents;
        SQL.Instance.getData("UPDATE `board` SET `incidents`='" + JsonUtility.ToJson(incidents) + "' WHERE boardID = " + local.board.boardID);
	}
	
	// Update is called once per frame
	void Update () {
        // Keep pulling to see if its my turn
        // Dont pull if its my turn

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
