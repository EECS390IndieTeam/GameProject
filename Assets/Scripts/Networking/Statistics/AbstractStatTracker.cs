using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// Keeps track of a single "column" of statistics, and synchronizes it with bolt
/// </summary>
public abstract class AbstractStatTracker<T, TState> : Bolt.EntityBehaviour<TState>, ISingleStatTracker<T> {

    public abstract string StatName {
        get;
        set;
    }


    public T this[int index] {
        get { return GetValueForPlayer(index); }
        set { SetValueForPlayer(index, value); }
    }


    public T this[string name] {
        get { return GetValueForPlayer(name); }
        set { SetValueForPlayer(name, value); }
    }


    public abstract T GetValueForPlayer(int index);

    public T GetValueForPlayer(string name) {
        if (BoltNetwork.isServer) {
            return this[ServerConnectionEventListener.IndexMap.GetIndexForPlayer(name)];
        } else {
            return this[FindObjectOfType<LobbyState>().ConnectedPlayers.First(p => p.UserName == name).StatIndex];
        }
    }

    public abstract void SetValueForPlayer(int index, T newValue);

    public void SetValueForPlayer(string name, T newValue) {
        this[FindObjectOfType<LobbyState>().ConnectedPlayers.First(p => p.UserName == name).StatIndex] = newValue;
    }
}
