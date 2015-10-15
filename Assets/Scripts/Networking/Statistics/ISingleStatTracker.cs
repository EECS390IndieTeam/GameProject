using UnityEngine;
using System.Collections;

public interface ISingleStatTracker<T> {
    /// <summary>
    /// The name of this stat
    /// </summary>
    string StatName {
        get;
        set;
    }

    /// <summary>
    /// Gets the value of the stat for the player with the given StatIndex
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    T GetValueForPlayer(int statIndex);

    /// <summary>
    /// Gets the value for the given player by name
    /// NOTE: this function is EXTREMELY slow for clients and not recommended for use except for by the server.  
    /// On the server, it is extremely fast due to the IndexMap
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    T GetValueForPlayer(string playerName);
    /// <summary>
    /// Sets the value of the stat for the given player, by their statIndex
    /// </summary>
    /// <param name="statIndex"></param>
    /// <param name="value"></param>
    void SetValueForPlayer(int statIndex, T value);
    /// <summary>
    /// changes the value of the stat for the given player.  this function carries the same warning as GetValueForPlayer(string)
    /// </summary>
    /// <param name="name"></param>
    /// <param name="newValue"></param>
    void SetValueForPlayer(string playerName, T value);

    /// <summary>
    /// gets or sets the value for the given player by their StatIndex
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    T this[int index] {
        get;
        set;
    }

    /// <summary>
    /// gets or sets the value for the given player, by name
    /// NOTE: while this will work on clients, it is MUCH slower; try to avoid using this
    /// The server can use this with no slowdown
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>

    T this[string playerName] {
        get;
        set;
    }
}
