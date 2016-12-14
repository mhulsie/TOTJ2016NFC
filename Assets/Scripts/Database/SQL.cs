using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// The main class that talks with the database. All queries go trough this class
/// </summary>
public class SQL
{
    private static SQL instance;
    public static SQL Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SQL();
            }
            return instance;
        }
    }

    private SQL() { }
    /// <summary>
    /// The query function.
    /// </summary>
    /// <param name="query"> The query you want to execute</param>
    /// <returns>The webrequest.text. This contains all text of the webpage</returns>
    public string getData(string query)
    {
        WWW www = new WWW("http://questionmarkgames.nl/API/query.php?s=abcd&q=" + WWW.EscapeURL(query));
        while (!www.isDone) { }
        return www.text;
    }
}