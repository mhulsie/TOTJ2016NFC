using System;
using System.Collections.Generic;
using System.Text;


public class Board
{
    public int boardID;
    public string active;
    public int roomID;
    public List<string> layout;
    public GameState gamestate;
    public int turn;

    public Board()
    {
        layout = new List<string>();
    }

    public void setBoard()
    {

    }
}
