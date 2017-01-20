/// <summary>
/// This is the treasure model, the treasure can be found to win the game.
/// </summary>
public class Treasure{

    public int found; //1 if found, 0 if not
    public int tile;  //Layout position of location of treasure

    /// <summary>
    /// Constructor for Treasure, sets found variable to 0
    /// </summary>
    public Treasure()
    {
        found = 0;
    }
}
