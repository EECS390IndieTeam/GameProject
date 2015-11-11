using UnityEngine;

/// <summary>
/// Contains static information about teams
/// </summary>
public static class Teams {
    /// <summary>
    /// The colors corresponding to each team
    /// </summary>
    public static Color[] Colors = {
		Color.white,                               //team 0
		Color.red,                                 //team 1
		Color.blue,                                //team 2
		Color.green,                               //team 3
		Color.yellow,                              //team 4
		Color.Lerp(Color.red,Color.yellow, 0.5f),  //team 5
		Color.Lerp(Color.red, Color.white, 0.5f),  //team 6
		Color.Lerp(Color.red, Color.blue, 0.5f)    //team 7
	};

    /// <summary>
    /// The name for each team
    /// </summary>
    public static string[] Names = {
        "White Team",   //team 0
        "Red Team",     //team 1
        "Blue Team",    //team 2
        "Green Team",   //team 3
        "Yellow Team",  //team 4
        "Orange Team",  //team 5
        "Pink Team",    //team 6
        "Purple Team"   //team 7
    };
}
