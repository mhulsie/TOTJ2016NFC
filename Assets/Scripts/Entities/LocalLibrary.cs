using System;
using System.Collections.Generic;
using System.Text;

namespace totj3.Models
{
    public class LocalLibrary
    {
        public List<Item> items;
        public List<Quest> quests;
        public List<Event> events;
        public List<Tile> tiles;
        public Board board;

        public LocalLibrary()
        {
         /*   string itemsResult = SQL.query("select * from item");
            string[] itemSplitResult = itemsResult.Split('*');
            foreach (string item in itemSplitResult)
            {
                items.Add(UnityEngine.JsonUtility.FromJson<Item>(item));
            }

            string questsResult = SQL.query("select * from quest");
            string[] questSplitResult = questsResult.Split('*');
            foreach (string quest in questSplitResult)
            {
                quests.Add(UnityEngine.JsonUtility.FromJson<Quest>(quest));
            }

            string tilesResult = SQL.query("select * from tile");
            string[] tileSplitResult = tilesResult.Split('*');
            foreach (string tile in tileSplitResult)
            {
                tiles.Add(UnityEngine.JsonUtility.FromJson<Tile>(tile));
            }

            string incidentsResult = SQL.query("select * from event");
            string[] incidentSplitResult = incidentsResult.Split('*');
            foreach (string incident in incidentSplitResult)
            {
                events.Add(UnityEngine.JsonUtility.FromJson<Event>(incident));
            }

            board = UnityEngine.JsonUtility.FromJson<Board>(SQL.query("select * from board where roomID = " + RoomState.roomID));
            */
        }
    }
}