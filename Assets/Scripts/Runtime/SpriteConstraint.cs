using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[InitializeOnLoadAttribute]
#endif
public class SpriteConstraint : MonoBehaviour
{
#if UNITY_EDITOR

    public bool autoSort = true;
    public bool autoScale = false;
    public bool autoPosXY = false;

    public Vector3 expectedCameraPos = new Vector3(0, 0, -50);

    private Vector3 prevPos;

    SpriteConstraint()
    {
        EditorApplication.update += this.EditorUpdateCallback;
    }

    void Start()
    {
        this.prevPos = this.transform.position;
    }

    void OnValidate()
    {
        this.prevPos = this.transform.position;
    }

    void Reset()
    {
        this.SortOnZ();
    }

    void EditorUpdateCallback()
    {
        if (this == null) // FIXME to not need to check null
        {
            return;
        }

        this.SortOnZ();
        this.ScaleOnZ();
        this.MoveOnZ();
    }

    void ScaleOnZ()
    {
        if (!this.autoScale)
        {
            return;
        }

        float scale = (this.transform.position.z - expectedCameraPos.z) / (0 - expectedCameraPos.z);
        this.transform.localScale = Vector3.one * scale;
    }

    void MoveOnZ()
    {
        if (!this.autoPosXY || this.prevPos == this.transform.position)
        {
            return;
        }

        float scale = (this.transform.position.z - expectedCameraPos.z) / (this.prevPos.z - expectedCameraPos.z);
        var newPos = expectedCameraPos + (this.prevPos - expectedCameraPos) * scale;

        this.transform.position = new Vector3(newPos.x, newPos.y, this.transform.position.z);
        this.prevPos = this.transform.position;
    }

    void SortOnZ()
    {
        if (!this.autoSort)
        {
            return;
        }

        var renderer = this.GetComponent<SpriteRenderer>();
        renderer.sortingOrder = Mathf.RoundToInt(-100 * this.transform.position.z);
    }
#endif
}
