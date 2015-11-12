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
		new Color(1, 0.9714503f, 0),                //team 2
		Color.green,                               //team 3
		Color.blue,                                //team 4
		Color.Lerp(Color.red,Color.yellow, 0.5f),  //team 5
		Color.Lerp(Color.red, Color.white, 0.5f),  //team 6
		Color.Lerp(Color.red, Color.blue, 0.5f)    //team 7
	};

    /// <summary>
    /// The name for each team
    /// </summary>
    public static string[] Names = {
        "White Team",   //team 0
        "Yellow Team",  //team 1
        "Blue Team",    //team 2
        "Green Team",   //team 3
        "Red Team",     //team 4
        "Orange Team",  //team 5
        "Pink Team",    //team 6
        "Purple Team"   //team 7
    };
}
