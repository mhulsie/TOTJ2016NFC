using System;
using System.Collections.Generic;
using System.Text;

public class LocalLibrary
{
    public List<Item> items;
    public List<Quest> quests;
    public List<Incident> events;
    public List<Tile> tiles;
    public List<Status> statusEffects;
    public Board board;

    public LocalLibrary()
    {
        string itemsResult = SQL.Instance.getData("select * from item");
        string[] itemSplitResult = itemsResult.Split('*');
        foreach (string item in itemSplitResult)
        {
            items.Add(UnityEngine.JsonUtility.FromJson<Item>(item));
        }

        string questsResult = SQL.Instance.getData("select * from quest");
        string[] questSplitResult = questsResult.Split('*');
        foreach (string quest in questSplitResult)
        {
            quests.Add(UnityEngine.JsonUtility.FromJson<Quest>(quest));
        }

        string tilesResult = SQL.Instance.getData("select * from tile");
        string[] tileSplitResult = tilesResult.Split('*');
        foreach (string tile in tileSplitResult)
        {
            tiles.Add(UnityEngine.JsonUtility.FromJson<Tile>(tile));
        }

        string incidentsResult = SQL.Instance.getData("select * from event");
        string[] incidentSplitResult = incidentsResult.Split('*');
        foreach (string incident in incidentSplitResult)
        {
            events.Add(UnityEngine.JsonUtility.FromJson<Incident>(incident));
        }

        board = UnityEngine.JsonUtility.FromJson<Board>(SQL.Instance.getData("select * from board where roomID = " + RoomState.id));
    }
}
