using UnityEngine;
using System.Collections;

public class IntegerStatTracker : AbstractStatTracker<int, IIntegerStatsObject> {

    public override string StatName {
        get {
            return state.StatName;
        }
        set {
            state.StatName = value;
        }
    }

    public override int GetValueForPlayer(int index) {
        return state.Data[index];
    }

    public override void SetValueForPlayer(int index, int newValue) {
        state.Data[index] = newValue;
    }

    public void Start() {
        DontDestroyOnLoad(this);
        if (BoltNetwork.isClient) GameStats.RegisterIntegerStat(this);
    }
}
