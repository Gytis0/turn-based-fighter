using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Items/Item")]
public class ItemData : ScriptableObject
{
    [SerializeField] protected string itemName;
    [SerializeField] protected Sprite icon;
    [SerializeField] protected int weight;

    [SerializeField] Vector3 rightHandOffset;
    [SerializeField] Vector3 leftHandOffset;
    [SerializeField] Vector3 leftHandRotation;
    [SerializeField] Vector3 rightHandRotation;

    [SerializeField] Quaternion dropAngle;

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

    public string GetName() { return itemName; }
}