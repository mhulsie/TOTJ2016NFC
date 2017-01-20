using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// This class is the model for the board
/// </summary>
public class Board
{
    /// <summary>
    /// These are the variables used in the board, corresponding to the database table,
    /// string variables for use in Json format
    /// </summary>
    public int boardID;
    public string active;
    public int roomID;
    public int turn;
    public string layout;
    public string players;
    public string incidents;
    public string treasure;
    public Treasure treasureT;

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
    /// Constructor of the board model
    /// </summary>
    public Board()
    {
        wrapper.layout = new List<string>();
    }
}
