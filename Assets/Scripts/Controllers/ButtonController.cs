﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour {
    //Main
    public GameObject main;
    public GameObject host;
    public GameObject join;
    public GameObject rules;
    public GameObject credits;
    public GameObject player;
    public GameObject lobby;
    public GameObject board;
    public GameObject error;
    public Text errorText;

    private GameObject currentMid;
    public Button backBtn;
    
    //Main
    public void switchPanel(GameObject panel)
    {
        if (currentMid == null)
        {
            currentMid = main;
        }
        if(PlayerState.name == null)
        {
            PlayerState.name = "1";
            switchPanel(player);
            return;
        }
        currentMid.gameObject.SetActive(false);
        currentMid = panel;
        panel.gameObject.SetActive(true);
    }

    //Main
    public void backButton()
    {
        if (currentMid == host || currentMid == join || currentMid == lobby  || currentMid == rules || currentMid == credits)
        {
            switchPanel(main);
        }
    }

    #region HostLogic
    public Text roomName;
    private int players;
    public Room room;

    //Host
    public void setPlayers(int i)
    {
        this.players = i;
    }
    //host
    public void CreateLobby()
    {
        room = new Room();
        if (roomName.text != "")
        {
            string roomID = SQL.Instance.getData("select roomID as result from room where name = '" + roomName.text + "' and active = 'true'");
            if (roomID != "TRUE")
            {
                toggleError("Deze kamernaam is al in gebruik");
            }
            else
            {
                room.name = roomName.text;
                room.players = players;

                room.active = "true";
                room.host = PlayerState.id;

                RoomState.name = room.name;
                RoomState.players = room.players;
                RoomState.active = "true";
                RoomState.host = PlayerState.id;

                SQL.Instance.getData("INSERT INTO `room`(`name`, `active`, `players`, `host`, `started`) VALUES ('" + room.name + "', 'true', " + room.players + "," + room.host + ", 'false')");
                int.TryParse(SQL.Instance.getData("select max(roomID) as result from room"), out RoomState.id);

                SQL.Instance.getData("UPDATE `account` SET `roomID`= " + RoomState.id + " WHERE accountID = '" + PlayerState.id + "'");
                switchPanel(lobby);
            }
        }
        else
        {
            toggleError("Vul een kamernaam in.");
        }
    }
    #endregion

    #region PlayerLogic
    //player
    public Text playerName;
    //player
    public void createPlayer()
    {
        if (playerName.text != "")
        {
            PlayerState.name = playerName.text;

            SQL.Instance.getData("INSERT INTO account (`nickName`) VALUES ('" + PlayerState.name + "')");
            int.TryParse(SQL.Instance.getData("select max(accountID) as result from account"), out PlayerState.id);
            switchPanel(main);
        }
        else
        {
            toggleError("Vul een spelernaam in.");
        }
    }
    #endregion

    #region JoinLogic
    //join
    public Text joinRoom;
    public Text errorJoin;

    public void JoinLobby()
    {
        lobbyStartBtn.gameObject.SetActive(false);
        if (joinRoom.text.Length > 0)
        {
            string resultRoom = SQL.Instance.getData("SELECT * FROM `room` WHERE name = '" + joinRoom.text + "'and active = 'true'");
            if (resultRoom != "TRUE")
            {
                Room room = JsonUtility.FromJson<Room>(resultRoom);

                if (room.name != null)
                {
                    RoomState.name = room.name;
                    RoomState.id = room.roomID;
                    RoomState.host = room.host;
                    RoomState.active = room.active;
                    RoomState.players = room.players;

                    string currentPlayers = SQL.Instance.getData("SELECT * FROM `account` WHERE roomID = " + RoomState.id);
                    string[] current = currentPlayers.Split('*');
                    if (current.Length < room.players)
                    {
                        switch (current.Length)
                        {
                            case 1:
                                RoomState.p1 = JsonUtility.FromJson<Player>(current[0]);
                                break;
                            case 2:
                                RoomState.p1 = JsonUtility.FromJson<Player>(current[0]);
                                RoomState.p2 = JsonUtility.FromJson<Player>(current[1]);
                                break;
                            case 3:
                                RoomState.p1 = JsonUtility.FromJson<Player>(current[0]);
                                RoomState.p2 = JsonUtility.FromJson<Player>(current[1]);
                                RoomState.p3 = JsonUtility.FromJson<Player>(current[2]);
                                break;
                            case 4:
                                RoomState.p1 = JsonUtility.FromJson<Player>(current[0]);
                                RoomState.p2 = JsonUtility.FromJson<Player>(current[1]);
                                RoomState.p3 = JsonUtility.FromJson<Player>(current[2]);
                                RoomState.p4 = JsonUtility.FromJson<Player>(current[3]);
                                break;
                        }
                        SQL.Instance.getData("UPDATE `account` SET `roomID` = " + RoomState.id + " WHERE accountID = '" + PlayerState.id + "'");
                        switchPanel(lobby);
                    }
                    else
                    {
                        toggleError("Deze kamer is vol");
                    }
                }
            }
            else
            {
                toggleError("Deze kamer bestaat niet.");
            }
        }
        else
        {
            toggleError("Vul een kamernaam in.");
        }
    }
    #endregion
    
    //lobby
    public void StartGame()
    {
        if (RoomState.players == RoomState.currentPlayers)
        {
            switchPanel(board);
        }
    }

    //lobby
    public void ExitGame()
    {
        lobbyStartBtn.gameObject.SetActive(true);
        if (RoomState.p1 != new Player())
        {
            SQL.Instance.getData("UPDATE `account` SET `roomID`= 0 WHERE accountID " + RoomState.p1.accountID);
        }
        if (RoomState.p2 != new Player())
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
        switchPanel(main);
    }
    //lobby

    //Game
    public void NewGameBtn(string newGameLevel)
    {
        SceneManager.LoadScene(newGameLevel);
    }

    //lobby
    public Text lobbyRoomName;
    public Text lobbyPlayerNames;
    private int lobbyPullTimer = -1;
    /// <summary>
    /// 
    /// </summary>
    public Button lobbyStartBtn;
    public Button lobbyExitBtn;

    public void Update()
    {
        if (currentMid == lobby)
        {
            if (lobbyPullTimer > 120 || lobbyPullTimer == -1)
            {
                if (RoomState.host != PlayerState.id)
                {
                    string started = SQL.Instance.getData("SELECT started as result FROM `room` where roomID = '" + RoomState.id + "'");
                    if(started == "true")
                    {
                        SceneManager.LoadScene("game");
                    }
                }
                lobbyPullTimer = 0;

                string currentPlayers = SQL.Instance.getData("SELECT * FROM `account` WHERE roomID = '" + RoomState.id + "'");
                Debug.Log(currentPlayers);
                string[] current = currentPlayers.Split('*');
                RoomState.currentPlayers = current.Length;
                RoomState.p1 = new Player();
                RoomState.p2 = new Player();
                RoomState.p3 = new Player();
                RoomState.p4 = new Player();

                if (current.Length > 0)
                {
                    RoomState.p1 = JsonUtility.FromJson<Player>(current[0]);
                    Debug.Log("p1 = " + RoomState.p1.nickName);
                }
                if (current.Length > 1)
                {
                    RoomState.p2 = JsonUtility.FromJson<Player>(current[1]);
                    Debug.Log("p1 = " + RoomState.p2.nickName);
                }
                if (current.Length > 2)
                {
                    RoomState.p3 = JsonUtility.FromJson<Player>(current[2]);
                    Debug.Log("p1 = " + RoomState.p3.nickName);
                }
                if (current.Length > 3)
                {
                    RoomState.p4 = JsonUtility.FromJson<Player>(current[3]);
                    Debug.Log("p1 = " + RoomState.p4.nickName);
                }

                lobbyRoomName.text = RoomState.name + " (" + RoomState.currentPlayers + " / " + RoomState.players + ")";
                lobbyPlayerNames.text = RoomState.p1.nickName + "\n" +
                    RoomState.p2.nickName + "\n" +
                    RoomState.p3.nickName + "\n" +
                    RoomState.p4.nickName;
            }
        }
        lobbyPullTimer++;
    }

    public void goHome()
    {
        if (currentMid != null)
        {
            switchPanel(main);
        }
    }

    public void toggleSound()
    {
        if (PlayerState.sound)
        {
            PlayerState.sound = false;
        }
        else
        {
            PlayerState.sound = true;
        }
    }

    public void toggleError(string description)
    {
        if (error.activeSelf)
        {
            error.SetActive(false);
        }
        else
        {
            error.SetActive(true);
            errorText.text = description;
        }
    }
}