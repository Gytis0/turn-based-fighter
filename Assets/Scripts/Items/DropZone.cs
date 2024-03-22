using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    GameObject player;
    [SerializeField]
    List<GameObject> items = new List<GameObject>();
    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("onDrop");

        ItemData itemData = eventData.selectedObject.GetComponent<ItemData>();

        ItemData temp;
        foreach(GameObject item in items)
        {
            temp = item.GetComponent<ItemData>();
            if(temp.GetName() == itemData.GetName())
            {
                Instantiate(item, transform.position, new Quaternion());
            }
        }
    }
}
