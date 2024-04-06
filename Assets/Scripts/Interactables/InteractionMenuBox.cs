using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionMenuBox : MonoBehaviour
{
    public static InteractionMenuBox Instance { get; private set; }

    [SerializeField]
    List<Button> buttonList = new List<Button>();

    Interactable currentInteractable;

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

        DisableButtons();
        gameObject.SetActive(false);
    }

    public void OpenInteractionBox(Interactable _interactable)
    {
        currentInteractable = _interactable;
        transform.position = _interactable.transform.position + Vector3.up * 0.5f;
        EnableButtons(_interactable);
    }

    public void CloseInteractionBox()
    {
        gameObject.SetActive(false);
        DisableButtons();
    }

    void EnableButtons(Interactable _interactable)
    {
        int i = 0, interactionsCount = _interactable.interactions.Count;

        if (_interactable.interactions.Count == 0)
        {
            buttonList[0].gameObject.SetActive(true);
            buttonList[0].transform.GetComponentInChildren<TextMeshProUGUI>().SetText("Exit");
            return;
        }

        foreach (Button button in buttonList)
        {
            if (i < interactionsCount)
            {
                button.gameObject.SetActive(true);
                button.transform.GetComponentInChildren<TextMeshProUGUI>().SetText(_interactable.interactions[i].ToString());
            }
            else return;

            i++;
        }
    }

    void DisableButtons()
    {
        foreach (Button button in buttonList)
        {
            button.gameObject.SetActive(false);
        }
    }

    public void Interact(int i)
    {
        currentInteractable.StartInteraction(i);
        CloseInteractionBox();
    }
}
