using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

[CustomEditor(typeof(TileObjects))]
public class TileObjectsEditor : Editor {

    private ReorderableList objectList;
    private ReorderableList materialList;

    void OnEnable()
    {
        materialList = new ReorderableList(serializedObject, serializedObject.FindProperty("tileMaterials"), true, true, true, true);
        materialList.serializedProperty.arraySize++;

        //Set how elements will be drawn both when selected and when not selected.
        materialList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
            var element = materialList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
        };

        materialList.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Tile Materials");
        };


        objectList = new ReorderableList(serializedObject, serializedObject.FindProperty("tileObjectData"), true, true, true, true);
        objectList.serializedProperty.arraySize++;

        //Set how elements will be drawn both when selected and when not selected.
        objectList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
            var element = objectList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
        };

        objectList.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Tile Objects");
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tilePrefab"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tileHighlightMat"));

        EditorGUILayout.Space();
        materialList.DoLayoutList();
        EditorGUILayout.Space();
        objectList.DoLayoutList();
        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();
    }
}