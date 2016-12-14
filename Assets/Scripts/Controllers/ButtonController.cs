using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Assets.Scripts.Model;
using Assets.Scripts.States;

public class ButtonController : MonoBehaviour {
    //Main
    public GameObject main;
    public GameObject host;
    public GameObject join;
    public GameObject rules;
    public GameObject settings;
    public GameObject credits;
    public GameObject player;
    public GameObject lobby;
    public GameObject board;

    private GameObject currentMid;
    public Button backBtn;

    //Main
    public void switchPanel(GameObject panel)
    {
        if(currentMid == null)
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
        if(currentMid == host || currentMid == join || currentMid == lobby || currentMid == settings || currentMid == rules || currentMid == settings)
        {
            switchPanel(main);
        }
    }
    
    //Host
    public Text roomName;
    private int players;
    public Room room;
    public Text error;

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
                error.text = "Deze kamernaam is al in gebruik";
            }
            else
            {
                room.name = roomName.text;
                room.players = players;

                room.active = "true";
                room.host = PlayerState.id;
                room.host = 1;

                RoomState.name = room.name;
                RoomState.players = room.players;
                RoomState.active = "true";
                RoomState.host = PlayerState.id;

                SQL.Instance.getData("INSERT INTO `room`(`name`, `active`, `players`, `host`, `started`) VALUES ('" + room.name + "', 'true', " + room.players + "," + room.host + ", 'false')");
                int.TryParse(SQL.Instance.getData("select max(roomID) as result from room"), out RoomState.id);

                SQL.Instance.getData("UPDATE `account` SET `roomID`= " + RoomState.id + " WHERE accountID = '" + PlayerState.id + "'");
                ButtonController BC = new ButtonController();
                switchPanel(lobby);
            }
        }
    }

    //player
    public int playerHat;
    public int playerVehicle;
    public Text playerName;
    //player
    public void toggleVehicleSelection(int i)
    {
        switch (i)
        {
            case 1:
                playerVehicle = 1;
                break;
            case 2:
                playerVehicle = 2;
                break;
            case 3:
                playerVehicle = 3;
                break;
        }
    }
    //player
    public void togglehatSelection(int i)
    {
        switch (i)
        {
            case 1:
                playerHat = 1;
                break;
            case 2:
                playerHat = 2;
                break;
            case 3:
                playerHat = 3;
                break;
        }
    }
    //player
    public void createPlayer()
    {
        if (playerName.text != "")
        {
            playerHat = 1;
            playerVehicle = 1;
            PlayerState.name = playerName.text;
            PlayerState.hat = playerHat;
            PlayerState.vehicle = playerVehicle;

            SQL.Instance.getData("INSERT INTO account (`nickName`, `vehicle`, `hat`) VALUES ('" + PlayerState.name + "'," + PlayerState.vehicle + "," + PlayerState.hat + ")");
            int.TryParse(SQL.Instance.getData("select max(accountID) as result from account"), out PlayerState.id);
            switchPanel(main);
        }
    }

    //join
    public Text joinRoom;
    public Text errorJoin;

    public void JoinLobby()
    {
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
                    Debug.Log("amount f players in room " + room.players);

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
                        error.text = "Deze kamer is vol.";
                    }
                }
            }
            else
            {
                error.text = "Deze kamer bestaat niet.";
            }
        }
        else
        {
            error.text = "Vul een kamernaam in!";
        }
    }


    //lobby
    public void StartGame()
    {
        if (RoomState.players == RoomState.currentPlayers)
        {
            SceneManager.LoadScene("game");
        }
    }
    //lobby
    public void ExitGame()
    {
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
    //

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
        if(currentMid == lobby)
        {
            if (lobbyPullTimer > 120 || lobbyPullTimer == -1)
            {
                lobbyPullTimer = 0;

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

                lobbyRoomName.text = RoomState.name + " (" + RoomState.currentPlayers + " / " + RoomState.players + ")";
                lobbyPlayerNames.text = RoomState.p1.nickName + "\n" +
                    RoomState.p2.nickName + "\n" +
                    RoomState.p3.nickName + "\n" +
                    RoomState.p4.nickName;
            }
        }
        lobbyPullTimer++;
    }
}
