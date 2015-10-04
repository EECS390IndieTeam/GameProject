using UnityEngine;
using UnityEditor;
using System.Collections;

//this script draws a custom editor for the SpawnPoint Behaviour to make it easier to use
[CustomEditor(typeof(SpawnPoint))]
[CanEditMultipleObjects]
public class SpawnPointEditor : Editor {
    private string[] TeamLabels = { "0", "1", "2", "3", "4", "5", "6", "7" };
    private int[] TeamNumbers = { 0, 1, 2, 3, 4, 5, 6, 7 };

    private SerializedProperty IsSpawnPointProp, IsStartPointProp, UsableInTeamModesProp, TeamProp, UsableInFreeForAllProp, UsableInAnyGameModeProp, ValidGameModesProp;

    void OnEnable() {
        IsSpawnPointProp = serializedObject.FindProperty("IsSpawnPoint");
        IsStartPointProp = serializedObject.FindProperty("IsStartPoint");
        UsableInTeamModesProp = serializedObject.FindProperty("UsableInTeamModes");
        TeamProp = serializedObject.FindProperty("Team");
        UsableInFreeForAllProp = serializedObject.FindProperty("UsableInFreeForAll");
        UsableInAnyGameModeProp = serializedObject.FindProperty("UsableInAnyGameMode");
        ValidGameModesProp = serializedObject.FindProperty("ValidGameModes");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.PropertyField(IsSpawnPointProp, new GUIContent("Usable as a spawn point"));
        EditorGUILayout.PropertyField(IsStartPointProp, new GUIContent("Usable as a start point"));
        EditorGUILayout.PropertyField(UsableInTeamModesProp, new GUIContent("Usable in team modes"));
        EditorGUI.indentLevel++;
        if (UsableInTeamModesProp.hasMultipleDifferentValues) {
            EditorGUILayout.HelpBox("Can only multi-edit Team number if all selected spawn points are usable in team modes", MessageType.None, true);
        } else if (UsableInTeamModesProp.boolValue) {
            if (TeamProp.hasMultipleDifferentValues) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Team");
                if (GUILayout.Button("Multiple - click to change")) {
                    TeamProp.intValue = 0;
                }
                EditorGUILayout.EndHorizontal();
            } else {
                TeamProp.intValue = EditorGUILayout.IntPopup("Team", TeamProp.intValue, TeamLabels, TeamNumbers);
            }
        }
        EditorGUI.indentLevel--;
        EditorGUILayout.PropertyField(UsableInFreeForAllProp, new GUIContent("Valid in Free-for-All"));
        EditorGUILayout.PropertyField(UsableInAnyGameModeProp, new GUIContent("Valid in any game mode"));
        EditorGUI.indentLevel++;
        if (!UsableInAnyGameModeProp.boolValue) EditorGUILayout.PropertyField(ValidGameModesProp, true);
        EditorGUI.indentLevel--;

        serializedObject.ApplyModifiedProperties();
    }
}
