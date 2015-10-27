using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class GameModeManager {
    //stores the IGameModes based on their indicies in the GameModes enum;
    public static IGameMode[] GameModes;

    //stores a list of human-readable game mode names that correspond to the instances in the GameModes property
    public static string[] GameModeNames;

    //maps the GameMode class names to their actual instance
    private static Dictionary<string, IGameMode> gameModes = new Dictionary<string,IGameMode>();

    //a dictionary to map game mode class names to lists of maps that can support that game mode
    private static Dictionary<string, List<string>> supportedMaps = new Dictionary<string, List<string>>();

    //a dictionary to map map class names to lists of the game modes they support
    private static Dictionary<string, List<string>> supportedGameModes = new Dictionary<string, List<string>>();

    /// <summary>
    /// This constant dictionary maps map names to human-readable names
    /// </summary>
    private static Dictionary<string, string> MapNameMap = new Dictionary<string, string>();

    /// <summary>
    /// Builds the mapNameMap with the values and stuff...
    /// </summary>
    private static void BuildMapNameMap(){
        MapNameMap.Add("kuiper", "Kuiper Stadium");
    }

    /// <summary>
    /// returns the human-readable alias for the given map.  if no alias has been defined, it just returns the map class name
    /// </summary>
    /// <param name="mapClassName"></param>
    /// <returns></returns>
    public static string GetHumanReadableNameForMap(string mapClassName) {
        if (MapNameMap.ContainsKey(mapClassName)) {
            return MapNameMap[mapClassName];
        } else {
            return mapClassName;
        }
    }

    /// <summary>
    /// returns a list of the class names of all maps that support the given game mode
    /// </summary>
    /// <param name="gameModeClassName"></param>
    /// <returns></returns>
    public static IEnumerable<string> GetSupportedMapsForGameMode(string gameModeClassName) {
        if(supportedMaps.ContainsKey(gameModeClassName))
            return supportedMaps[gameModeClassName];
        return new string[0];
    }

    /// <summary>
    /// returns a list of the class names of all maps that support the given game mode
    /// </summary>
    /// <param name="gameMode"></param>
    /// <returns></returns>
    public static IEnumerable<string> GetSupportedMapsForGameMode(IGameMode gameMode) {
        return GetSupportedMapsForGameMode(gameMode.GetType().Name);
    }

    /// <summary>
    /// returns a list of the IGameMode instances that support the given map
    /// </summary>
    /// <param name="mapClassName"></param>
    /// <returns></returns>
    public static IEnumerable<IGameMode> GetSupportedGameModesForMap(string mapClassName) {
        if(supportedGameModes.ContainsKey(mapClassName))
            return supportedGameModes[mapClassName].ConvertAll<IGameMode>(s => GetGameModeFromClassName(s));
        return new IGameMode[0];
    }

    /// <summary>
    /// returns the IGameMode instance that has the given class name
    /// </summary>
    /// <param name="gameModeClassName"></param>
    /// <returns></returns>
    public static IGameMode GetGameModeFromClassName(string gameModeClassName) {
        if(gameModes.ContainsKey(gameModeClassName))
            return gameModes[gameModeClassName];
        return null;
    }

    static GameModeManager() {
        BuildMapNameMap();
        //fill out the dictionary of game modes and supported map class names
        foreach (var scene in BoltScenes.AllScenes) {
            if (scene.StartsWith("ingame")) {
                string[] split = scene.Split('_');
                if (split.Length == 3) {
                    //split[0] = "ingame"
                    //split[1] = map name
                    //split[2] = game mode

                    //add to the mode->map dictionary
                    if (!supportedMaps.ContainsKey(split[2])) {
                        supportedMaps.Add(split[2], new List<string>());
                    }
                    supportedMaps[split[2]].Add(split[1]);

                    //now we add the game mode to the map->mode dictionary
                    if (!supportedGameModes.ContainsKey(split[1])) {
                        supportedGameModes.Add(split[1], new List<string>());
                    }
                    supportedGameModes[split[1]].Add(split[2]);
                }
            }
        }


        //create an instance of every class that implements IGameMode
        IGameMode[] GameModesInstances = AppDomain.CurrentDomain.GetAssemblies()
          .SelectMany(x => x.GetTypes())
          .Where(x => typeof(IGameMode).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
          .Select(x => (IGameMode)Activator.CreateInstance(x))
          .ToArray();
        //now, put those instances into an array with indicies corrsponding to their entries in the GameModes enum
        //also into the class name -> instance map
        GameModes = new IGameMode[Enum.GetNames(typeof(GameModes)).Length];
        GameModeNames = new string[GameModes.Length];
        foreach (IGameMode mode in GameModesInstances) {
            GameModes[(int)mode.Mode] = mode;
            string name = mode.GetType().Name;
            gameModes.Add(name, mode);
            GameModeNames[(int)mode.Mode] = name;
        }
    }
}
