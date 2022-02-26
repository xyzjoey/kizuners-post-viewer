using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpriteLoader))]
public class SpriteLoaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        this.DrawDefaultInspector();
        this.DrawButtons();
    }

    void DrawButtons()
    {
        if (GUILayout.Button("Load Sprite Reference"))
        {
            ((SpriteLoader)this.target).LoadSpriteReference();
        }
    }
}
