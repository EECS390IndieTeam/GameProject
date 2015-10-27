using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class FlagCapturePoint : MonoBehaviour {
    void Awake() {
        if (!BoltNetwork.isServer) enabled = false;
    }
    
    public int teamID; //Team who owns this capture point and can score points here

    void OnTriggerEnter(Collider other)
    {
        if (!BoltNetwork.isServer) return;
        IGameMode currentGameMode = GameManager.instance.gameMode;
        if(GameManager.instance.gameMode.Mode == GameModes.CAPTURE_THE_FLAG)
        {
            CaptureTheFlagMode mode = (CaptureTheFlagMode)currentGameMode;
            Flag f = other.gameObject.GetComponent<Flag>();
            if (f != null)
            {
                if(f.player != null && f.player.Team == teamID)
                {
                    if(f.teamID == teamID)
                    {
                        //We are returning the flag to our base
                        mode.setFlagAtBase(teamID,true);
                        f.ReturnFlag();
                    } else
                    {
                        //The flag we are returning is not ours. Check and see if ours is returned and if so, you score!
                        if (mode.isFlagAtBaseForTeam(teamID))
                        {
                            //update scores
                            if (ServerConnectionEventListener.IndexMap.ContainsPlayer(f.player.Username)) {
                                int playerStatIndex = ServerConnectionEventListener.IndexMap[f.player.Username];
                                GameStats.SetIntegerStat(playerStatIndex, "Flags", GameStats.GetIntegerStat(playerStatIndex, "Flags") + 1);
                            }

                            GameManager.instance.CheckForGameOver();
                            f.ReturnFlag();
                        }
                    }
                    
                }
            }
        }
    }
}
