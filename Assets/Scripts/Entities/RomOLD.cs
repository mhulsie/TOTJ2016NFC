using System;
using System.Collections.Generic;
using System.Text;

    public class RomOLD
    {
        public int roomID;
        public string name;
        public string active;
        public int players;
        public int host;

        public RomOLD(string n, string a, int p, int h){
            this.name = n;
            this.active = "false";
            this.players = p;
            this.host = h;
        }
    
    

    }