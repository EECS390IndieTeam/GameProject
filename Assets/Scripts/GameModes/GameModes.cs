using UnityEngine;
using System.Collections;

//this is an enum of all the game modes
public enum GameModes : byte {
	CAPTURE_THE_FLAG = 0,   //capture the objective for points
	TEAM_DEATHMATCH,    //kill the other team for points
    FFA_DEATHMATCH, //free for all, kill other players for points
}
