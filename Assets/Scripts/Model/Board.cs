using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// This class is the model for the board
/// </summary>
public class Board
{
    public int boardID;         //Board ID
    public string active;       //State of board
    public int roomID;          //Corresponding room
    public int turn;            //Current turn
    public string layout;       //Jsonstring for layout list
    public string players;      //Jsonstring for player list
    public string incidents;    //Jsonstring for incident list
    public string treasure;     //Jsonstring for treasure
    public Treasure treasureT;  //Object variant of treasure

    /// <summary>
    /// This is the layout wrapper holding a list with the layout of the board.
    /// This is done because Json cannot directly convert a list.
    /// </summary>
    [Serializable]
    public struct layoutWrapper { public List<string> layout; };
    public layoutWrapper wrapper;

    /// <summary>
    /// This is the player wrapper holding a list of all players on this board.
    /// This is done because Json cannot directly convert a list.
    /// </summary>
    [Serializable]
    public struct playerWrapper { public List<Player> list; };
    public playerWrapper playerwrap;

    /// <summary>
    /// This the incident wrapper holding a list of all incidents on this board.
    /// This is done because Json cannot directly convert a list.
    /// </summary>
    [Serializable]
    public struct incidentWrapper { public List<Incident> list; };
    public incidentWrapper incidentwrap;

    /// <summary>
    /// Constructor of the board model, initializes the layout list in the wrapper
    /// </summary>
    public Board()
    {
        wrapper.layout = new List<string>();
    }
}
