using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is the local library class. It is used to get and store data from the catalogues in the databases.
/// It also sets the map and determines which quests are assigned to the player.
/// </summary>
public class LocalLibrary
{
    public int positionVillageC;        //Layout positions of each village
    public int positionVillageB;
    public int positionVillageA;
    public int positionAirplane;        //Layout position of the airplane tile
    public int positionCave;            //Layout position of the cave tile
    public List<int> positionLake;      //Layout positions of the lake tiles
    public List<int> positionOpen;      //Layout positions of the open place tiles
    public List<int> positionFlowers;   //Layout positions of the flower tiles
    public List<int> positionForest;    //Layout positions of the forest tiles
    public List<int> positionBirds;     //Layout positions of the bird tiles
    public List<int> positionPlants;    //Layout positions of the plant tiles

    public List<Quest> quests;          //List of all quests
    public Treasure treasure;           //Treasure of this game
    public int turn;                    //Current turn
    private Board board;                //Board from this game
    public bool myTurn = false;         //True when current turn is my turn

    /// <summary>
    /// This is the wrapper for the board layout list.
    /// It is done this way bescause Json cannot directly convert a list.
    /// </summary>
    [System.Serializable]
    public struct layoutWrapper { public List<string> list; };
    public layoutWrapper layout;

    /// <summary>
    /// This is the wrapper for the player list.
    /// It is done this way bescause Json cannot directly convert a list.
    /// </summary>
    [System.Serializable]
    public struct playerWrapper { public List<Player> list; };
    public playerWrapper players;

    /// <summary>
    /// This is the wrapper for the incident list.
    /// It is done this way bescause Json cannot directly convert a list.
    /// </summary>
    [System.Serializable]
    public struct incidentWrapper { public List<Incident> list; };
    public incidentWrapper incidents;

    /// <summary>
    /// The constructor of the local library
    /// </summary>
    public LocalLibrary()
    {
        //Get the current board state from database and decode into board object
        string boardResult = SQL.Instance.executeQuery("select * from board where roomID =" + RoomState.id);
        board = JsonUtility.FromJson<Board>(boardResult);
        turn = board.turn;

        //Initializes lists
        positionLake = new List<int>();
        positionOpen = new List<int>();
        positionFlowers = new List<int>();
        positionForest = new List<int>();
        positionBirds = new List<int>();
        positionPlants = new List<int>();
        incidents.list = new List<Incident>();
        //Update local data
        updateData();
        //Get permanent data
        getData();
    }

    /// <summary>
    /// Gets the renewable data
    /// </summary>
    public void updateData()
    {
        string boardResult = SQL.Instance.executeQuery("select * from board where roomID =" + RoomState.id);
        board = JsonUtility.FromJson<Board>(boardResult);
        
        incidents.list = new List<Incident>();
        incidents = JsonUtility.FromJson<incidentWrapper>(board.incidents);

        turn = board.turn;
    }
    /// <summary>
    /// Gets the permanent data
    /// </summary>
    public void getData()
    {
        //Get quests
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

        //Get players
        players.list = new List<Player>();
        players = JsonUtility.FromJson<playerWrapper>(board.players);

        //Get treasure
        treasure = JsonUtility.FromJson<Treasure>(board.treasure);
        
        //Get layout
        layout = JsonUtility.FromJson<layoutWrapper>(board.layout);
        
        //Set map
        setMap();
        
        //Assign quests to player
        defineQuests();
    }

    /// <summary>
    /// This function sets the map, so later on clues and quest positions can be shown
    /// </summary>
    public void setMap()
    {
        //layout.list.RemoveAt(0);
        //layout.list[0] = "11";
        //layout.list.Add("8");
        

        //This loop loops through the layout 
        for (int i = 0; i < 30; i++)
        {

            //It assigns the right tile image to each image on the map according to the tile id
            int j = i + 1;
            Image tempImage = GameObject.Find("Image" + j).GetComponent<Image>();
            int current = int.Parse(layout.list[i]);
            tempImage.sprite = GameObject.Find("tile " + current).GetComponent<SpriteRenderer>().sprite;


            //Then it sets the layout positions of different tile types like airplane, cave etc according to tile id.
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

    /// <summary>
    /// This function assigns random red, blue and green quests to the player
    /// </summary>
    public void defineQuests()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        //Calculate the place of the player in the turnorder
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

        //Define the 3 quests
        setRandomTile(turn);
        setRandomTile(turn + 4);
        setRandomTile(turn + 8);

        //Set the quest values to the playerstate
        PlayerState.redQuest = quests[turn];
        PlayerState.greenQuest = quests[turn + 4];
        PlayerState.blueQuest = quests[turn + 8];

        //Set the progress values to the quests
        PlayerState.redQuest.progress = 0;
        PlayerState.greenQuest.progress = 0;
        PlayerState.blueQuest.progress = 0;

        //Set the dialogues for each quest
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

    /// <summary>
    /// This functions sets the tiles for quest start, do and turnin points
    /// </summary>
    /// <param name="i">The index of the quest in local.quests</param>
    public void setRandomTile(int i)
    {
        //Translate the startpoint to a position   
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

        //Translate dopoint to correct position
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

        //Translate the turnInPoint to the correct position
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