﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public int currentTurn = 0;
    public Treasure treasure;


    [System.Serializable]
    public struct playerWrapper { public List<Player> list; };
    public playerWrapper players;

    [System.Serializable]
    public struct incidentWrapper { public List<Incident> list; };
    public incidentWrapper incidents;

    public int pullTimer = -1;
    public LocalLibrary local;
    
    // Use this for initialization
    void Start ()
    {
        incidents.list = new List<Incident>();
        local = new LocalLibrary();
        Debug.Log(local.incidents[0].name);
        //Debug.Log(local.board.playersList[currentTurn].accountID);
        foreach (Player item in local.players.list)
        {
            Debug.Log(item.accountID);
        }
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

        //Keep pulling to see if its my turn
        // Dont pull if its my turn
        if(pullTimer > 120 || pullTimer == -1)
        {
            string turn = SQL.Instance.getData("SELECT 'turn' FROM 'board' WHERE boardID = " + local.board.boardID);
            if(PlayerState.id == local.players.list[currentTurn].accountID)
            {
                Debug.Log("IK BEN AAN DE BEURT HEUY");
            }
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
}
