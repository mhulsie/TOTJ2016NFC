using System;
using System.Collections.Generic;
using System.Text;

namespace totj3.Models
{
    public class Board : Model
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
}
