using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class Pose
{
    public Vector3 position;
    public Quaternion rotation; 
}

[CustomEditor(typeof(EditItemUtils))]
public class EditItemUtilsEditor : Editor
{
    private GameObject targetGameObject;
    

    public override void OnInspectorGUI()
    {
        this.targetGameObject = ((EditItemUtils)this.target).gameObject;

        this.DrawDefaultInspector();
        this.DrawButtons();
    }

    void DrawButtons()
    {
        // if (GUILayout.Button("Rename Sprite File Names with Alphabet"))
        // {
        //     this.RenameSpriteRecursively(this.targetGameObject);
        // }

        if (GUILayout.Button("Swap Item Pose in Alphabetical Order"))
        {
            this.SwapItemByName();
        }
    }

    private void SwapItemByName()
    {
        List<Item> unsortedItems = this.RetrieveItems();
        List<Item> sortedItems = new List<Item>(unsortedItems);
        sortedItems.Sort((item1, item2) =>
        {
            var name1 = item1.itemNameAlphabet != "" ? item1.itemNameAlphabet : item1.itemName;
            var name2 = item2.itemNameAlphabet != "" ? item2.itemNameAlphabet : item2.itemName;
            return name1.CompareTo(name2);
        });

        List<Pose> unsortedPoses = unsortedItems.ConvertAll(item => new Pose {position = item.transform.position, rotation = item.transform.rotation});

        for (int i = 0; i < sortedItems.Count; ++i)
        {
            this.ApplyPose(sortedItems[i], unsortedPoses[i]);
            sortedItems[i].transform.SetSiblingIndex(i);
        }
    }

    private void ApplyPose(Item item, Pose pose)
    {
        item.transform.position = pose.position;
        item.transform.rotation = pose.rotation;
    }

    private List<Item> RetrieveItems()
    {
        List<Item> items = new List<Item>();
        this.RetrieveItemsRecursively(items, this.targetGameObject);
        return items;
    }

    private void RetrieveItemsRecursively(List<Item> items, GameObject gameObject)
    {
        foreach (Transform child in gameObject.transform)
        {
            if (child.tag == Tag.Item)
            {
                items.Add(child.gameObject.GetComponent<Item>());
            }

            this.RetrieveItemsRecursively(items, child.gameObject);
        }
    }

    private void RenameSprite(Item item)
    {
        string originalName = item.itemName;
        string alphabetName = item.itemNameAlphabet;

        if (alphabetName == "")
        {
            return;
        }

        var asset = item.GetComponent<SpriteRenderer>().sprite;
        string assetPath = AssetDatabase.GetAssetPath(asset);
        string assetName = Path.GetFileName(assetPath);
        string newAssetName = assetName.Replace(originalName, alphabetName);

        string result = AssetDatabase.RenameAsset(assetPath, newAssetName);

        AssetDatabase.Refresh();

        if (result != "")
        {
            Debug.LogWarning("Failed to rename sprite: " + result);
        }
    }

    private void RenameSpriteRecursively(GameObject gameObject)
    {
        foreach (Transform child in gameObject.transform)
        {
            if (child.tag == Tag.Item)
            {
                this.RenameSprite(child.GetComponent<Item>());
            }

            this.RenameSpriteRecursively(child.gameObject);
        }
    }
}
