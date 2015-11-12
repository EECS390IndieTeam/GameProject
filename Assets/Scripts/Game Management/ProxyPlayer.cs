using UnityEngine;
using System.Collections;

/// <summary>
/// This implemtation of AbstractPlayer is for the Proxy version of the Player (the 'other' players in the scene).  
/// Its functions mostly just send Bolt events to eventually call the same functions on the corresponding OwnerPlayer.  
/// </summary>
public class ProxyPlayer : AbstractPlayer {

	public MeshRenderer[] materials;

	void Start() {
		SetColor(Teams.Colors[this.Team]);
	}

    public override void Die(string killer, int weaponID) {
        if (!BoltNetwork.isServer) {
            Debug.LogError("Only the server is allowed to kill other players!");
            throw new System.NotImplementedException();
        }
    }

    public override void RespawnAt(Vector3 position, Quaternion rotation) {
        if (!BoltNetwork.isServer) {
            Debug.LogError("Only the server is allowed to respawn other players!");
            throw new System.NotImplementedException();
        }
        //tell the other player to move!
        PlayerMoveEvent evnt = PlayerMoveEvent.Create(PlayerRegistry.GetConnectionFromUserName(Username), Bolt.ReliabilityModes.ReliableOrdered);
        evnt.NewPosition = position;
        evnt.NewRotation = rotation;
        evnt.Respawn = true;
        evnt.Send();
    }

    public override void MoveTo(Vector3 position, Quaternion rotation) {
        //only the server is allowed to move other players
        if (!BoltNetwork.isServer) {
            Debug.LogError("Only the server is allowed to move other players!");
            throw new System.NotImplementedException();
        }
        //tell the other player to move!
        PlayerMoveEvent evnt = PlayerMoveEvent.Create(PlayerRegistry.GetConnectionFromUserName(Username), Bolt.ReliabilityModes.ReliableOrdered);
        evnt.NewPosition = position;
        evnt.NewRotation = rotation;
        evnt.Respawn = false;
        evnt.Send();
    }
    //attackerName should be the name of the player who dealt the damage
    public override void TakeDamage(float Damage, string attackerName, Vector3 direction, int weaponID) {
        //tell the other player to take damage
        HurtEvent evnt = HurtEvent.Create(Bolt.GlobalTargets.OnlyServer, Bolt.ReliabilityModes.ReliableOrdered);
        evnt.Amount = Damage;
        evnt.Source = attackerName;
        evnt.Target = this.Username;
        evnt.Direction = direction;
        evnt.WeaponID = weaponID;
        evnt.Send();
    }
	
	public override void SetColor(Color color) {
		foreach (MeshRenderer renderer in materials) {
			renderer.material.SetColor ("_EmissionColor", color);
		}
	}
}
