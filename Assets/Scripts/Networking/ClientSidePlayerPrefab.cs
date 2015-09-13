using UnityEngine;
using System.Collections;

public class ClientSidePlayerPrefab : BoltSingletonPrefab<ClientSidePlayerPrefab> {
    public void AttachTo(Transform parent) {
        transform.parent = parent;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}
