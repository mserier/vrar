using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

[CustomEditor(typeof(Enemies))]
public class EnemiesEditor : Editor {

    private ReorderableList list;

    void OnEnable()
    {
        list = new ReorderableList(serializedObject, serializedObject.FindProperty("enemyData"), true, true, true, true);
        list.serializedProperty.arraySize++;

        //Set how elements will be drawn both when selected and when not selected.
        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
        };

        list.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Enemies");
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();        

        list.DoLayoutList();
        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();
    }
}