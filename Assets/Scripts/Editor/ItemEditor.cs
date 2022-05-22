using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    private Item item;

    public override void OnInspectorGUI()
    {
        this.item = (Item)this.target;

        this.DrawDefaultInspector();
        this.DrawButtons();
    }

    void DrawButtons()
    {
        if (GUILayout.Button("Load Image from url"))
        {
            this.LoadImage();
        }
    }

    private void LoadImage()
    {
        if (this.item.imageUrl == "")
        {
            Debug.LogWarning(this.item.name + ": Image url is empty.");

            return;
        }

        this.item.StartCoroutine(this.item.GetComponent<SpriteLoader>().LoadSpriteFromUrl(this.item.imageUrl));
    }
}
