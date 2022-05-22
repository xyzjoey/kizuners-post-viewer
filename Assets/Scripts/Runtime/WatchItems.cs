using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WatchItems : MonoBehaviour
{
    private GameObject itemGroup;
    private ItemMenu itemMenu;

    private List<GameObject> itemGameObjects;
    private List<Item> items;

    private void Awake()
    {
        this.itemGroup = CommonObjects.Get().itemGroup;
        this.itemMenu = CommonObjects.Get().itemMenu;
    }

    private void Start()
    {
        this.itemGameObjects = this.RetrieveItemGameObjects();
        this.items = this.GetItems(this.itemGameObjects);
    }

    private void Update()
    {
        List<GameObject> newItemGameObjects = this.RetrieveItemGameObjects();

        if (this.itemGameObjects.Count == newItemGameObjects.Count)
        {
            return;
        }

        this.itemGameObjects = newItemGameObjects;
        this.items = this.GetItems(this.itemGameObjects);

        this.itemMenu.OnItemChanged(this.items);
    }

    private List<GameObject> RetrieveItemGameObjects()
    {
        List<GameObject> itemGameObjects = new List<GameObject>();

        this.RetrieveItemGameObjectsRecursively(itemGameObjects, this.itemGroup);

        return itemGameObjects;
    }

    private void RetrieveItemGameObjectsRecursively(List<GameObject> itemGameObjects, GameObject gameObject)
    {
        foreach (Transform child in gameObject.transform)
        {
            if (child.tag == Tag.Item)
            {
                itemGameObjects.Add(child.gameObject);
            }

            this.RetrieveItemGameObjectsRecursively(itemGameObjects, child.gameObject);
        }
    }

    private List<Item> GetItems(List<GameObject> itemGameObjects)
    {
        List<Item> items = new List<Item>();

        foreach (var gameObject in itemGameObjects)
        {
            items.Add(gameObject.GetComponent<Item>());
        }

        return items;
    }
}
