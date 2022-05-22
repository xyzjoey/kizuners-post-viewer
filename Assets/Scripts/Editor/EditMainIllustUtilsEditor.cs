using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EditMainIllustUtils))]
public class EditMainIllustUtilsEditor : Editor
{
    private EditMainIllustUtils targetObject;

    public override void OnInspectorGUI()
    {
        this.targetObject = (EditMainIllustUtils)this.target;

        this.DrawDefaultInspector();
        this.DrawButtons();
    }

    private void DrawButtons()
    {
        if (GUILayout.Button("Apply Position of MainIllustFlat"))
        {
            this.ApplyFlatPositionToAllParts();
        }
    }

    private void ApplyFlatPositionToAllParts()
    {
        var mainIllustPartMap = this.GenerateMainIllustPartMap(this.targetObject.mainIllustFlat, this.targetObject.gameObject);

        foreach (var tuple in mainIllustPartMap.Values)
        {
            var src = tuple.Item1;
            var dst = tuple.Item2;

            dst.GetComponent<SpriteConstraint>().UpdateWithConstraint(src.transform.position, src.transform.localScale.z, this.targetObject.referenceCamera.transform.position);
        }
    }

    private Dictionary<string, (GameObject, GameObject)> GenerateMainIllustPartMap(GameObject sourceMainIllust, GameObject targetMainIllust)
    {
        List<GameObject> sourceParts = new List<GameObject>();
        List<GameObject> targetParts = new List<GameObject>();

        this.FindDescendantObjectsWithSprite(sourceMainIllust, ref sourceParts);
        this.FindDescendantObjectsWithSprite(targetMainIllust, ref targetParts);

        var map = new Dictionary<string, (GameObject, GameObject)> ();

        foreach (var part in sourceParts)
        {
            map[part.name] = (part, null);
        }

        foreach (var part in targetParts)
        {
            if (map.ContainsKey(part.name))
            {
                map[part.name] = (map[part.name].Item1, part);
            }
        }

        return map;
    }

    private void FindDescendantObjectsWithSprite(GameObject parent, ref List<GameObject> foundObjects)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.GetComponent<SpriteRenderer>() != null)
            {
                foundObjects.Add(child.gameObject);
            }

            this.FindDescendantObjectsWithSprite(child.gameObject, ref foundObjects);
        }
    }
}
