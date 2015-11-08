using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Scoreboard : MonoBehaviour {

	public Text Players;
    public GameObject textElement;

    private Dictionary<Lobby.LobbyPlayer, string> players;
	
	public Text Kills;

    public Text Deaths;

    public Text Scores;
	
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
		UpdateScoreBoard();
	}
	
	void Lobby_LobbyUpdatedEvent(Lobby.LobbyChange change) {
		UpdateScoreBoard();
	}

    public void UpdateScoreBoard()
    {
        IGameMode currentGameMode = GameManager.instance.GameMode;
        //Players.text = "<b>Players</b>\n";
        //Kills.text = "<b>Kills</b>\n";

        //assume teams are used
        //TODO: not assume teams are used
        for(int i = 1; i <= currentGameMode.MaxTeams; i++)
        {
            //int teamScore = Lobby.GetStatForPlayer(Lobby.PP_TEAMS[i], GameManager.instance.GameMode.StatToDisplay);
            foreach (var player in Lobby.GetPlayersOnTeam(i))
            {
                Players.text += "\n\n" + player.Name;
                
                /*Text newText =  Instantiate(textElement, Vector3.zero, Players.transform.rotation) as Text;
                newText.rectTransform.SetParent(Players.transform, false);
                newText.rectTransform.position = Vector3.zero;
                //Text blah = newText.GetComponentInChildren<Text>();
                newText.text = player.Name;
                //newText.GetComponent(Text).text = player.Name;*/
            }

        }

    }
}
