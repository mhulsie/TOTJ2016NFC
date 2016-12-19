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
    [Serializable]
    public struct layoutWrapper { public List<string> layout; };
    public layoutWrapper wrapper;


    public Board()
    {
        wrapper.layout = new List<string>();
    }

    public void setBoard()
    {

    }
}
