/// <summary>
/// This is the Quest model, a quest can be found in each village.
/// When the quest is complete, a piece of the treasure code is given.
/// When all three codes are collected, the treasure can be digged.
/// </summary>
public class Quest
{
    public int questID;                 //Quest ID
    public string name;                 //Name of quest
    public string startPoint;           //Type of tile to collect quest, always a Village
    public string doPoint;              //Type of tile to do the quest, like a Cave / Lake / Airplane / Village / Open / Forest / Flowers
    public string turnInPoint;          //Type of tile to finish the quest, a Village when turning in is needed
    public Dialogue startDialogueD;     //Dialogue for collecting quest
    public Dialogue doDialogueD;        //Dialogue for doing the quest
    public Dialogue turnInDialogueD;    //Dialogue for turning in
    public string startDialogue;        //Jsonstring variant
    public string doDialogue;           //Jsonstring variant
    public string turnInDialogue;       //Jsonstring variant
    public string type;                 //Type of questcode: Red / Blue / Green

    public int progress;                //Progress of quest: 0 / 1 / 2 / 3
    public int startPosition;           //Position in layout to collect quest
    public int doPosition;              //Position in layout to do quest
    public int turnInPosition;          //Position in layout for turning in
}