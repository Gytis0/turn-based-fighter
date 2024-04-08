using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    public delegate void PlayerItemDropped(int index);
    public static event PlayerItemDropped onItemDrop;
    public delegate void PlayerItemAdded(ItemData item, int index);
    public static event PlayerItemAdded onItemAdd;

    public delegate void PlayerArmorDropped(ArmorType armorType);
    public static event PlayerArmorDropped onArmorDrop;
    public delegate void PlayerArmorEquipped(Armor armor, int index);
    public static event PlayerArmorEquipped onArmorEquip;

    public delegate void PlayerItemSwitch(int indexFrom, int indexTo);
    public static event PlayerItemSwitch onSwitch;

    ItemSlot itemSlot;
    RectTransform itemInventoryBackground;
    RectTransform armorInventoryBackground;
    RectTransform imageRect;
    CanvasGroup canvasGroup;


    public void Start()
    {
        itemSlot = GetComponent<ItemSlot>();
        itemInventoryBackground = GameObject.FindGameObjectWithTag("Item Inventory").GetComponent<RectTransform>();
        armorInventoryBackground = GameObject.FindGameObjectWithTag("Armor Inventory").GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        imageRect = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(itemSlot.itemData != null)
        {
            imageRect.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsRectInside(imageRect, armorInventoryBackground) && !IsRectInside(imageRect, itemInventoryBackground))
        {
            if (itemSlot.index == -1) onArmorDrop(((Armor)itemSlot.itemData).GetArmorType());
            else onItemDrop(itemSlot.index);
        }

        imageRect.localPosition = new Vector2(0, 0);

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        ItemSlot otherItemSlot = eventData.pointerDrag.GetComponent<ItemSlot>();

        // Item dropped on armor slot
        if (itemSlot.index == -1)
        {
            // Item is an armor
            if (otherItemSlot.itemData.GetType() == typeof(Armor))
            {
                Armor armor = (Armor)otherItemSlot.itemData;
                if (itemSlot.itemData == null && itemSlot.armorSlot == armor.GetArmorType())
                {
                    onArmorEquip(armor, otherItemSlot.index);    
                }
                else return;
            }
            else return;
        }
        // Item dropped on item slot
        else
        {
            // Item is an armor coming from armor inventory
            if (otherItemSlot.index == -1)
            {
                if (itemSlot.itemData == null)
                {
                    onItemAdd(otherItemSlot.itemData, itemSlot.index);
                }
                else return;
            }
            else
            {
                int indexFrom = eventData.pointerDrag.GetComponent<ItemSlot>().index, indexTo = itemSlot.index;
                if (indexFrom == indexTo) return;

                onSwitch(indexFrom, indexTo);
            }
        }
    }

    bool IsRectInside(RectTransform R1, RectTransform R2)
    {
        Vector3[] imageVector = new Vector3[4];
        Vector3[] backgroundVector = new Vector3[4];

        R1.GetWorldCorners(imageVector);
        R2.GetWorldCorners(backgroundVector);

        for (int i = 0; i < 4; i++)
        {
            if (Contains(imageVector[i], backgroundVector[0][0], backgroundVector[2][0], backgroundVector[0][1], backgroundVector[1][1]))
            {
                return true;
            }
        }
        return false;
    }

    public bool Contains(Vector3 point, float xMin, float xMax, float yMin, float yMax)
    {
        return point.x >= xMin && point.x < xMax && point.y >= yMin && point.y < yMax;
    }

   
}