using UnityEngine;
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
    private GameObject target;

    public Button backBtn;
    public Button homeBtn;

    public Image p2Edge;
    public Image p3Edge;
    public Image p4Edge;

    //Main
    public void switchPanel(GameObject panel)
    {
        //string cipherData = "1gw0Ap0k4iHpwl8S7ZFjAJGLZnX4dToR2h0T9oI/OL9dhM2IB1jSqOKjjyadw4kqTdcqmlDFXHa1CKI3fcRXbQ==";
        //Debug.Log(SQL.Encrypt("UPDATE `board` SET `boardID`=[value-1],`active`=[value-2],`roomID`=[value-3],`layout`=[value-4],`turn`=[value-5],`players`=[value-6],`treasure`=[value-7],`incidents`=[value-8],`animals`=[value-9] WHERE 1")); 
        if (PlayerState.name == null || PlayerState.name == "")
        {
            target = panel;
            player.gameObject.SetActive(true);
            return;
        }

        if (currentMid == null)
        {
            currentMid = main;
        }
        backBtn.gameObject.SetActive(true);
        homeBtn.gameObject.SetActive(true);
        currentMid.gameObject.SetActive(false);
        currentMid = panel;
        panel.gameObject.SetActive(true);
        if (panel == main || panel == player)
        {
            backBtn.gameObject.SetActive(false);
            homeBtn.gameObject.SetActive(false);
        }
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
    private int players = 2;
    public Room room;

    //Host
    public void setPlayers(int i)
    {
        this.players = i;
        p2Edge.gameObject.SetActive(false);
        p3Edge.gameObject.SetActive(false);
        p4Edge.gameObject.SetActive(false);
        if (i == 2)
        {
            p2Edge.gameObject.SetActive(true);
        }
        else if(i == 3)
        {
            p3Edge.gameObject.SetActive(true);
        }
        else if (i == 4)
        {
            p4Edge.gameObject.SetActive(true);
        }
    }
    //host
    public void CreateLobby()
    {
        room = new Room();
        if (roomName.text != "")
        {
            string roomID = SQL.Instance.executeQuery("select roomID as result from room where name = '" + roomName.text + "' and active = 'true'");
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

                SQL.Instance.executeQuery("INSERT INTO `room`(`name`, `active`, `players`, `host`, `started`) VALUES ('" + room.name + "', 'true', " + room.players + "," + room.host + ", 'false')");
                int.TryParse(SQL.Instance.executeQuery("select max(roomID) as result from room"), out RoomState.id);

                SQL.Instance.executeQuery("UPDATE `account` SET `roomID`= " + RoomState.id + " WHERE accountID = '" + PlayerState.id + "'");
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
        PlayerState.name = playerName.text;
        if (playerName.text == "")
        {
            toggleError("Vul een spelernaam in.");
        }
        else
        {
            SQL.Instance.executeQuery("INSERT INTO account (`nickName`) VALUES ('" + PlayerState.name + "')");
            int.TryParse(SQL.Instance.executeQuery("select max(accountID) as result from account"), out PlayerState.id);
            switchPanel(target);
            player.gameObject.SetActive(false);
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
            string resultRoom = SQL.Instance.executeQuery("SELECT * FROM `room` WHERE name = '" + joinRoom.text + "'and active = 'true'");
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

                    string currentPlayers = SQL.Instance.executeQuery("SELECT * FROM `account` WHERE roomID = " + RoomState.id);
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
                        SQL.Instance.executeQuery("UPDATE `account` SET `roomID` = " + RoomState.id + " WHERE accountID = '" + PlayerState.id + "'");
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
            SQL.Instance.executeQuery("UPDATE `account` SET `roomID`= 0 WHERE accountID " + RoomState.p1.accountID);
        }
        if (RoomState.p2 != new Player())
        {
            SQL.Instance.executeQuery("UPDATE `account` SET `roomID`= 0 WHERE accountID " + RoomState.p2.accountID);
        }
        if (RoomState.p3 != new Player())
        {
            SQL.Instance.executeQuery("UPDATE `account` SET `roomID`= 0 WHERE accountID " + RoomState.p3.accountID);
        }
        if (RoomState.p4 != new Player())
        {
            SQL.Instance.executeQuery("UPDATE `account` SET `roomID`= 0 WHERE accountID " + RoomState.p4.accountID);
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
                    string started = SQL.Instance.executeQuery("SELECT started as result FROM `room` where roomID = '" + RoomState.id + "'");
                    if(started == "true")
                    {
                        SceneManager.LoadScene("game");
                    }
                }
                lobbyPullTimer = 0;

                string currentPlayers = SQL.Instance.executeQuery("SELECT * FROM `account` WHERE roomID = '" + RoomState.id + "'");
                string[] current = currentPlayers.Split('*');
                RoomState.currentPlayers = current.Length;
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