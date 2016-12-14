using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace totj3.Models
{
    public class GameState
    {
        public int currentTurn;
        public Player[] players;
        public Treasure treasure;
        public Event[] events;

        public GameState(bool t)
        {/*
            currentTurn = 1;
            // Host
            if (t)
            {
                Random rnd = new Random();
                int rand1 = rnd.Next(100);
                int rand2 = rnd.Next(100);
                int rand3 = rnd.Next(100);
                int rand4 = rnd.Next(100);
                while(rand1 == rand2 || rand1 == rand3 || rand1 == rand4 || rand2 == rand3 || rand2 == rand4 || rand3 == rand4)
                {
                    rand1 = rnd.Next(100);
                    rand2 = rnd.Next(100);
                    rand3 = rnd.Next(100);
                    rand4 = rnd.Next(100);
                }
                int count = 0;
                players = new Player[4];
                while(rand1 != -1 && rand2 != -1 && rand3 != -1 && rand4 != -1)
                {
                    if (rand1 > rand2 && rand1 > rand3 && rand1 > rand4)
                    {
                        this.players[count] = new Player(RoomState.p1);
                        count++;
                        rand1 = -1;
                    }
                    if (rand2 > rand1 && rand2 > rand3 && rand2 > rand4)
                    {
                        this.players[count] = new Player(RoomState.p2);
                        count++;
                        rand2 = -1;
                    }
                    if (rand3 > rand1 && rand3 > rand2 && rand3 > rand4)
                    {
                        this.players[count] = new Player(RoomState.p3);
                        count++;
                        rand3 = -1;
                    }
                    if (rand4 > rand1 && rand4 > rand2 && rand4 > rand3)
                    {
                        this.players[count] = new Player(RoomState.p4);
                        count++;
                        rand4 = -1;
                    }

                }
                //TODO calcualte treasure
                treasure = new Treasure();
                treasure.location = new Location(rnd.Next(1, 7), rnd.Next(1, 6));
                //TODO calc events and items
                //?????????????????????

                //SQL.query("UPDATE `totj`.`room` SET `started` = 'true' WHERE `room`.`roomID` = " + RoomState.roomID);
                submitGameState();
            }
            // Join
            else
            {
                getGameState();
            }      */     
        }

        public void submitGameState()
        {
            //sql.query("UPDATE `totj`.`board` SET `Gamestate` = '" + UnityEngine.JsonUtility.ToJson(this) + "' where roomID = " + RoomState.roomID);
        }

        public void getGameState()
        {
            //return UnityEngine.JsonUtility.FromJson<GameState>(SQL.query("select Gamestate from board where roomID = " + RoomState.roomID));
        }
    }
}
