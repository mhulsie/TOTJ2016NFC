using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class BoardController : MonoBehaviour
{
    public Image tile1;
    public Image tile2;
    public Image tile3;
    public Image tile4;
    public Image tile5;
    public Image tile6;
    public Image tile7;
    public Image tile8;
    public Image tile9;
    public Image tile10;
    public Image tile11;
    public Image tile12;
    public Image tile13;
    public Image tile14;
    public Image tile15;
    public Image tile16;
    public Image tile17;
    public Image tile18;
    public Image tile19;
    public Image tile20;
    public Image tile21;
    public Image tile22;
    public Image tile23;
    public Image tile24;
    public Image tile25;
    public Image tile26;
    public Image tile27;
    public Image tile28;
    public Image tile29;
    public Image tile30;

    public Image city;
    public Image cave;
    public Image village;
    public Image lake;
    public Image open;
    public Image flowers;
    public Image forest;

    public Text text;

    public bool ready = true;

    private Image[] tiles;

    private int currentTile;

    private Board board = new Board();


    [System.Serializable]
    public struct playerWrapper { public List<Player> list; };
    public playerWrapper players;

    [System.Serializable]
    public struct incidentWrapper { public List<Incident> list; };
    public incidentWrapper incidents;


    // Use this for initialization
    void Start () {

        players.list = new List<Player>();
        incidents.list = new List<Incident>();
        currentTile = 0;
        tiles = new Image[30];

        tiles[0] = tile1;
        tiles[1] = tile2;
        tiles[2] = tile3;
        tiles[3] = tile4;
        tiles[4] = tile5;
        tiles[5] = tile6;
        tiles[6] = tile7;
        tiles[7] = tile8;
        tiles[8] = tile9;
        tiles[9] = tile10;
        tiles[10] = tile11;
        tiles[11] = tile12;
        tiles[12] = tile13;
        tiles[13] = tile14;
        tiles[14] = tile15;
        tiles[15] = tile16;
        tiles[16] = tile17;
        tiles[17] = tile18;
        tiles[18] = tile19;
        tiles[19] = tile20;
        tiles[20] = tile21;
        tiles[21] = tile22;
        tiles[22] = tile23;
        tiles[23] = tile24;
        tiles[24] = tile25;
        tiles[25] = tile26;
        tiles[26] = tile27;
        tiles[27] = tile28;
        tiles[28] = tile29;
        tiles[29] = tile30;

        AndroidNFCReader.enableBackgroundScan();
        AndroidNFCReader.ScanNFC("GameController", "OnScan");
        
        

    }
	
	// Update is called once per frame
	void Update () {
        

	}


    public void OnScan(string result)
    {
        if (board.wrapper.layout.Count < 30)
        {
            text.text = result;
            int scan;
            int.TryParse(result, out scan);

            if (scan == 1)
            {
                tiles[currentTile].sprite = city.sprite;
            }
            else if (scan == 2)
            {
                tiles[currentTile].sprite = cave.sprite;
            }
            else if(scan == 3 || scan == 4 || scan == 5)
            {
                tiles[currentTile].sprite = village.sprite;
            }
            else if(scan == 6 || scan == 7)
            {
                tiles[currentTile].sprite = lake.sprite;
            }
            else if(scan >= 8 && scan <= 13)
            {
                tiles[currentTile].sprite = open.sprite;
            }
            else if (scan == 14 || scan == 15 || scan == 16)
            {
                tiles[currentTile].sprite = flowers.sprite;
            }
            else if (scan > 16)
            {
                tiles[currentTile].sprite = forest.sprite;
            }

            currentTile++;
            board.wrapper.layout.Add(result);
            
            if(board.wrapper.layout.Count == 30)
            {
                ready = true;
            }
        }

    }

    public void Undo()
    {
        if (currentTile > 0)
        {
            tiles[currentTile-1].sprite = null;
            currentTile--;

            string lastItem = board.wrapper.layout[board.wrapper.layout.Count - 1];
            board.wrapper.layout.Remove(lastItem);
        }
    }

    public void StartGame()
    {
        if(ready == true)
        {

            #region decidePlayers
            UnityEngine.Random.InitState((int) System.DateTime.Now.Ticks);
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
            while (rand1 != -1 || rand2 != -1 || rand3 != -1 || rand4 != -1)
            {
                if (rand1 > rand2 && rand1 > rand3 && rand1 > rand4)
                {
                    rand1 = -1;
                    if (RoomState.p1.nickName != "")
                    {
                        players.list.Add(RoomState.p1);
                    }
                }
                if (rand2 > rand1 && rand2 > rand3 && rand2 > rand4)
                {
                    rand2 = -1;
                    if (RoomState.p2.nickName != "")
                    {
                        players.list.Add(RoomState.p2);
                    }
                }
                if (rand3 > rand1 && rand3 > rand2 && rand3 > rand4)
                {
                    rand3 = -1;
                    if (RoomState.p3.nickName != "")
                    {
                        players.list.Add(RoomState.p3);
                    }
                }
                if (rand4 > rand1 && rand4 > rand2 && rand4 > rand3)
                {
                    rand4 = -1;
                    if (RoomState.p4.nickName != "")
                    {
                        players.list.Add(RoomState.p4);
                    }
                }
            }
            int count = 0;
            foreach (Player item in players.list)
            {
                Debug.Log(count + "   " + item.nickName);
                count++;
            }
            #endregion

            #region decideTreasure
            Treasure treasure = new Treasure();
            int.TryParse(UnityEngine.Random.Range(1f, 30f).ToString("0"), out treasure.tile);
            #endregion
            
            #region allToJson
            string layoutJson = JsonUtility.ToJson(board.wrapper);
            string playersJson = JsonUtility.ToJson(players);
            string treasureJson = JsonUtility.ToJson(treasure);
            string incidentsJson = JsonUtility.ToJson(incidents);
            #endregion

            text.text = "INSERT INTO `board`(`active`, `roomID`, `layout`, `turn`, `players`, `treasure`, `incidents`) VALUES('true','" + RoomState.id + "','" + layoutJson + "','0','" + playersJson + "','" + treasureJson + "','" + incidentsJson + "')";
            SQL.Instance.getData("INSERT INTO `board`(`active`, `roomID`, `layout`, `turn`, `players`, `treasure`, `incidents`) VALUES('true','" + RoomState.id + "','" + layoutJson + "',0,'" + playersJson + "','" + treasureJson + "','" + incidentsJson + "')");
            SceneManager.LoadScene("game");
        }
    }
}
