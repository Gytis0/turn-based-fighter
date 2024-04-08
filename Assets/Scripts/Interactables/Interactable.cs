using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public delegate void SelectAttack(GameObject enemy);
    public static event SelectAttack onAttack;

    public delegate void SelectPickUp(GameObject item);
    public static event SelectPickUp onPickUp;

    public delegate void SelectEquip(GameObject item);
    public static event SelectEquip onEquip;

    public enum InteractionOptions
    {
        Attack,
        PickUp,
        Equip
    };

    public List<InteractionOptions> interactions = new List<InteractionOptions>();
    public float radius = 2f;
    [SerializeField]
    Vector3 radiusOffset;

    public void StartInteraction(int i)
    {
        Invoke(interactions[i].ToString(), 0f);
    }

    void Attack()
    {
        if (onAttack != null) onAttack(transform.parent.gameObject);
    }

    void PickUp()
    {
        if (onPickUp != null) onPickUp(transform.gameObject);
    }

    void Equip()
    {
        if (onEquip != null) onEquip(transform.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + radiusOffset, radius);
    }
}