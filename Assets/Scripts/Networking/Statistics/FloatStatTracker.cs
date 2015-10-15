using UnityEngine;
using System.Collections;

public class FloatStatTracker : AbstractStatTracker<float, IFloatStatsObject> {

    public override string StatName {
        get {
            return state.StatName;
        }
        set {
            state.StatName = value;
        }
    }

    public override float GetValueForPlayer(int index) {
        return state.Data[index];
    }

    public override void SetValueForPlayer(int index, float newValue) {
        state.Data[index] = newValue;
    }

    public void Start() {
        DontDestroyOnLoad(this);
        if (BoltNetwork.isClient) GameStats.RegisterFloatStat(this);
    }
}
