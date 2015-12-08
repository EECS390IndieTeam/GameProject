using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// An abstract implemtation of IPlayer.  This class contains all the code common to 
/// both OwnerPlayers and ProxyPlayers.  This class handles any IPlayer fields that need to
/// be synchronized by simply forwarding the gets and sets to the corresponding fields in the
/// PlayerState.  
/// </summary>
public abstract class AbstractPlayer : Bolt.GlobalEventListener, IPlayer {

    /// <summary>
    /// A reference to the attached PlayerState
    /// </summary>
    protected PlayerState state;

    public Transform HandPoint;

    /// <summary>
    /// The flag that this player can carry
    /// </summary>
    public GameObject flag;

    /// <summary>
    /// The object that needs to be hidden when carrying a flag
    /// </summary>
    public GameObject gun;

    /// <summary>
    /// These objects will be disabled when the player dies and re-enabled when the player respawns
    /// </summary>
    public GameObject[] ObjectsToDisableWhileDead;

    /// <summary>
    /// True of the player is currently dead
    /// </summary>
    public bool IsDead {
        get {
            return state.IsDead;
        }
        protected set {
            state.IsDead = value;
            foreach (var o in ObjectsToDisableWhileDead) o.SetActive(!value);
        }
    }

    /// <summary>
    /// Sets the PlayerState to use.  We can't simply use GetComponent because when the prefab
    /// containing the implementation of this script, it initially has no parent transform, so we don't
    /// have a good way to find the PlayerState.  instead, the PlayerState itself calls SetState(this) 
    /// after it is done with all its instantiation.  
    /// </summary>
    /// <param name="newState"></param>
    public void SetState(PlayerState newState) {
        state = newState;
        MaxHealth = Health;
    }


	public string Username {
		get{
            return state.Name;
        }
		protected set{
            state.Name = value;
        }
	}

    public int CurrentWeaponID {
        get {
            return state.SelectedWeapon;
        }
        protected set {
            state.SelectedWeapon = value;
        }
    }
	public int Team {
        get {
            return state.Team;
        }
        protected set {
            state.Team = value;
        }
	}
	public System.Collections.Generic.List<IWeapon> AvailableWeapons {
		get;
		protected set;
	}
	public IWeapon CurrentWeapon {
		get;
		protected set;
	}
	public float Health {
        get {
            return state.Health;
        }
        protected set {
            state.Health = value;
        }
	}

	public float MaxHealth {
		get;
		protected set;
	}

    public Vector3 GrappleEndpoint {
        get {
            return state.GrappleEndpoint;
        }
        set {
            state.GrappleEndpoint = value;
        }
    }

    public bool GrappleVisible {
        get {
            return state.GrappleVisible;
        }
        set {
            state.GrappleVisible = value;
        }
    }

	public Vector3 LaserEndpoint {
		get {
			return state.LaserEndpoint;
		}
		set {
			state.LaserEndpoint = value;
		}
	}
	
	public bool LaserVisible {
		get {
			return state.LaserVisible;
		}
		set {
			state.LaserVisible = value;
		}
	}

	public Vector3 MuzzlePoint {
		get {
			return state.MuzzlePoint;
		}
		set {
			state.MuzzlePoint = value;
		}
	}

	public bool HoldingFlag {
		get {
			return state.HoldingFlag;
		}
	}

    public Color HeldFlagColor {
        get {
            return state.HeldFlagColor;
        }
    }

    public int HeldFlagTeam {
        get {
            return state.HeldFlagTeam;
        }
    }

    /// <summary>
    /// Sets the flag that this player is carrying for networking purposes
    /// </summary>
    /// <param name="team"></param>
    protected void SetHeldFlagTeam(int team) {
        state.HeldFlagTeam = team;
    }

    public abstract void Die(string killer, int weaponID);

    public void RespawnAt(Transform location) {
        RespawnAt(location.position, location.rotation);
    }

    public abstract void RespawnAt(Vector3 position, Quaternion rotation);

    public abstract void MoveTo(Vector3 position, Quaternion rotation);

    public void MoveTo(Transform location) {
        MoveTo(location.position, location.rotation);
    }

    public abstract void TakeDamage(float Damage, string DamageSource, Vector3 direction, int weaponID);

	public abstract void SetColor(Color color);

    public void DropFlag() {
        if (!BoltNetwork.isServer) {
            throw new System.Exception("Only the server can take flags from other players");
        }
        FlagDroppedEvent evnt = FlagDroppedEvent.Create(Bolt.GlobalTargets.Everyone, Bolt.ReliabilityModes.ReliableOrdered);
        evnt.FlagTeam = this.HeldFlagTeam;
		evnt.Carrier = Username;
        evnt.Send();
    }

    public void PickupFlag(int team) {
        if (!BoltNetwork.isServer) {
            throw new System.Exception("Only the server can give flags to other players");
        }
        FlagGrabbedEvent evnt = FlagGrabbedEvent.Create(Bolt.GlobalTargets.Everyone, Bolt.ReliabilityModes.ReliableOrdered);
        evnt.FlagTeam = team;
        evnt.PlayerTeam = Team;
		evnt.CarrierName = Username;
//		CaptureTheFlagMode mode = (CaptureTheFlagMode)GameManager.instance.GameMode;
//		mode.SetFlagHolderForTeam(team, this);
        evnt.Send();
    }

    public override void OnEvent(FlagGrabbedEvent evnt) {
        if (evnt.CarrierName == this.Username) {
            SetHeldFlagTeam(evnt.FlagTeam);
            flag.SetActive(true);
            flag.GetComponentInChildren<Renderer>().materials[1].SetColor("_EmissionColor", Teams.Colors[evnt.FlagTeam] * 2);
            flag.GetComponentInChildren<Light>().color = Teams.Colors[evnt.FlagTeam];
            gun.SetActive(false);
            CaptureTheFlagMode mode = (CaptureTheFlagMode)GameManager.instance.GameMode;
            mode.GetCapPointForTeam(evnt.FlagTeam).FlagAtBase = false;
            mode.SetFlagHolderForTeam(evnt.FlagTeam, this);
        }
    }

    public override void OnEvent(FlagDroppedEvent evnt) {
        if (evnt.Carrier == this.Username) {
            dropFlag();
        }
    }

	private void dropFlag() {
		Debug.Log ("nulling out flag");
		CaptureTheFlagMode mode = (CaptureTheFlagMode)GameManager.instance.GameMode;
		mode.SetFlagHolderForTeam(this.HeldFlagTeam, null);
        SetHeldFlagTeam(0);
        flag.SetActive(false);
        gun.SetActive(true);
    }

    public override void OnEvent(FlagCapturedEvent evnt) {
        if (evnt.Player == this.Username) {
            dropFlag();
        }
    }

    public override void OnEvent(FlagReturnedEvent evnt) {
        if (evnt.Team == Team && evnt.Team == HeldFlagTeam) {
            dropFlag();
        }
    }
}
