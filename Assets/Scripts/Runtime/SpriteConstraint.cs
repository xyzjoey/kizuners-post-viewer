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
    class TransformInfo
    {
        public Vector3 pos;
        public float scale;
    }

    public bool autoSort = true;
    public bool autoScale = false;
    public bool autoPosXY = false;

    private TransformInfo prevTransformInfo = null;

    SpriteConstraint()
    {
        EditorApplication.update += this.EditorUpdateCallback;
    }

    ~SpriteConstraint()
    {
        EditorApplication.update -= this.EditorUpdateCallback;
    }

    private void Start()
    {
        this.SetPrevInfo();
    }

    private void OnValidate()
    {
        this.SetPrevInfo();
    }

    private void SetPrevInfo()
    {
        if (this.prevTransformInfo == null)
        {
            this.prevTransformInfo = new TransformInfo();
        }
        
        this.prevTransformInfo.pos = this.transform.position;
        this.prevTransformInfo.scale = this.transform.localScale.z;
    }

    private void Reset()
    {
        this.SortOnZ();
    }

    private void EditorUpdateCallback()
    {
        if (this == null) // FIXME to not need to check null
        {
            return;
        }

        if (this.prevTransformInfo != null)
        {
            this.UpdateWithConstraint(this.prevTransformInfo.pos, this.prevTransformInfo.scale, Camera.main.transform.position);
        }

        this.SetPrevInfo();
    }

    public void UpdateWithConstraint(Vector3 prevPos, float prevScale, Vector3 referenceCameraPos)
    {
        if (prevPos == this.transform.position)
        {
            return;
        }

        if (this.autoScale)
        {
            this.ScaleOnZ(prevPos, prevScale, referenceCameraPos);
        }

        if (this.autoPosXY)
        {
            this.MoveOnZ(prevPos, referenceCameraPos);
        }

        if (this.autoSort)
        {
            this.SortOnZ();
        }
    }

    private void ScaleOnZ(Vector3 prevPos, float prevScale, Vector3 referenceCameraPos)
    {
        float scale = (this.transform.position.z - referenceCameraPos.z) / (prevPos.z - referenceCameraPos.z);

        this.transform.localScale = Vector3.one * prevScale * scale;
    }

    private void MoveOnZ(Vector3 prevPos, Vector3 referenceCameraPos)
    {
        float scale = (this.transform.position.z - referenceCameraPos.z) / (prevPos.z - referenceCameraPos.z);
        var newPos = referenceCameraPos + (prevPos - referenceCameraPos) * scale;

        this.transform.position = new Vector3(newPos.x, newPos.y, this.transform.position.z);
    }

    private void SortOnZ()
    {
        var renderer = this.GetComponent<SpriteRenderer>();
        renderer.sortingOrder = Mathf.RoundToInt(-100 * this.transform.position.z);
    }
#endif
}
