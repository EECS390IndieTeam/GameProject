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
				Destroy (this);
			}
		}
	}

	public enum GameState
	{
		MENU,
        LOBBY,
        PRE_GAME,
		IN_GAME,
		POST_GAME_FADE,
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

    private IGameMode _GameMode = GameModeManager.GameModes.First();
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
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
            case GameState.IN_GAME:
                GameStartTime = BoltNetwork.serverTime;
                if(BoltNetwork.isServer) GameMode.OnGameStart();
                break;
            case GameState.POST_GAME_FADE:
                if (BoltNetwork.isServer) {
                    Lobby.StatChangesAllowed = false;
                } else {
                    Lobby.RequestFullDataFromServer();
                }
                break;
            case GameState.POST_GAME:
                if (BoltNetwork.isServer) {
                    BoltNetwork.LoadScene(BoltScenes.postgame);
                }
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
        }
        if (BoltNetwork.isServer) ServerSideData.UpdateZeusData();
	}

    public void CheckForGameOver() {
        if (!BoltNetwork.isServer) return;
        if (CurrentGameState != GameState.IN_GAME) return;
        if (GameMode.GameOver()) {
            GameOver();
            //tell the clients to end the game
            //we do this here instead of in GameOver() because if the game ends due to the timer, the clients will already know that
            //and don't need the server to tell them to call GameOver()
            GameOverEvent.Create(Bolt.GlobalTargets.AllClients, Bolt.ReliabilityModes.ReliableOrdered).Send();
        }
    }


    internal void GameOver() {
        if (BoltNetwork.isServer) {
            GameMode.OnGameEnd();

        }
        ChangeGameState(GameState.POST_GAME_FADE);
        SpawningBlind blind = SpawningBlind.instance;
        if (blind != null) {
            blind.Text = "Game Over";
            blind.FadeIn(3f);
        }
        fadeTime = 0f;
        
        //more game over stuff here
    }
    private float fadeTime = 0f;

    void Update() {
        DebugHUD.setValue("GameState", System.Enum.GetName(typeof(GameState), CurrentGameState));
        DebugHUD.setValue("GameMode", GameMode == null ? "NULL" : GameMode.GetType().Name);
        //only the server needs to do this because it will change the scene and bring the clients along for the ride
        if (BoltNetwork.isServer && CurrentGameState == GameState.POST_GAME_FADE) {
            if (fadeTime < 3f) {
                fadeTime += Time.deltaTime;
            } else {
                ChangeGameState(GameState.POST_GAME);
            }
        }
        //both the client and server need to be able to detect that the timer has expired
        //this ensures that the game ends at the same time for all players
        if (CurrentGameState == GameState.IN_GAME) {
            float gameTime = BoltNetwork.serverTime - GameStartTime;
            if (gameTime >= GameMode.TimeLimit) {
                Debug.Log("Timer expired!");
                GameOver();
            }
        }
    }

    /// <summary>
    /// Jank-ASS disconnect network and go to MainMenu function.
    /// </summary>
    public void QuitToMainMenu()
    {
        // Disconnect network.
        if (BoltNetwork.isRunning)
        {
            BoltLauncher.Shutdown();
        }

        Application.LoadLevel(BoltScenes.MainMenu);
    }
}
