/// <summary>
/// This is the player model, it holds the information for the player.
/// It is made serializable so  a list of players can be stored in a wrapper, so Json can encode it
/// </summary>
[System.Serializable]
public class Player
{
    public int accountID;               //Account ID, corresponding to column in database
    public string nickName = "";        //Nickname of player, corresponding to column in database
    public int vehicle;                 //Vehicle color ID
    public int hat;                     //Hat color ID
    public int currentTile;             //Current tile id of players position
    public int currentPosition = 31;    //Current position in the layout of the board
}