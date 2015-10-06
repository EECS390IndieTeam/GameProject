using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class GameModeManager {
    public static IGameMode[] GameModes;
    public static string[] GameModeNames;
    public static GameModes[] GameModeTypes;

    static GameModeManager() {
        GameModes = AppDomain.CurrentDomain.GetAssemblies()
          .SelectMany(x => x.GetTypes())
          .Where(x => typeof(IGameMode).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
          .Select(x => (IGameMode)Activator.CreateInstance(x))
          .ToArray();
        GameModeNames = GameModes.Select(x => x.GameModeName).ToArray();
        GameModeTypes = GameModes.Select(x => x.Mode).ToArray();
    }
}
