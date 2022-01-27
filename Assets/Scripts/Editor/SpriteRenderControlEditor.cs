using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpriteRenderControl))]
public class SpriteRenderControlEditor : Editor
{
    public override void OnInspectorGUI()
    {
        this.SortOnZ();
        this.DrawDefaultInspector();
        this.DrawLoadImageButton();
    }

    void SortOnZ()
    {
        var renderer = Selection.activeGameObject.GetComponent<SpriteRenderer>();
        renderer.sortingOrder = Mathf.RoundToInt(-100 * Selection.activeGameObject.transform.position.z);
    }

    void DrawLoadImageButton()
    {
        if (GUILayout.Button("Load Image"))
        {
            ((SpriteRenderControl)this.target).LoadImage();
        }
    }
}