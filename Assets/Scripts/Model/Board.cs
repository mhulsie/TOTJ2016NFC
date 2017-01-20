using System;
using System.Collections.Generic;
using System.Text;


public class Board
{
    public int boardID;
    public string active;
    public int roomID;
    public int turn;
    public string layout;
    public string players;
    public string incidents;
    public string treasure;
    public Treasure treasureT;

    [Serializable]
    public struct layoutWrapper { public List<string> layout; };
    public layoutWrapper wrapper;

    [Serializable]
    public struct playerWrapper { public List<Player> list; };
    public playerWrapper playerwrap;

    [Serializable]
    public struct incidentWrapper { public List<Incident> list; };
    public incidentWrapper incidentwrap;




    public Board()
    {
        wrapper.layout = new List<string>();
    }

    public void setBoard()
    {

    }
}
