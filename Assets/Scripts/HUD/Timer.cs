using UnityEngine;
using UnityEngine.UI;
using System.Collections;

 public class Timer : MonoBehaviour {
     const int WARNING_TIME = 10;
     public Text timer;

     void Update() {
         if (GameManager.instance.CurrentGameState != GameManager.GameState.IN_GAME) {
             timer.text = "";
         } else if (float.IsPositiveInfinity(GameManager.instance.GameMode.TimeLimit)) {
             timer.text = ((char)236) + ""; //infinity symbol
             this.enabled = false;
         } else {
             float timeSinceStart = BoltNetwork.serverTime - GameManager.instance.GameStartTime;
             float timeRemaining = GameManager.instance.GameMode.TimeLimit - timeSinceStart;
             int time = (int)timeRemaining;
             int seconds = time % 60;
             int minutes = time / 60;
             bool color = time <= 10;
             timer.text = (color ? "<color=red>" : "") + (minutes > 0 ? minutes + ":" : "") + (seconds < 10 ? "0" : "") + seconds + (color ? "</color>" : "");
         }
     }
 }
