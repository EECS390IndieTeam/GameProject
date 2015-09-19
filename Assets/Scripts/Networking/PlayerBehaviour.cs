using UnityEngine;
using System.Collections;

public class PlayerBehaviour : Bolt.EntityBehaviour<IPlayerState> {

    public GameObject OwnerPrefab, ProxyPrefab;

    private Transform prefabTransform;
    

    public float Health {
        get {
            return state.Health;
        }
    }

    public string Name {
        get {
            return state.Name;
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
    }
}
