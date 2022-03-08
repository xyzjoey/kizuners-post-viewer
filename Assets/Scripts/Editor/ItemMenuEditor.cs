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
        // for debug
        // if (GUILayout.Button("Reset Item Buttons"))
        // {
        //     ((ItemMenu)this.target).ResetItemsAndButtons();
        // }
    }
}
