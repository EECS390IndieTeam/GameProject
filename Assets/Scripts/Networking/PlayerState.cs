using UnityEngine;
using System.Collections;

public class PlayerState : Bolt.EntityBehaviour<IPlayerState> {

    public GameObject OwnerPrefab, ProxyPrefab;

    public IPlayer Player {
        get;
        private set;
    }

    public int Team {
        get {
            return state.Team;
        }
        set { 
            state.Team = value; 
        }
    }

    private Transform prefabTransform;
    

    public float Health {
        get {
            return state.Health;
        }
        set {
            state.Health = value;
        }
    }

    public string Name {
        get {
            return state.Name;
        }
        set {
            state.Name = value;
        }
    }

    public int SelectedWeapon {
        get {
            return state.SelectedWeapon;
        }
        set {
            state.SelectedWeapon = value;
        }
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

    public override void Attached() {
        if (entity.isOwner) {
            GameObject prefab = Instantiate(OwnerPrefab);
            state.Transform.SetTransforms(prefab.transform);
            prefabTransform = prefab.transform;
            if (BoltNetwork.isServer) {
                entity.TakeControl();
            } else {
                entity.AssignControl(BoltNetwork.server);
            }
        } else {
            GameObject prefab = Instantiate(ProxyPrefab);
            state.Transform.SetTransforms(prefab.transform);
            prefabTransform = prefab.transform;
        }
        prefabTransform.parent = this.transform;
        prefabTransform.localPosition = Vector3.zero;
        prefabTransform.localRotation = Quaternion.identity;
        Player = prefabTransform.GetComponent<IPlayer>();
        ((AbstractPlayer)Player).SetState(this);
        if (entity.isOwner) state.Name = GameManager.instance.CurrentUserName;

        //if this is the server's player we have to do some special stuff
        if (BoltNetwork.isServer && entity.isOwner) {
            PlayerRegistry.SetPlayer(Player);
        }
    }
}
