using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LocalLibrary
{
    public List<Item> items;
    public List<Quest> quests;
    public List<Incident> incidents;
    public List<Tile> tiles;
    public List<Status> statusEffects;
    public Board board;

    public LocalLibrary()
    {
        quests = new List<Quest>();
        incidents = new List<Incident>();
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
        if (incidentsResult != "TRUE")
        {
            string[] incidentSplitResult = incidentsResult.Split('*');
            foreach (string incident in incidentSplitResult)
            {
                Debug.Log(incident);
                incidents.Add(UnityEngine.JsonUtility.FromJson<Incident>(incident));
            }
        }
        //board = JsonUtility.FromJson<Board>(SQL.Instance.getData("select * from board where roomID = " + RoomState.id));
    }
}