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
		new Color(1, 0.9714503f, 0),               //team 2
		Color.green,                               //team 3
		Color.blue,                                //team 4
		Color.Lerp(Color.red,Color.yellow, 0.5f),  //team 5
		Color.Lerp(Color.red, Color.white, 0.5f),  //team 6
		Color.Lerp(Color.red, Color.blue, 0.5f)    //team 7
	};

	public static Color[] MenuColors = {
		Color.white,                               //team 0
		new Color(0.698f, 0.121568f, 0.121568f),   //team 1
		new Color(0.796f, 0.7255f, 0.16863f),      //team 2
		new Color(0.361f, 0.8314f, 0.2706f),       //team 3
		new Color(0.2157f, 0.3412f, 0.7294f),      //team 4
		Color.Lerp(Color.red,Color.yellow, 0.5f),  //team 5
		Color.Lerp(Color.red, Color.white, 0.5f),  //team 6
		new Color(0.6392f, 0.2863f, 0.6902f),      //team 7
	};

    /// <summary>
    /// The name for each team
    /// </summary>
    public static string[] Names = {
        "White Team",   //team 0
        "Red Team",  //team 1
        "Yellow Team",    //team 2
        "Green Team",   //team 3
        "Blue Team",     //team 4
        "Orange Team",  //team 5
        "Pink Team",    //team 6
        "Purple Team"   //team 7
    };
}
