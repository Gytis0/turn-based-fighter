using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    [SerializeField]
    Subtegral.DialogueSystem.DataContainers.DialogueContainer dialogue;
    public CustomUtility.LinkedList dialogueNodes;

    void Start()
    {
        dialogueNodes = new CustomUtility.LinkedList(dialogue);
    }

    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(this);
    }
}
