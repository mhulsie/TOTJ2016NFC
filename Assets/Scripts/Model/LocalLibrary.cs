using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LocalLibrary
{
    public List<Item> items;
    public List<Quest> quests;
    public List<Tile> tiles;
    public List<Status> statusEffects;
    public Board board;

    [Serializable]
    public struct layoutWrapper { public List<string> layout; };
    public layoutWrapper layout;

    [Serializable]
    public struct playerWrapper { public List<Player> list; };
    public playerWrapper players;

    [Serializable]
    public struct incidentWrapper { public List<Incident> list; };
    public incidentWrapper incidents;



    public LocalLibrary()
    {
        quests = new List<Quest>();
        /*string itemsResult = SQL.Instance.getData("select * from item");
        string[] itemSplitResult = itemsResult.Split('*');
        foreach (string item in itemSplitResult)
        {
            items.Add(UnityEngine.JsonUtility.FromJson<Item>(item));
        }*/

        string questsResult = SQL.Instance.getData("select * from quest");
        if(questsResult != "TRUE")
        {
            string[] questSplitResult = questsResult.Split('*');
            foreach (string quest in questSplitResult)
            {
                quests.Add(UnityEngine.JsonUtility.FromJson<Quest>(quest));
            }
        }


        /*string tilesResult = SQL.Instance.getData("select * from tile");
        string[] tileSplitResult = tilesResult.Split('*');
        foreach (string tile in tileSplitResult)
        {
            tiles.Add(UnityEngine.JsonUtility.FromJson<Tile>(tile));
        }*/

        string incidentsResult = SQL.Instance.getData("select * from incident");
        incidents.list = new List<Incident>();
        Debug.Log(incidentsResult);
        if (incidentsResult != "TRUE")
        {
            string[] incidentSplitResult = incidentsResult.Split('*');
            foreach (string incident in incidentSplitResult)
            {
                Debug.Log(incident);
                incidents.list.Add(UnityEngine.JsonUtility.FromJson<Incident>(incident));
            }
        }
        board = JsonUtility.FromJson<Board>(SQL.Instance.getData("select * from board where roomID =" + RoomState.id));

        players = JsonUtility.FromJson<playerWrapper>(board.players);

        //incidents = JsonUtility.FromJson<incidentWrapper>(board.incidents);
        Debug.Log("insert qwuery " + "INSERT INTO `board`(`incidents`) VALUES ('" + JsonUtility.ToJson(incidents) + "') WHERE roomID = " + RoomState.id);
        SQL.Instance.getData("INSERT INTO `board`(`incidents`) VALUES ('" + JsonUtility.ToJson(incidents) + "') WHERE roomID = " + RoomState.id);

        layout = JsonUtility.FromJson<layoutWrapper>(board.layout);
        Debug.Log(layout.layout[0]);

    }
}