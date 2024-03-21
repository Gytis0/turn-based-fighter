using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField]
    TextMeshProUGUI charName;
    [SerializeField]
    TextMeshProUGUI dialogueText;
    List<Button> buttonList = new List<Button>();
    [SerializeField]
    UISlideEffect dialogueBoxSlider;

    Dialogue currentDialogue;
    List<string> optionNames = new List<string>();
    string currentNodeID;

    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        charName.SetText("NO_NAME");

        foreach (Transform child in transform.GetComponentsInChildren<Transform>())
        {
            if(child.GetComponent<Button>())
            {
                buttonList.Add(child.GetComponent<Button>());
            }
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        Debug.Log("Starting a dialogue");
        currentDialogue = dialogue;
        currentNodeID = dialogue.dialogueNodes.head.nexts[0].GUID;

        ShowDialogueText(currentNodeID);
        ShowOptions(currentDialogue.dialogueNodes.nodes[currentNodeID]);

        dialogueBoxSlider.OpenClose();
    }

    public void LoadNextNode(int i)
    {
        if(currentDialogue.dialogueNodes.nodes[currentNodeID].nexts.Count == 0)
        {
            dialogueBoxSlider.OpenClose();
            return;
        }


        currentNodeID = currentDialogue.dialogueNodes.nodes[currentNodeID].nexts[i].GUID;

        ShowDialogueText(currentNodeID);
        ShowOptions(currentDialogue.dialogueNodes.nodes[currentNodeID]);
    }

    public void ShowDialogueText(string nodeID)
    {
        dialogueText.text = currentDialogue.dialogueNodes.nodes[currentNodeID].dialogueText;
    }

    public void ShowOptions(Node node)
    {
        int i = 0;
        optionNames = node.options;

        if(optionNames.Count == 0)
        {
            optionNames.Add("End...");
        }

        //this might be slow
        foreach (Button button in buttonList)
        {
            if(i < optionNames.Count)
            {
                button.gameObject.SetActive(true);
                button.transform.GetComponentInChildren<TextMeshProUGUI>().SetText(optionNames[i]);
            }
            else
                button.gameObject.SetActive(false);

            i++;
        }
    }
}
