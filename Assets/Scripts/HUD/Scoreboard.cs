using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
        //TODO: actually use this to optimize updates
        players = new Dictionary<Lobby.LobbyPlayer, GameObject>();
		UpdateScoreBoard();
	}

    void Update()
    {
        //show the detailed scores only while the proper key is held
        if (Input.GetKeyDown(KeyCode.H)) { panel.SetActive(true); }
        if (Input.GetKeyUp(KeyCode.H)) { panel.SetActive(false);  }
    }
	
	void Lobby_LobbyUpdatedEvent(Lobby.LobbyChange change) {
		UpdateScoreBoard();
	}

    /// <summary>
    /// Return the text element but bolded
    /// </summary>
    string boldText(string text)
    {
        return "<b>" + text + "</b>";
    }

    void updateTextElement()
    {
        //TODO: use this to destroy ungodly messes
    }

    public void UpdateScoreBoard()
    {
        IGameMode currentGameMode = GameManager.instance.GameMode;
        bool flags = currentGameMode.ToString() == "CaptureTheFlagMode";

        int index = 4; //used to find row number for player, first four elements are for headers
        if (currentGameMode.UsesTeams) //if teams are used
        {
            //I feel like this shouldn't work if Team 1 is skipped but somehow it works fine. Magic.
            //nevermind, works for team 1 and 2 but not 3 and up when only 1 player exists. Wonky.
            int teamNum = 0; //keep track of current team since a given team might be unused.
            for (int i = 1; i <= currentGameMode.MaxTeams; i++)
            {
                teamNum++;
                if (Lobby.GetPlayersOnTeam(i).Count() != 0)
                {
                    GameObject team_info = panel.transform.GetChild(index).gameObject;
                    team_info.SetActive(true);
                    team_info.transform.GetChild(0).GetComponent<Text>().text = boldText(teamNames[i]);
                    team_info.transform.GetChild(0).GetComponent<Text>().color = teamColors[i];
                    team_info.transform.GetChild(1).GetComponent<Text>().text = boldText(Lobby.GetStatForPlayer(Lobby.PP_TEAMS[i], "Kills").ToString());
                    team_info.transform.GetChild(1).GetComponent<Text>().color = teamColors[i];
                    team_info.transform.GetChild(2).GetComponent<Text>().text = boldText(Lobby.GetStatForPlayer(Lobby.PP_TEAMS[i], "Deaths").ToString());
                    team_info.transform.GetChild(2).GetComponent<Text>().color = teamColors[i];
                    if (flags)
                    {
                        team_info.transform.GetChild(3).GetComponent<Text>().text = boldText(Lobby.GetStatForPlayer(Lobby.PP_TEAMS[i], currentGameMode.StatToDisplay).ToString());
                        team_info.transform.GetChild(3).GetComponent<Text>().color = teamColors[i];
                    } else
                    {
                        team_info.transform.GetChild(3).gameObject.SetActive(false);
                    }
                    index++;
                }
                else {
                    print(teamNum + ": Best go again.");
                    if (teamNum == 7) { break; }  //the last team has no players so we done fools
                    else
                    {
                        i--; //we need to rerun iteration, teamNum will be one more
                        continue;
                    }
                }
                //int kills = 0; int deaths = 0; int score = 0;
                foreach (var player in Lobby.GetPlayersOnTeam(i))
                {
                    GameObject player_info = panel.transform.GetChild(index).gameObject;
                    players.Add(player, player_info);
                    player_info.SetActive(true);
                    //begin ungodly mess
                    player_info.transform.GetChild(0).GetComponent<Text>().text = player.Name;
                    player_info.transform.GetChild(0).GetComponent<Text>().color = teamColors[player.Team];
                    //kills += Lobby.GetStatForPlayer(player.Name, "Kills");
                    player_info.transform.GetChild(1).GetComponent<Text>().text = Lobby.GetStatForPlayer(player.Name, "Kills").ToString();
                    player_info.transform.GetChild(1).GetComponent<Text>().color = teamColors[player.Team];
                    //deaths += Lobby.GetStatForPlayer(player.Name, "Deaths");
                    player_info.transform.GetChild(2).GetComponent<Text>().text = Lobby.GetStatForPlayer(player.Name, "Deaths").ToString();
                    player_info.transform.GetChild(2).GetComponent<Text>().color = teamColors[player.Team];
                    //score += Lobby.GetStatForPlayer(player.Name, currentGameMode.StatToDisplay);
                    if (flags)
                    {
                        player_info.transform.GetChild(3).GetComponent<Text>().text = Lobby.GetStatForPlayer(player.Name, currentGameMode.StatToDisplay).ToString(); //I'm a bad person
                        player_info.transform.GetChild(3).GetComponent<Text>().color = teamColors[player.Team];
                    } else { player_info.transform.GetChild(3).gameObject.SetActive(false); }
                    index++;
                }

            }
        } else //if teams are not used
        {
            foreach (var player in Lobby.AllPlayers)
            {
                GameObject player_info = panel.transform.GetChild(index).gameObject;
                players.Add(player, player_info);
                player_info.SetActive(true);
                //here we go again
                player_info.transform.GetChild(0).GetComponent<Text>().text = player.Name;
                //player_info.transform.GetChild(0).GetComponent<Text>().color = teamColors[player.Team];
                player_info.transform.GetChild(1).GetComponent<Text>().text = Lobby.GetStatForPlayer(player.Name, "Kills").ToString();
                //player_info.transform.GetChild(1).GetComponent<Text>().color = teamColors[player.Team];
                player_info.transform.GetChild(2).GetComponent<Text>().text = Lobby.GetStatForPlayer(player.Name, "Deaths").ToString();
                //player_info.transform.GetChild(2).GetComponent<Text>().color = teamColors[player.Team];
                if (flags) {
                    player_info.transform.GetChild(3).GetComponent<Text>().text = Lobby.GetStatForPlayer(player.Name, currentGameMode.StatToDisplay).ToString(); //I should rethink my life
                    //player_info.transform.GetChild(3).GetComponent<Text>().color = teamColors[player.Team];
                } else { player_info.transform.GetChild(3).gameObject.SetActive(false); }
                index++;
            }
        }
    }
}
