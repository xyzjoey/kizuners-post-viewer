using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SortOnZ))]
public class SortOnZEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
        serializedObject.Update();

        //Update order
        var renderer = Selection.activeGameObject.GetComponent<SpriteRenderer>();
        renderer.sortingOrder = Mathf.RoundToInt(-100 * Selection.activeGameObject.transform.position.z);
        
        // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
        serializedObject.ApplyModifiedProperties();
    }
}