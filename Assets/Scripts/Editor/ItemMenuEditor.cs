using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemMenu))]
public class ItemMenuEditor : Editor
{
    public override void OnInspectorGUI()
    {
        this.DrawDefaultInspector();
        this.DrawResetContentButton();
    }

    void DrawResetContentButton()
    {
        if (GUILayout.Button("Reset Item Buttons"))
        {
            ((ItemMenu)this.target).RecreateItemButtons();
        }

        if (GUILayout.Button("Clear Item Buttons"))
        {
            ((ItemMenu)this.target).ClearItemButtons();
        }
    }
}
