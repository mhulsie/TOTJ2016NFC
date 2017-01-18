using System;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour  {

    public int currentTurn;
    public Treasure treasure;


    [Serializable]
    public struct playerWrapper { public List<Player> list; };
    public playerWrapper players;
    
    [Serializable]
    public struct incidentWrapper { public List<Incident> list; };
    public incidentWrapper incidents;

    public GameState(bool t)
    {

            //TODO calc events and items

            SQL.Instance.executeQuery("UPDATE `totj`.`room` SET `started` = 'true' WHERE `room`.`roomID` = " + RoomState.id);
            submitGameState();
    }

    public void createGameState()
    {
    currentTurn = 1;

        UnityEngine.Random.InitState(1234567);
        int rand1;
        int rand2;
        int rand3;
        int rand4;
        int.TryParse(UnityEngine.Random.Range(0f, 100f).ToString("0"), out rand1);
        int.TryParse(UnityEngine.Random.Range(0f, 100f).ToString("0"), out rand2);
        int.TryParse(UnityEngine.Random.Range(0f, 100f).ToString("0"), out rand3);
        int.TryParse(UnityEngine.Random.Range(0f, 100f).ToString("0"), out rand4);
        while (rand1 == rand2 || rand1 == rand3 || rand1 == rand4 || rand2 == rand3 || rand2 == rand4 || rand3 == rand4)
        {
            int.TryParse(UnityEngine.Random.Range(0f, 100f).ToString("0"), out rand1);
            int.TryParse(UnityEngine.Random.Range(0f, 100f).ToString("0"), out rand2);
            int.TryParse(UnityEngine.Random.Range(0f, 100f).ToString("0"), out rand3);
            int.TryParse(UnityEngine.Random.Range(0f, 100f).ToString("0"), out rand4);
        }
        while (rand1 != -1 && rand2 != -1 && rand3 != -1 && rand4 != -1)
        {
            if (rand1 > rand2 && rand1 > rand3 && rand1 > rand4)
            {
                players.list.Add(RoomState.p1);
                rand1 = -1;
            }
            if (rand2 > rand1 && rand2 > rand3 && rand2 > rand4)
            {
                players.list.Add(RoomState.p2);
                rand2 = -1;
            }
            if (rand3 > rand1 && rand3 > rand2 && rand3 > rand4)
            {
                players.list.Add(RoomState.p3);
                rand3 = -1;
            }
            if (rand4 > rand1 && rand4 > rand2 && rand4 > rand3)
            {
                players.list.Add(RoomState.p4);
                rand4 = -1;
            }

        }

        treasure = new Treasure();
        int.TryParse(UnityEngine.Random.Range(1f, 31f).ToString("0"), out treasure.tile);
    }

    public void submitGameState()
    {
        SQL.Instance.executeQuery("UPDATE `totj`.`board` SET `Gamestate` = '" + UnityEngine.JsonUtility.ToJson(this) + "' where roomID = " + RoomState.id);
    }

    public GameState getGameState()
    {
        return JsonUtility.FromJson<GameState>(SQL.Instance.executeQuery("select Gamestate from board where roomID = " + RoomState.id));
    }
}
