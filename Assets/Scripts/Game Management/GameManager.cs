using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

	private static GameManager _instance;
	
	public static GameManager instance {
		get {
			if (_instance == null) {
                GameObject obj = new GameObject("GameManager", typeof(GameManager));
                _instance = obj.GetComponent<GameManager>();

				
				//Tell unity not to destroy this object when loading a new scene!
				DontDestroyOnLoad (_instance.gameObject);
			}
			
			return _instance;
		}
	}
	
	void Awake ()
	{
		if (_instance == null) {
			//If I am the first instance, make me the Singleton
			_instance = this;
			DontDestroyOnLoad (this);
		} else {
			//If a Singleton already exists and you find
			//another reference in scene, destroy it!
			if (this != _instance) {
				Destroy (this.gameObject);
			}
		}
	}

	public enum GAME_STATE
	{
		MAIN_MENU,
		SETTINGS,
		ABOUT,
		GAME_MENU,
		FIND_GAME,
		HOST_GAME,
		LOBBY_WAITING,
		IN_GAME,
		GAME_RESULTS
	}

    //TODO replace with interfaces from Bolt
	private IPlayer Player;
	private LobbyState Lobby;
	private GAME_STATE currentGameState;

    //the username of the current player
    [System.NonSerialized]
    public string CurrentUserName = "";

	private IDictionary<GAME_STATE,int> stateToSceneMap;

	public IPlayer CurrentPlayer {
		get {
			return Player;
		}
	}

    public void SetCurrentPlayer(IPlayer p) {
        Player = p;
    }

	void Start ()
	{
		//Setup the mapping of game state to level number
		stateToSceneMap = new Dictionary<GAME_STATE, int>();
		stateToSceneMap.Add (GAME_STATE.MAIN_MENU, 0);
        stateToSceneMap.Add(GAME_STATE.GAME_MENU, 1);
        stateToSceneMap.Add(GAME_STATE.FIND_GAME, 1);

		//Set default game state
		currentGameState = GAME_STATE.MAIN_MENU;
		Player = GameObject.FindObjectOfType<AbstractPlayer>();
	}

	public void transitionGameState (GAME_STATE state)
	{
		checkForLevelConflicts (state);

		//Here is where we can do other things such as doing the animation swaps for the menu system, etc.
	}

	private void checkForLevelConflicts(GAME_STATE newState)
	{
		int currentStateLevel = stateToSceneMap [currentGameState];
		int newStateLevel = stateToSceneMap [newState];
		if (newStateLevel != currentStateLevel) {
			Application.LoadLevel(newStateLevel);
		}
	}

}
