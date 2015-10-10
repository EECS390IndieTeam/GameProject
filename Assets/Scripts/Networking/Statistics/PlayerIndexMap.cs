using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Maintains a map pairing each player's names to their StatIndex, to allow players to reconnect to games 
/// </summary>
public class PlayerIndexMap {
    private int nextId = 0;
    private Dictionary<string, int> map;
    private string[] reverseLookupTable;

    public PlayerIndexMap() {
        Clear();
    }

    /// <summary>
    /// clears the map
    /// </summary>
    public void Clear() {
        map = new Dictionary<string, int>(16);
        reverseLookupTable = new string[16];
        nextId = 0;
    }

    /// <summary>
    /// adds the given player to the map.  if they are already in the map, nothing will happen, and their id will be returned.  
    /// If they're not already in the map, an id will be assigned to the player, and it will be returned.  
    /// </summary>
    /// <param name="name">the player to add</param>
    /// <returns>The id for the given player</returns>
    public int AddPlayer(string name) {
        if (map.ContainsKey(name)) {
            return map[name];
        }
        int id = nextId;
        map.Add(name, id);
        reverseLookupTable[id] = name;
        nextId++;
        Debug.Log("Player " + name + " added to IndexMap with index " + id);
        return id;
    }

    /// <summary>
    /// returns true if the given player has been assigned an id
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool ContainsPlayer(string name) {
        return map.ContainsKey(name);
    }

    /// <summary>
    /// returns the id of the given player
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public int GetIndexForPlayer(string name) {
        if (map.ContainsKey(name)) {
            return map[name];
        } else return -1;
    }

    /// <summary>
    /// same as GetIndexForPlayer
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public int this[string name] {
        get { return GetIndexForPlayer(name); }
    }

    public string GetPlayerNameForIndex(int index) {
        return reverseLookupTable[index];
    }

    public int PlayerCount {
        get { return nextId; }
    }
}
