/// <summary>
/// This is the playerstate class. It is used to store the properties only relevant to this player.
/// Frequenlty used to determine gamelogic in GameController with variables like movedIncorrect and validMove
/// </summary>
public static class PlayerState
{
    public static int id;                               //Player ID
    public static string name;                          //Player name
    public static string vehicle = "Red";               //Color of vehicle
    public static string hat = "Gray";                  //Color of hat
    public static int energy = 15;                      //Energy level
    public static bool movedIncorrect = false;          //True if player has moved incorrect,
    public static bool validMove = false;               //True if player made a valid move
    public static Quest redQuest;                       //Current quest for the red code
    public static Quest blueQuest;                      //Current quest for the blue code
    public static Quest greenQuest;                     //Current quest for the green code
    public static Quest energyQuest;                    //Current quest to collect energy
    public static int clues = 0;                        //Number of clues collected
    public static bool sound = true;                    //Switch for sound or no sound in game
    public static bool activeTrap;                      //Position in layout where this player set a trap for other players
}

