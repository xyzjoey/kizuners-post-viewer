using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonObjects : MonoBehaviour
{
    public SceneControl sceneControl;
    public CameraControl cameraControl;
    public ItemMenu itemMenu;
    public GameObject itemButtonContainer;
    public GameObject itemButtonPrefab;

    static private CommonObjects instance = null;

    static public CommonObjects Get()
    {
        if (instance == null)
        {
            instance = Object.FindObjectOfType<CommonObjects>();
        }

        return instance;
    }
}
