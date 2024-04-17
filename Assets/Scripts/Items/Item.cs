using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Interactable
{
    [SerializeField]
    ItemData itemData;
    
    Rigidbody rigid;

    public Item(Item item)
    {
        interactions = item.interactions;
        radius = item.radius;
        radiusOffset = item.radiusOffset;
        itemData = item.itemData;
    }

    public void Start()
    {
        base.Start();
        rigid = GetComponent<Rigidbody>();
    }

    public ItemData GetItemData() { return itemData; }
    public void AppearInHand(HandSlot slot)
    {
        if (rigid == null) Start();

        rigid.isKinematic = true;
        rigid.useGravity = false;
        transform.localPosition = itemData.GetHandOffset(slot);
        transform.localRotation = itemData.GetRotationOffset(slot);
        gameObject.SetActive(true);
    }

    public void AppearInWorld()
    {
        if (rigid == null) Start();

        rigid.isKinematic = false;
        rigid.useGravity = true;
        transform.localPosition = Vector3.zero;
        transform.eulerAngles = Vector3.zero;
        gameObject.SetActive(true);
        transform.parent = null;
    }

    public void HideInInventory()
    {
        gameObject.SetActive(false);
    }
}
