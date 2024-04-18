using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropEquipment : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    public delegate void PlayerItemDropped(int index);
    public static event PlayerItemDropped onItemDrop;
    public delegate void PlayerItemAddedFromArmor(ItemData item, int index);
    public static event PlayerItemAddedFromArmor onItemAddFromArmor;

    public delegate void PlayerArmorDropped(ArmorType armorType);
    public static event PlayerArmorDropped onArmorDrop;
    public delegate void PlayerArmorEquipped(Armor armor, int index);
    public static event PlayerArmorEquipped onArmorEquip;

    public delegate void PlayerItemSwitch(int indexFrom, int indexTo);
    public static event PlayerItemSwitch onSwitch;

    public delegate void PlayerItemAddedFromWeapon(ItemData item, int index);
    public static event PlayerItemAddedFromWeapon onItemAddFromWeapon;

    public delegate void PlayerEquippedWeapon(Item item, HandSlot handSlot, int index);
    public static event PlayerEquippedWeapon onWeaponEquip;

    public delegate void PlayerUnequippedWeapon(HandSlot handSlot);
    public static event PlayerUnequippedWeapon onWeaponUnequip;

    public ItemSlot itemSlot;
    RectTransform imageRect;
    CanvasGroup canvasGroup;

    [SerializeField] SlotType slotType;
    public void Start()
    {
        itemSlot = GetComponent<ItemSlot>();
        canvasGroup = GetComponent<CanvasGroup>();
        imageRect = GetComponent<RectTransform>();

        itemSlot.slotType = slotType;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (itemSlot.itemData != null)
        {
            imageRect.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        imageRect.localPosition = new Vector2(0, 0);

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        ItemSlot otherItemSlot = eventData.pointerDrag.GetComponent<ItemSlot>();

        // Item dropped on armor slot
        if (slotType == SlotType.Armor)
        {
            // Item is not an armor
            if (otherItemSlot.itemData.GetItemType() != ItemType.Armor) return;

            // If the slot is already full
            if (itemSlot.itemData != null) return;

            // If armor types don't match
            Armor armor = (Armor)otherItemSlot.itemData;
            if (itemSlot.armorSlot != armor.GetArmorType()) return;

            onArmorEquip(armor, otherItemSlot.index);
        }
        else if (slotType == SlotType.Weapon)
        {
            // Item is not a weapon
            if (otherItemSlot.itemData.GetItemType() != ItemType.Weapon) return;

            // If the slot is already full
            if (itemSlot.itemData != null) return;

            Weapon weapon = (Weapon)otherItemSlot.itemData;
            Item weaponItem = ItemManager.Instance.GetItem(weapon.GetName());
            if(itemSlot.index == 0)
            {
                onWeaponEquip(weaponItem, HandSlot.LeftHand, otherItemSlot.index);
            }
            else if (itemSlot.index == 1)
            {
                onWeaponEquip(weaponItem, HandSlot.RightHand, otherItemSlot.index);
            }
        }
        // Item dropped on item slot
        else
        {
            // Item is an armor coming from armor inventory
            if (otherItemSlot.slotType == SlotType.Armor)
            {
                if (itemSlot.itemData == null)
                {
                    onItemAddFromArmor(otherItemSlot.itemData, itemSlot.index);
                }
                else return;
            }
            // Item is a weapon coming from weapon inventory
            else if (otherItemSlot.slotType == SlotType.Weapon)
            {
                if (itemSlot.itemData == null)
                {
                    onItemAddFromWeapon(otherItemSlot.itemData, itemSlot.index);

                    if (otherItemSlot.index == 0)
                    {
                        onWeaponUnequip(HandSlot.LeftHand);
                    }
                    else if (otherItemSlot.index == 1)
                    {
                        onWeaponUnequip(HandSlot.RightHand);
                    }
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

    public bool Contains(Vector3 point, float xMin, float xMax, float yMin, float yMax)
    {
        return point.x >= xMin && point.x < xMax && point.y >= yMin && point.y < yMax;
    }


}