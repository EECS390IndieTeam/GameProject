using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

	public enum GameState
	{
		MENU,
        LOBBY,
        PRE_GAME,
		IN_GAME,
		POST_GAME
	}

	private IPlayer Player;

    /// <summary>
    /// The value of BoltNetwork.ServerTime when the game started
    /// </summary>
    public float GameStartTime {
        get;
        private set;
    }

    public GameState CurrentGameState {
        get;
        private set;
    }

    private IGameMode _GameMode;
    public IGameMode GameMode {
        get { return _GameMode; }
        set {
            _GameMode = value;
            if (BoltNetwork.isServer) {
                Lobby.GameModeName = value.GetType().Name;
            }
        }
    }

    //the username of the current player
    [System.NonSerialized]
    public string CurrentUserName = "";
    public int CurrentPlayerStatIndex = 0;

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
		//Set default game state
		CurrentGameState = GameState.MENU;
        Lobby.LobbyUpdatedEvent += LobbyUpdated;
	}

    /// <summary>
    /// callback for lobby change events
    /// </summary>
    /// <param name="change"></param>
    public void LobbyUpdated(Lobby.LobbyChange change) {
        if(BoltNetwork.isServer) CheckForGameOver();
    }

	public void ChangeGameState (GameState state)
	{
        CurrentGameState = state;
        switch (state) {
            case GameState.PRE_GAME:
                if (BoltNetwork.isServer) {
                    Lobby.StatChangesAllowed = true;
                    Lobby.RemoveAllDisconnectedPlayers();
                    Lobby.ClearAllStats();
                    GameMode.OnPreGame();
                }
                break;
            case GameState.IN_GAME:
                GameStartTime = BoltNetwork.serverTime;
                if(BoltNetwork.isServer) GameMode.OnGameStart();
                break;
            case GameState.POST_GAME:
                if (BoltNetwork.isServer) {
                    Lobby.StatChangesAllowed = false;
                }
                break;
        }
        if (BoltNetwork.isServer) ServerSideData.UpdateZeusData();
	}

    public void CheckForGameOver() {
        if (!BoltNetwork.isServer) return;
        if (CurrentGameState != GameState.IN_GAME) return;
        if (GameMode.GameOver()) {
            GameOver();
        }
    }

    private void GameOver() {
            GameMode.OnGameEnd();
            ChangeGameState(GameState.POST_GAME);
            //more game over stuff here
    }

    void Update() {
        if(!BoltNetwork.isServer) return;
        if(CurrentGameState != GameState.IN_GAME) return;
        float gameTime = BoltNetwork.serverTime - GameStartTime;
        if (gameTime <= GameMode.TimeLimit) {
            Debug.Log("Timer expired!");
            GameOver();
        }
    }

}
