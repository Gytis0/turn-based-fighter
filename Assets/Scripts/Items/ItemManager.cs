using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }
    public List<GameObject> items = new List<GameObject>();
    public static List<GameObject> itemsStatic = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        itemsStatic = items;
    }

    public static GameObject GetItemObject(string itemName)
    {
        ItemData temp;
        foreach (GameObject item in itemsStatic)
        {
            temp = item.GetComponent<Item>().GetItemData();
            if (temp.GetName() == itemName)
            {
                return item;
            }
        }

        return null;
    }

}
