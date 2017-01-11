public class Quest
{
    public int questID;
    public string name;
    public string startPoint; //Village
    public string doPoint; // Cave / Lake / Airplane / Village / Open / Forest / Flowers
    public string turnInPoint; // Village if exists
    public Dialogue startDialogueD;
    public Dialogue doDialogueD;
    public Dialogue turnInDialogueD;
    public string startDialogue;
    public string doDialogue;
    public string turnInDialogue;
    public string type;

    public int progress;
    public int startPosition;
    public int doPosition;
    public int turnInPosition;
}