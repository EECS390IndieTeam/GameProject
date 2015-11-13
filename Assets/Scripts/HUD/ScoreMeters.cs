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
            PlayerScore.color = Teams.Colors[curPlayerTeam];
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
                OpponentScore.color = Teams.Colors[otherTeam];
                OpponentScore.text = "" + maxOtherScore;
            }
        } else {
            int currentPlayerScore = Lobby.GetStatForPlayer(GameManager.instance.CurrentUserName, currentGameMode.StatToDisplay);
            PlayerScore.text = "" + currentPlayerScore;

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

    void OnDestroy() {
        Lobby.LobbyUpdatedEvent -= Lobby_LobbyUpdatedEvent;
    }




}
