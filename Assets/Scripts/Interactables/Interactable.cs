using cakeslice;
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
    public static event SelectEquip onPickUpAndEquip;

    public delegate void SelectTravel();
    public static event SelectTravel onTravel;

    protected Outline outline;

    protected void Start()
    {
        if(!(outline = gameObject.GetComponent<Outline>()))
        {
            outline = gameObject.AddComponent<Outline>();
            outline.enabled = false;
        }
    }
    public enum InteractionOptions
    {
        Attack,
        PickUp,
        Equip,
        Travel
    };

    public List<InteractionOptions> interactions = new List<InteractionOptions>();
    public float radius = 2f;
    [SerializeField]
    protected Vector3 radiusOffset;

    public Vector3 GetRadiusOffset() { return radiusOffset; }

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
        if (onPickUpAndEquip != null) onPickUpAndEquip(transform.gameObject);
    }

    void Travel()
    {
        if (onTravel != null) onTravel();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + radiusOffset, radius);
    }

    protected void OnMouseOver()
    {
        outline.enabled = true;
    }

    protected void OnMouseExit()
    {
        outline.enabled = false;
    }
}