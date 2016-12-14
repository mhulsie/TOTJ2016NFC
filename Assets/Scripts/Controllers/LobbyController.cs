using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Model;
using Assets.Scripts.States;
using UnityEngine.SceneManagement;

public class LobbyController : MonoBehaviour {

    public Text roomName;
    public Text playerNames;
    private int pullTimer;

    public Button startBtn;
    public Button ExitBtn;

	// Use this for initialization
	void Start () {
        pullTimer = -1;
        startBtn.gameObject.SetActive(false);
        if (RoomState.host != PlayerState.id)
        {
            startBtn.gameObject.SetActive(false);
            //startBtn.gameObject.SetActive(true);
            Destroy(ExitBtn.gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
        if(pullTimer > 120 || pullTimer == -1)
        {
            pullTimer = 0;

            string currentPlayers = SQL.Instance.getData("SELECT * FROM `account` WHERE roomID = '" + RoomState.id + "'");
            string[] current = currentPlayers.Split('*');
            RoomState.currentPlayers = current.Length;
            Player defaultPlayer = new Player();
            RoomState.p1 = new Player();
            RoomState.p2 = new Player();
            RoomState.p3 = new Player();
            RoomState.p4 = new Player();

            if (current.Length > 0)
            {
                RoomState.p1 = JsonUtility.FromJson<Player>(current[0]);
            }
            if (current.Length > 1)
            {
                RoomState.p2 = JsonUtility.FromJson<Player>(current[1]);
            }
            if (current.Length > 2)
            {
                RoomState.p3 = JsonUtility.FromJson<Player>(current[2]);
            }
            if (current.Length > 3)
            {
                RoomState.p4 = JsonUtility.FromJson<Player>(current[3]);
            }
                
            Debug.Log(RoomState.p1.nickName);
            Debug.Log(RoomState.p2.nickName);
            Debug.Log(RoomState.p3.nickName);
            Debug.Log(RoomState.p4.nickName);

            roomName.text = RoomState.name + " (" + RoomState.currentPlayers + " / " + RoomState.players + ")";
            playerNames.text = RoomState.p1.nickName + "\n" + 
                RoomState.p2.nickName + "\n" + 
                RoomState.p3.nickName + "\n" + 
                RoomState.p4.nickName;
        }
        pullTimer++;
    }

    public void StartGame()
    {
        if(RoomState.players == RoomState.currentPlayers)
        {
            SceneManager.LoadScene("game");
        }
    }

    public void ExitGame()
    {
        if(RoomState.p2 != new Player())
        {
            SQL.Instance.getData("UPDATE `account` SET `roomID`= 0 WHERE accountID " + RoomState.p2.accountID);
        }
        if (RoomState.p3 != new Player())
        {
            SQL.Instance.getData("UPDATE `account` SET `roomID`= 0 WHERE accountID " + RoomState.p3.accountID);
        }
        if (RoomState.p4 != new Player())
        {
            SQL.Instance.getData("UPDATE `account` SET `roomID`= 0 WHERE accountID " + RoomState.p4.accountID);
        }

    }
}