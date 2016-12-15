using UnityEngine;

public class GameState : MonoBehaviour  {

    public int currentTurn;
    public Player[] players;
    public Treasure treasure;
    public Event[] events;

    public GameState(bool t)
    {
        currentTurn = 1;
        // Host
        if (t)
        {
            Random rnd = new Random();
            Random.InitState(1234567);
            int rand1;
            int rand2;
            int rand3;
            int rand4;
            int.TryParse(Random.Range(0f, 100f).ToString("0"), out rand1);
            int.TryParse(Random.Range(0f, 100f).ToString("0"), out rand2);
            int.TryParse(Random.Range(0f, 100f).ToString("0"), out rand3);
            int.TryParse(Random.Range(0f, 100f).ToString("0"), out rand4);
            while (rand1 == rand2 || rand1 == rand3 || rand1 == rand4 || rand2 == rand3 || rand2 == rand4 || rand3 == rand4)
            {
                int.TryParse(Random.Range(0f, 100f).ToString("0"), out rand1);
                int.TryParse(Random.Range(0f, 100f).ToString("0"), out rand2);
                int.TryParse(Random.Range(0f, 100f).ToString("0"), out rand3);
                int.TryParse(Random.Range(0f, 100f).ToString("0"), out rand4);
            }
            int count = 0;
            players = new Player[4];
            while (rand1 != -1 && rand2 != -1 && rand3 != -1 && rand4 != -1)
            {
                if (rand1 > rand2 && rand1 > rand3 && rand1 > rand4)
                {
                    this.players[count] = RoomState.p1;
                    count++;
                    rand1 = -1;
                }
                if (rand2 > rand1 && rand2 > rand3 && rand2 > rand4)
                {
                    this.players[count] = RoomState.p2;
                    count++;
                    rand2 = -1;
                }
                if (rand3 > rand1 && rand3 > rand2 && rand3 > rand4)
                {
                    this.players[count] = RoomState.p3;
                    count++;
                    rand3 = -1;
                }
                if (rand4 > rand1 && rand4 > rand2 && rand4 > rand3)
                {
                    this.players[count] = RoomState.p4;
                    count++;
                    rand4 = -1;
                }

            }

            treasure = new Treasure();
            int randx;
            int randy;
            int.TryParse(Random.Range(1f, 7f).ToString("0"), out randx);
            int.TryParse(Random.Range(1f, 6f).ToString("0"), out randy);
            treasure.location = new Location(randx, randy);

            //TODO calc events and items

            SQL.Instance.getData("UPDATE `totj`.`room` SET `started` = 'true' WHERE `room`.`roomID` = " + RoomState.id);
            submitGameState();
        }
        // Join
        else
        {
            getGameState();
        }
    }

    public void submitGameState()
    {
        SQL.Instance.getData("UPDATE `totj`.`board` SET `Gamestate` = '" + UnityEngine.JsonUtility.ToJson(this) + "' where roomID = " + RoomState.id);
    }

    public GameState getGameState()
    {
        return JsonUtility.FromJson<GameState>(SQL.Instance.getData("select Gamestate from board where roomID = " + RoomState.id));
    }
}
