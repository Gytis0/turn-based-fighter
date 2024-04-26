using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Items/Item")]
public class ItemData : ScriptableObject
{
    [SerializeField] protected Vector3 rightHandOffset;
    [SerializeField] protected Vector3 leftHandOffset;
    [SerializeField] protected Vector3 leftHandRotation;
    [SerializeField] protected Vector3 rightHandRotation;

    [SerializeField] protected Quaternion dropAngle;

    [SerializeField] protected string itemName;
    [SerializeField] protected Sprite icon;
    [SerializeField] protected int weight;

    protected virtual ItemType itemType { get; set; }

    public ItemData(ItemData itemData)
    {
        rightHandOffset = itemData.rightHandOffset;
        leftHandOffset = itemData.leftHandOffset;
        leftHandRotation = itemData.leftHandRotation;
        rightHandOffset = itemData.rightHandOffset;

        dropAngle = itemData.dropAngle;

        itemName = itemData.itemName;
        icon = itemData.icon;
        weight = itemData.weight;
    }

    public ItemData()
    {

    }

    public Sprite GetIcon() { return icon; }
    public Vector3 GetHandOffset(HandSlot slot)
    {
        if (slot == HandSlot.LeftHand) return leftHandOffset;
        else return rightHandOffset;
    }
    public Quaternion GetRotationOffset(HandSlot slot)
    {
        if (slot == HandSlot.LeftHand) return Quaternion.Euler(leftHandRotation);
        else return Quaternion.Euler(rightHandRotation);
    }
    public Quaternion GetDropRotation() { return dropAngle; }
    public int GetWeight() { return weight; } 

    public string GetName() { return itemName; }
    public ItemType GetItemType () { return itemType; }
}