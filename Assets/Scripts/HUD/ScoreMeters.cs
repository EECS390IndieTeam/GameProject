using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreMeters : MonoBehaviour {
    /// <summary>
    /// The score text of the current player
    /// </summary>
    public Text PlayerScore;

    /// <summary>
    /// The score text of the closest opponent
    /// </summary>
    public Text OpponentScore;

    /// <summary>
    /// A mapping of team numbers and colors
    /// </summary>
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

    void Awake() {
        Lobby.LobbyUpdatedEvent += Lobby_LobbyUpdatedEvent;
    }
    void Start() {
        UpdateText();
    }

    void Lobby_LobbyUpdatedEvent(Lobby.LobbyChange change) {
        UpdateText();
    }


    public void UpdateText() {
        IGameMode currentGameMode = GameManager.instance.GameMode;
        if (currentGameMode.UsesTeams) {
            //current player
            int curPlayerTeam = Lobby.GetPlayer(GameManager.instance.CurrentUserName).Team;
            PlayerScore.color = teamColors[curPlayerTeam];
            PlayerScore.text = "" + Lobby.GetStatForPlayer(Lobby.PP_TEAMS[curPlayerTeam], currentGameMode.StatToDisplay);
            //opponent
            int maxOtherScore = -1;
            int otherTeam = -1;
            for (int i = 1; i <= currentGameMode.MaxTeams; i++) {
                if (i != curPlayerTeam) {
                    int score = Lobby.GetStatForPlayer(Lobby.PP_TEAMS[i], currentGameMode.StatToDisplay);
                    if (score > maxOtherScore) {
                        maxOtherScore = score;
                        otherTeam = i;
                    }
                }
            }
            if (otherTeam != -1) {
                OpponentScore.color = teamColors[otherTeam];
                OpponentScore.text = "" + maxOtherScore;
            }
        } else {
            int currentPlayerScore = Lobby.GetStatForPlayer(GameManager.instance.CurrentUserName, currentGameMode.StatToDisplay);
            PlayerScore.text = "" + currentGameMode;

            int maxOtherScore = -1;
            foreach (var player in Lobby.AllPlayers) {
                if (player.Name != GameManager.instance.CurrentUserName) {
                    int score = player.GetStat(currentGameMode.StatToDisplay);
                    if (score > maxOtherScore) maxOtherScore = score;
                }
            }
            if (maxOtherScore != -1)
                OpponentScore.text = "" + maxOtherScore;
        }
    }




}
