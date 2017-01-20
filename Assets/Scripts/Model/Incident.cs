/// <summary>
/// This is the incident model, incidents can be encountered on different tiles
/// Class is made serializable so a list of incidents can be stored in a wrapper, so Json can encode it
/// </summary>
[System.Serializable]
public class Incident
{
    
    public int incidentID;      //Identification
    public string name;         //Name of incident
    public string action;       //Result of encountered incident
    public int tile;            //ID of tile on which this incident is placed
    public string title;        //Title to show in dialogue
    public string description;  //Description to show in dialogue
    public string button;       //Button text to show in dialogue
}