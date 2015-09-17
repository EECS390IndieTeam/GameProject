using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Interfaces_REPLACEME : MonoBehaviour {

	interface ILobby {
		List<IPlayer> players {
			get;
		}
		IGameMode selectedGameMode {
			get;
			set;
		}
		IGameLevel selectedGameLevel {
			get;
			set;
		}
		void addPlayerToLobby(IPlayer player);
		void removePlayerToLobby(IPlayer player);
		void startMatch();
	}
	
	interface IPlayer {
		string username 
		{
			get;
			set;
		}
		string connectionName {
			get;
			set;
		}
		int teamID 
		{
			get;
			set;
		}
		
		List<IWeapon> availableWeapons {
			get;
		}
		IWeapon currentWeapon {
			get;
		}
		
	}
	
	interface IWeapon {
		
	}
	
	interface IGameMode {
		int minPlayers
		{
			get;
		}
		int maxPlayers
		{
			get;
		}
		IGameLevel level {
			get;
			set; //Not sure if we need to be able to set this or not.
		}
		string gameModeName 
		{
			get;
		}
	}
	
	interface IGameLevel {
		string levelName {
			get;
		}
		void loadGameLevel();
	}
}
