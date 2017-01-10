using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LocalLibrary
{
    public List<Item> items;
    public List<Quest> quests;
    public List<Quest> blueQuests;
    public List<Quest> redQuests;
    public List<Quest> greenQuests;
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
        redQuests = new List<Quest>();
        blueQuests = new List<Quest>();
        greenQuests = new List<Quest>();

        string questsResult = SQL.Instance.getData("select * from quest");
        Debug.Log(questsResult);
        if(questsResult != "TRUE")
        {
            string[] questSplitResult = questsResult.Split('*');
            foreach (string quest in questSplitResult)
            {
                Debug.Log(quest);
                quests.Add(JsonUtility.FromJson<Quest>(quest));
            }
        }
        Quest tempQuest = new Quest();
        foreach (Quest item in quests)
        {
            //Kijk of het een start quest is
            if (item.name.Contains("Start"))
            {
                Debug.Log(item.questID);
                int tryParseInt;
                // Kijk naar de opvolgende quest
                if(int.TryParse(item.result, out tryParseInt))
                {
                    //Pak de volgende quest
                    tempQuest = quests[tryParseInt - 1];
                    //Indien er nog een volgende quest is
                    if(int.TryParse(tempQuest.result, out tryParseInt))
                    {
                        //pak de kleurcode
                        tempQuest = quests[tryParseInt - 1];
                    }
                }

                if(tempQuest.result == "blue")
                {
                    blueQuests.Add(item);
                }
                else if(tempQuest.result == "red")
                {
                    redQuests.Add(item);
                }
                else if(tempQuest.result == "green")
                {
                    greenQuests.Add(item);
                }
            }
        }

        string incidentsResult = SQL.Instance.getData("select * from incident");
        incidents.list = new List<Incident>();
        if (incidentsResult != "TRUE")
        {
            string[] incidentSplitResult = incidentsResult.Split('*');
            foreach (string incident in incidentSplitResult)
            {
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