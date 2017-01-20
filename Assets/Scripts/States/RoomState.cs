/// <summary>
/// This is the roomstate class. It is used to locally store the relevant room properties
/// </summary>
public static class RoomState
{
    public static int id;               //Room ID
    public static string name;          //Room name
    public static string active;        //State of room
    public static int players;          //Number of players needed in this room
    public static int host;             //Host of room
    public static Player p1;            //Player 1
    public static Player p2;            //Player 2
    public static Player p3;            //Player 3
    public static Player p4;            //Player 4


    public static int currentPlayers;   //Number of current players in this room
}