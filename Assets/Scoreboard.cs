using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Scoreboard : MonoBehaviour {

	public Text Players;

    private Dictionary<Lobby.LobbyPlayer, GameObject> players;
	
	public Text Kills;

    public Text Deaths;

    public Text Scores;

    public GameObject panel; 
	
	public Color[] teamColors = {
		Color.white,                               //team 0
		Color.red,                                 //team 1
		Color.blue,                                //team 2
		Color.green,                               //team 3
		Color.yellow,                              //team 4
		Color.Lerp(Color.red,Color.yellow, 0.5f),  //team 5
		Color.Lerp(Color.red, Color.white, 0.5f),  //team 6
		Color.Lerp(Color.red, Color.blue, 0.5f)    //team 7
	};

    public string[] teamNames =
    {
        "White Team",
        "Red Team",
        "Blue Team",
        "Green Team",
        "Yellow Team",
        "Orange Team",
        "Pink Team",
        "Purple Team"
    };
	
	void Awake() {
		Lobby.LobbyUpdatedEvent += Lobby_LobbyUpdatedEvent;
	}

    void OnDestroy()
    {
        Lobby.LobbyUpdatedEvent -= Lobby_LobbyUpdatedEvent;
    }
	void Start() {
        players = new Dictionary<Lobby.LobbyPlayer, GameObject>();
		UpdateScoreBoard();
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) { panel.SetActive(!panel.active); }
    }
	
	void Lobby_LobbyUpdatedEvent(Lobby.LobbyChange change) {
		UpdateScoreBoard();
	}

    public void UpdateScoreBoard()
    {
        IGameMode currentGameMode = GameManager.instance.GameMode;

        //assume teams are used
        //TODO: not assume teams are used
        int index = 4; //used to find row number for player
        for(int i = 1; i <= currentGameMode.MaxTeams; i++)
        {
            //int teamScore = Lobby.GetStatForPlayer(Lobby.PP_TEAMS[i], GameManager.instance.GameMode.StatToDisplay);
            foreach (var player in Lobby.GetPlayersOnTeam(i))
            {
                GameObject player_info = panel.transform.GetChild(index).gameObject;
                players.Add(player, player_info);
                player_info.SetActive(true);
                
                player_info.transform.GetChild(0).GetComponent<Text>().text = player.Name;
                player_info.transform.GetChild(0).GetComponent<Text>().color = teamColors[player.Team];
                player_info.transform.GetChild(1).GetComponent<Text>().text = Lobby.GetStatForPlayer(player.Name, "Kills").ToString();
                player_info.transform.GetChild(1).GetComponent<Text>().color = teamColors[player.Team];
                player_info.transform.GetChild(2).GetComponent<Text>().text = Lobby.GetStatForPlayer(player.Name, "Deaths").ToString();
                player_info.transform.GetChild(2).GetComponent<Text>().color = teamColors[player.Team];
                player_info.transform.GetChild(3).GetComponent<Text>().text = Lobby.GetStatForPlayer(player.Name, currentGameMode.StatToDisplay).ToString();
                player_info.transform.GetChild(3).GetComponent<Text>().color = teamColors[player.Team];
                index++;
            }

        }

    }
}
