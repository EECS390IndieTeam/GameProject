using UnityEngine;
using System.Collections;

public class StringStatTracker : AbstractStatTracker<string, IStringStatsObject> {

    public override string StatName {
        get {
            return state.StatName;
        }
        set {
            state.StatName = value;
        }
    }

    public override string GetValueForPlayer(int index) {
        return state.Data[index];
    }

    public override void SetValueForPlayer(int index, string newValue) {
        state.Data[index] = newValue;
    }

    public void Start() {
        DontDestroyOnLoad(this);
        if (BoltNetwork.isClient) GameStats.RegisterStringStat(this);
    }
}
