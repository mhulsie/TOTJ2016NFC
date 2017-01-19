using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LocalLibrary
{
    public int positionVillageC;
    public int positionVillageB;
    public int positionVillageA;
    public int positionAirplane;
    public int positionCave;
    public List<int> positionLake;
    public List<int> positionOpen;
    public List<int> positionFlowers;
    public List<int> positionForest;
    public List<int> positionBirds;
    public List<int> positionPlants;

    public List<Quest> quests;
    public Treasure treasure;
    public int turn;
    private Board board;
    public bool myTurn = false;

    [System.Serializable]
    public struct layoutWrapper { public List<string> list; };
    public layoutWrapper layout;

    [System.Serializable]
    public struct playerWrapper { public List<Player> list; };
    public playerWrapper players;

    [System.Serializable]
    public struct incidentWrapper { public List<Incident> list; };
    public incidentWrapper incidents;

    public LocalLibrary()
    {
        string boardResult = SQL.Instance.executeQuery("select * from board where roomID =" + RoomState.id);
        board = JsonUtility.FromJson<Board>(boardResult);
        turn = board.turn;

        positionLake = new List<int>();
        positionOpen = new List<int>();
        positionFlowers = new List<int>();
        positionForest = new List<int>();
        positionBirds = new List<int>();
        positionPlants = new List<int>();
        incidents.list = new List<Incident>();
        updateData();
        getData();
    }

    /// <summary>
    /// Gets the renewable data
    /// </summary>
    public void updateData()
    {
        string incidentsResult = SQL.Instance.executeQuery("select * from incident");
        incidents.list = new List<Incident>();
        if (incidentsResult != "TRUE")
        {
            string[] incidentSplitResult = incidentsResult.Split('*');
            foreach (string incident in incidentSplitResult)
            {
                incidents.list.Add(JsonUtility.FromJson<Incident>(incident));
            }
        }
    }
    /// <summary>
    /// Gets the permanent data
    /// </summary>
    public void getData()
    {
        quests = new List<Quest>();
        string questsResult = SQL.Instance.executeQuery("select * from quest");
        if (questsResult != "TRUE")
        {
            string[] questSplitResult = questsResult.Split('*');
            foreach (string quest in questSplitResult)
            {
                quests.Add(JsonUtility.FromJson<Quest>(quest));
            }
        }

        players.list = new List<Player>();
        players = JsonUtility.FromJson<playerWrapper>(board.players);

        treasure = JsonUtility.FromJson<Treasure>(board.treasure);
        
        layout = JsonUtility.FromJson<layoutWrapper>(board.layout);
        setMap();
        defineQuests();
    }

    public void setMap()
    {
        //layout.list.RemoveAt(0);
        //layout.list[0] = "11";
        //layout.list.Add("8");
        
        for (int i = 0; i < 30; i++)
        {
            int j = i + 1;
            Image tempImage = GameObject.Find("Image" + j).GetComponent<Image>();
            int current = int.Parse(layout.list[i]);
            tempImage.sprite = GameObject.Find("tile " + current).GetComponent<SpriteRenderer>().sprite;

            if (current == 1)
            {
                positionAirplane = i;
            }
            else if (current == 2)
            {
                positionCave = i;
            }
            else if (current == 3 || current == 4 || current == 5)
            {
                if (current == 3)
                {
                    positionVillageA = i;
                }
                else if (current == 4)
                {
                    positionVillageB = i;
                }
                else if (current == 5)
                {
                    positionVillageC = i;
                }
            }
            else if (current == 6 || current == 7)
            {
                positionLake.Add(i);
            }
            else if (current >= 8 && current <= 13)
            {
                positionOpen.Add(i);
            }
            else if (current == 14 || current == 15 || current == 16)
            {
                positionPlants.Add(i);
            }
            else if (current == 17 || current == 18 || current == 19)
            {
                positionFlowers.Add(i);
            }
            else if (current > 19)
            {
                positionForest.Add(i);
            }

            if (current == 8 || current == 9 || current == 10 || current == 14 || current == 20 || current == 22 || current == 30)
            {
                positionBirds.Add(i);
            }
        }
    }

    public void defineQuests()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        int turn = 0;
        int count = 0;
        foreach (Player item in players.list)
        {
            if (item.accountID == PlayerState.id)
            {
                turn = count;
            }
            else
            { 
                count++;
            }
        }

        setRandomTile(turn);
        setRandomTile(turn + 4);
        setRandomTile(turn + 8);

        PlayerState.redQuest = quests[turn];
        PlayerState.greenQuest = quests[turn + 4];
        PlayerState.blueQuest = quests[turn + 8];

        PlayerState.redQuest.progress = 0;
        PlayerState.greenQuest.progress = 0;
        PlayerState.blueQuest.progress = 0;

        PlayerState.redQuest.startDialogueD = JsonUtility.FromJson<Dialogue>(PlayerState.redQuest.startDialogue);
        PlayerState.redQuest.doDialogueD = JsonUtility.FromJson<Dialogue>(PlayerState.redQuest.doDialogue);
        PlayerState.redQuest.turnInDialogueD = JsonUtility.FromJson<Dialogue>(PlayerState.redQuest.turnInDialogue);

        PlayerState.blueQuest.startDialogueD = JsonUtility.FromJson<Dialogue>(PlayerState.blueQuest.startDialogue);
        PlayerState.blueQuest.doDialogueD = JsonUtility.FromJson<Dialogue>(PlayerState.blueQuest.doDialogue);
        PlayerState.blueQuest.turnInDialogueD = JsonUtility.FromJson<Dialogue>(PlayerState.blueQuest.turnInDialogue);

        PlayerState.greenQuest.startDialogueD = JsonUtility.FromJson<Dialogue>(PlayerState.greenQuest.startDialogue);
        PlayerState.greenQuest.doDialogueD = JsonUtility.FromJson<Dialogue>(PlayerState.greenQuest.doDialogue);
        PlayerState.greenQuest.turnInDialogueD = JsonUtility.FromJson<Dialogue>(PlayerState.greenQuest.turnInDialogue);
    }

    public void setRandomTile(int i)
    {
        if (quests[i].startPoint == "3")
        {
            quests[i].startPosition = positionVillageA;
        }
        else if (quests[i].startPoint == "4")
        {
            quests[i].startPosition = positionVillageB;
        }
        else if (quests[i].startPoint == "5")
        {
            quests[i].startPosition = positionVillageC;
        }

        switch (quests[i].doPoint)
        {
            case "cave":
                quests[i].doPosition = positionCave;
                break;
            case "villageBlue":
                quests[i].doPosition = positionVillageB;
                break;
            case "villageGreen":
                quests[i].doPosition = positionVillageA;
                break;
            case "airplane":
                quests[i].doPosition = positionAirplane;
                break;
            case "birds":
                quests[i].doPosition = positionBirds[(int)Random.Range(0f, positionBirds.Count - 1)];
                break;
            case "plants":
                quests[i].doPosition = positionPlants[(int)Random.Range(0f, positionPlants.Count - 1)];
                break;
            case "flowers":
                quests[i].doPosition = positionFlowers[(int)Random.Range(0f, positionFlowers.Count - 1)];
                break;
            case "water":
                quests[i].doPosition = positionLake[(int)Random.Range(0f, positionLake.Count - 1)];
                break;
        }
        if (quests[i].turnInPoint == "3")
        {
            quests[i].turnInPosition = positionVillageA;
        }
        else if (quests[i].turnInPoint == "4")
        {
            quests[i].turnInPosition = positionVillageB;
        }
        else if (quests[i].turnInPoint == "5")
        {
            quests[i].turnInPosition = positionVillageC;
        }
    }
}