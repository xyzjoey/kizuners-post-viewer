using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class CommonObjects : MonoBehaviour
{
    public PlayControl playControl;
    public CameraControl cameraControl;
    public Camera backgroundCamera;
    public GameObject itemGroup;
    public ItemMenu itemMenu;
    public GameObject itemButtonContainer;
    public GameObject itemButtonPrefab;

    public List<Item> items { get; private set; } = new List<Item>();

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
