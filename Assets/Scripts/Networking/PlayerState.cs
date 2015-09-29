using UnityEngine;
using System.Collections;

public class PlayerState : Bolt.EntityBehaviour<IPlayerState> {

    public GameObject OwnerPrefab, ProxyPrefab;

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
        prefabTransform.GetComponent<AbstractPlayer>().SetState(this);
    }
}
