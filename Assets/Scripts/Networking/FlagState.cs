using UnityEngine;
using System.Collections;

public class FlagState : Bolt.EntityBehaviour<IFlagState> {

    public bool isEnabled {
        get {
            return state.isEnabled;
        }
        set { 
            state.isEnabled = value; 
        }
    }

    private Transform prefabTransform;
   
    public override void Attached() {
		Debug.Log ("Setting up a flag.");
		Debug.Log (this.gameObject);
	    state.Transform.SetTransforms(this.transform);
	    if (BoltNetwork.isServer) {
	    	entity.TakeControl();
	     } else {
	     	entity.AssignControl(BoltNetwork.server);
	     }
    }
}
