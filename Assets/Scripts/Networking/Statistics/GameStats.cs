using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

public static class GameStats {
    private static Dictionary<string, IntegerStatTracker> intStatMap = new Dictionary<string, IntegerStatTracker>();
    private static Dictionary<string, FloatStatTracker> floatStatMap = new Dictionary<string, FloatStatTracker>();
    private static Dictionary<string, StringStatTracker> stringStatMap = new Dictionary<string, StringStatTracker>();


    public static void RegisterIntegerStat(IntegerStatTracker sst) {
        intStatMap.Add(sst.StatName, sst);
    }

    public static void RegisterFloatStat(FloatStatTracker sst) {
        floatStatMap.Add(sst.StatName, sst);
    }

    public static void RegisterStringStat(StringStatTracker sst) {
        stringStatMap.Add(sst.StatName, sst);
    }


    public static void CreateNewIntegerStat(string statName) {
        if (!BoltNetwork.isServer) {
            throw new System.NotImplementedException("Only the server can create new stat objects");
        }
        if (intStatMap.ContainsKey(statName)) return;
        var obj = BoltNetwork.Instantiate(BoltPrefabs.IntegerStatObject);
        var sst = obj.GetComponent<IntegerStatTracker>();
        sst.StatName = statName;
        obj.name = "Int stat (" + statName + ")";
        RegisterIntegerStat(sst);
    }
    public static void CreateNewFloatStat(string statName) {
        if (!BoltNetwork.isServer) {
            throw new System.NotImplementedException("Only the server can create new stat objects");
        }
        if (floatStatMap.ContainsKey(statName)) return;
        var obj = BoltNetwork.Instantiate(BoltPrefabs.FloatStatObject);
        var sst = obj.GetComponent<FloatStatTracker>();
        sst.StatName = statName;
        obj.name = "Float stat (" + statName + ")";
        RegisterFloatStat(sst);
    }

    public static void CreateNewStringStat(string statName) {
        if (!BoltNetwork.isServer) {
            throw new System.NotImplementedException("Only the server can create new stat objects");
        }
        if (stringStatMap.ContainsKey(statName)) return;
        var obj = BoltNetwork.Instantiate(BoltPrefabs.StringStatObject);
        var sst = obj.GetComponent<StringStatTracker>();
        sst.StatName = statName;
        obj.name = "String stat (" + statName + ")";
        RegisterStringStat(sst);
    }

    public static void SetStringStat(int playerStatIndex, string statName, string value) {
        if(stringStatMap.ContainsKey(statName))
            stringStatMap[statName][playerStatIndex] = value;
    }

    public static void SetStringStat(string playerName, string statName, string value) {
        if(stringStatMap.ContainsKey(statName))
            stringStatMap[statName][playerName] = value;
    }

    public static void SetFloatStat(int playerStatIndex, string statName, float value) {
        if(floatStatMap.ContainsKey(statName))
            floatStatMap[statName][playerStatIndex] = value;
    }

    public static void SetFloatStat(string playerName, string statName, float value) {
        if (floatStatMap.ContainsKey(statName))
            floatStatMap[statName][playerName] = value;
    }

    public static void SetIntegerStat(int playerStatIndex, string statName, int value) {
        if (intStatMap.ContainsKey(statName))
            intStatMap[statName][playerStatIndex] = value;
    }

    public static void SetIntegerStat(string playerName, string statName, int value) {
        if (intStatMap.ContainsKey(statName))
            intStatMap[statName][playerName] = value;
    }

    public static float GetFloatStat(int playerStatIndex, string statName) {
        if (floatStatMap.ContainsKey(statName))
            return floatStatMap[statName][playerStatIndex];
        return 0f;
    }
    public static float GetFloatStat(string playerName, string statName) {
        if (floatStatMap.ContainsKey(statName))
            return floatStatMap[statName][playerName];
        return 0f;
    }

    public static int GetIntegerStat(int playerStatIndex, string statName) {
        if (intStatMap.ContainsKey(statName))
            return intStatMap[statName][playerStatIndex];
        return 0;
    }
    public static int GetIntegerStat(string playerName, string statName) {
        if (intStatMap.ContainsKey(statName))
            return intStatMap[statName][playerName];
        return 0;
    }

    public static string GetStringStat(int playerStatIndex, string statName) {
        if (stringStatMap.ContainsKey(statName))
            return stringStatMap[statName][playerStatIndex];
        else return null;
    }
    public static string GetStringStat(string playerName, string statName) {
        if (stringStatMap.ContainsKey(statName))
            return stringStatMap[statName][playerName];
        else return null;
    }

    public static void ClearAllStats() {
        foreach (IntegerStatTracker sst in intStatMap.Values) {
            GameObject.Destroy(sst.gameObject);
        }
        intStatMap.Clear();

        foreach (FloatStatTracker sst in floatStatMap.Values) {
            GameObject.Destroy(sst.gameObject);
        }
        floatStatMap.Clear();

        foreach (StringStatTracker sst in stringStatMap.Values) {
            GameObject.Destroy(sst.gameObject);
        }
        stringStatMap.Clear();
    }

    public static IntegerStatTracker GetFullIntegerStat(string statName) {
        if (intStatMap.ContainsKey(statName))
            return intStatMap[statName];
        return null;
    }
    public static FloatStatTracker GetFullFloatStat(string statName) {
        if (floatStatMap.ContainsKey(statName))
            return floatStatMap[statName];
        return null;
    }
    public static StringStatTracker GetFullStringStat(string statName) {
        if (stringStatMap.ContainsKey(statName))
            return stringStatMap[statName];
        return null;
    }

    /// <summary>
    /// constructs a database out of all stats.  the output is a dictionary pairing player names to dictionaries pairing stat names to stat values.
    /// This is not a function you should be calling often due to it instatntiating so many dictionaries.  
    /// This function can only be called on the server
    /// </summary>
    /// <returns></returns>
    public static Dictionary<string, Dictionary<string, object>> BuildStatDatabase() {
        if (!BoltNetwork.isServer) return null;
        Dictionary<string, Dictionary<string, object>> dict = new Dictionary<string, Dictionary<string, object>>();
        for (int playerIndex = 0; playerIndex < 16; playerIndex++) {
            string playerName = ServerConnectionEventListener.IndexMap.GetPlayerNameForIndex(playerIndex);
            if (string.IsNullOrEmpty(playerName)) break;
            if (!dict.ContainsKey(playerName)) dict[playerName] = new Dictionary<string, object>();
            foreach (var pair in stringStatMap) {
                dict[playerName][pair.Key] = pair.Value.GetValueForPlayer(playerIndex);
            }
            foreach (var pair in floatStatMap) {
                dict[playerName][pair.Key] = pair.Value.GetValueForPlayer(playerIndex);
            }
            foreach (var pair in intStatMap) {
                dict[playerName][pair.Key] = pair.Value.GetValueForPlayer(playerIndex);
            }
        }
        return dict;
    }
}
