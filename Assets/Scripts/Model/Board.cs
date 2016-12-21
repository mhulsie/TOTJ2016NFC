using System;
using System.Collections.Generic;
using System.Text;


public class Board
{
    public int boardID;
    public string active;
    public int roomID;
    public GameState gamestate;
    public int turn;
    public string layout;
    public string players;

    [Serializable]
    public struct layoutWrapper { public List<string> layout; };
    public layoutWrapper wrapper;

    [Serializable]
    public struct playerWrapper { public List<Player> list; };
    public playerWrapper playerwrap;




    public Board()
    {
        wrapper.layout = new List<string>();
    }

    public void setBoard()
    {

    }
}
