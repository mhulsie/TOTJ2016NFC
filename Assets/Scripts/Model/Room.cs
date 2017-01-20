/// <summary>
/// This is the room model, players can host and join a room to create a new game
/// Except for player models, variables are corresponding to database table
/// </summary>
public class Room
{
    public int roomID;              //Room ID
    public string name;             //Name of room, players can join using this name as a reference
    public string active;           //Room can be active of non active
    public int players;             //Number of players needed for this game to start
    public int host;                //One of the players is the host
    public Player p1;               //Player 1
    public Player p2;               //Player 2
    public Player p3;               //Player 3
    public Player p4;               //Player 4
    public string started;          //True if game is started, false when not
}

