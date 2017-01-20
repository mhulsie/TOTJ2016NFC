using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour {
    /// <summary>
    /// The panels for each part of the main menu
    /// </summary>
    public GameObject main;
    public GameObject host;
    public GameObject join;
    public GameObject rules;
    public GameObject credits;
    public GameObject player;
    public GameObject lobby;
    public GameObject board;
    public GameObject error;

    /// <summary>
    /// The text element for the encountered error
    /// </summary>
    public Text errorText;

    /// <summary>
    /// The currentMid that is shown now
    /// </summary>
    private GameObject currentMid;
    /// <summary>
    /// The targeted panel where the player wants to go
    /// </summary>
    private GameObject target;

    /// <summary>
    /// The buttons in the top and bottombar
    /// </summary>
    public Button backBtn;
    public Button homeBtn;

    /// <summary>
    /// The edges around the player selection in the HostMid
    /// </summary>
    public Image p2Edge;
    public Image p3Edge;
    public Image p4Edge;

    /// <summary>
    /// Called when a player wants to switch panels
    /// </summary>
    /// <param name="panel">The panel where the player wants to go</param>
    public void switchPanel(GameObject panel)
    {
        //If there is no player yet, send player to PlayerMid
        if (PlayerState.name == null || PlayerState.name == "")
        {
            target = panel;
            player.gameObject.SetActive(true);
            return;
        }

        //If there is no currentMid, currentmid = main
        if (currentMid == null)
        {
            currentMid = main;
        }

        //Set the top and bottom button to their default
        backBtn.gameObject.SetActive(true);
        homeBtn.gameObject.SetActive(true);
        //Disable the currentmid
        currentMid.gameObject.SetActive(false);
        currentMid = panel;
        //Set the new  mid active
        panel.gameObject.SetActive(true);
        //If in the player panel or main panel, disable top and bot buttons
        if (panel == main || panel == player)
        {
            backBtn.gameObject.SetActive(false);
            homeBtn.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Sets the screen back to main menu
    /// </summary>
    public void backButton()
    {
        if (currentMid == host || currentMid == join || currentMid == lobby  || currentMid == rules || currentMid == credits)
        {
            switchPanel(main);
        }
    }
    
    /// <summary>
    /// The host variables
    /// </summary>
    public Text roomName;
    private int players = 2;
    public Room room;

    /// <summary>
    /// Called when an amount of players is selected.
    /// Sets the correct edge around the button
    /// </summary>
    /// <param name="i">The amount of players</param>
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
    
    /// <summary>
    /// Creates the lobby and checks if the given input for roomname is correct
    /// </summary>
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
    
    /// <summary>
    /// The playername
    /// </summary>
    public Text playerName;

    /// <summary>
    /// Creates the player and sends the player on to the next panel
    /// </summary>
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
    
    //join
    public Text joinRoom;

    /// <summary>
    /// Join a lobby. Called from the joinMid.
    /// Checks if the player entered a correct roomname and if so adds the player to that lobby
    /// </summary>
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
                    //Set the roomstate vars for later use
                    RoomState.name = room.name;
                    RoomState.id = room.roomID;
                    RoomState.host = room.host;
                    RoomState.active = room.active;
                    RoomState.players = room.players;

                    //Update the panel to show correct data
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
    
    /// <summary>
    /// Starts the game if the amount of players is correct
    /// </summary>
    public void StartGame()
    {
        if (RoomState.players == RoomState.currentPlayers)
        {
            switchPanel(board);
        }
    }

    /// <summary>
    /// Exit the game
    /// </summary>
    public void ExitGame()
    {
        switchPanel(main);
    }

    // The lobby vars
    public Text lobbyRoomName;
    public Text lobbyPlayerNames;
    private int lobbyPullTimer = -1;
    public Button lobbyStartBtn;

    /// <summary>
    /// The update loop in the lobby
    /// </summary>
    public void Update()
    {
        //Only trigger if currently in the lobby
        if (currentMid == lobby)
        {
            if (lobbyPullTimer > 120 || lobbyPullTimer == -1)
            {
                //If not the host, check if game started
                if (RoomState.host != PlayerState.id)
                {
                    string started = SQL.Instance.executeQuery("SELECT started as result FROM `room` where roomID = '" + RoomState.id + "'");
                    if(started == "true")
                    {
                        SceneManager.LoadScene("game");
                    }
                }
                //Reset timer
                lobbyPullTimer = 0;

                //Get the current players and update roomstate
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

                //Update the lobby data in the panel
                lobbyRoomName.text = RoomState.name + " (" + RoomState.currentPlayers + " / " + RoomState.players + ")";
                lobbyPlayerNames.text = RoomState.p1.nickName + "\n" +
                    RoomState.p2.nickName + "\n" +
                    RoomState.p3.nickName + "\n" +
                    RoomState.p4.nickName;
            }
        }
        lobbyPullTimer++;
    }

    /// <summary>
    /// Go back to the main menu
    /// Called from the homebtn in the bottom of the screen
    /// </summary>
    public void goHome()
    {
        if (currentMid != null)
        {
            switchPanel(main);
        }
    }

    /// <summary>
    /// Toggle sound on and off
    /// Called from the button in the bottom
    /// </summary>
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

    /// <summary>
    /// Enables and disables the errorPopUp
    /// 
    /// </summary>
    /// <param name="description"></param>
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