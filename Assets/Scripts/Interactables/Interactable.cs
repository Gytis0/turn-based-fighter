using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public delegate void SelectAttack(GameObject enemy);
    public static event SelectAttack onAttack;

    public delegate void SelectPickUp(GameObject item);
    public static event SelectPickUp onPickUp;

    public enum InteractionOptions
    {
        StartDialogue,
        Attack,
        PickUp
    };

    public List<InteractionOptions> interactions = new List<InteractionOptions>();
    public float radius = 2f;

    public void StartInteraction(int i)
    {
        Invoke(interactions[i].ToString(), 0f);
    }

    void StartDialogue()
    {
        DialogueManager.Instance.StartDialogue(transform.GetComponent<Dialogue>());
    }

    void Attack()
    {
        if (onAttack != null) onAttack(transform.parent.gameObject);
    }

    void Close()
    {
        InteractionMenuBox.Instance.CloseInteractionBox();
        InteractionMenuBox.Instance.gameObject.SetActive(false);
    }

    void PickUp()
    {
        if (onPickUp != null) onPickUp(transform.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}