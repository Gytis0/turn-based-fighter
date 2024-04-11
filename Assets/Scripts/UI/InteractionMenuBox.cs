using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InteractionMenuBox : MonoBehaviour
{
    public static InteractionMenuBox Instance { get; private set; }

    [SerializeField] List<Button> buttonList = new List<Button>();
    [SerializeField] TMP_Text interactableName;
    List<Tuple<TMP_Text, TMP_Text>> textPairs = new List<Tuple<TMP_Text, TMP_Text>>();
    [SerializeField] List<GameObject> pairsGO = new List<GameObject>();

    [SerializeField] GameObject textSet1, textSet2;
    [SerializeField] Vector2 twoSetSize, oneSetSize, noSetSize;

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
        Tuple<TMP_Text, TMP_Text> temp;
        foreach (GameObject go in pairsGO)
        {
            temp = new Tuple<TMP_Text, TMP_Text>(go.transform.GetChild(0).GetComponent<TMP_Text>(), go.transform.GetChild(1).GetComponent<TMP_Text>());
            textPairs.Add(temp);
        }

        DisableButtons();
        gameObject.SetActive(false);
    }

    public void OpenInteractionBox(Interactable _interactable, Vector3 mousePos)
    {
        RectTransform rect = GetComponent<RectTransform>();
        currentInteractable = _interactable;
        float minX = rect.sizeDelta.x / 2;
        float minY = rect.sizeDelta.y / 2;
        float maxX = Screen.currentResolution.width - minX;
        float maxY = Screen.currentResolution.height - minY;

        transform.position = new Vector3(Math.Clamp(mousePos.x, minX, maxX), Math.Clamp(mousePos.y, minY, maxY), 0);
        if(_interactable.GetType() == typeof(Item))
        {
            Item item = (Item) _interactable;
            ItemData itemData = item.GetItemData();
            interactableName.SetText(itemData.GetName());

            if(itemData.GetType() == typeof(Weapon))
            {
                Weapon weapon = (Weapon)itemData;
                SetForStats(2, new string[]{ 
                    weapon.GetWeaponType().ToString(), "",
                    "Damage:", weapon.GetDamage().ToString(),
                    "Speed:", weapon.GetSpeed().ToString(),
                    "", "",
                    weapon.GetDamageType().ToString(), "",
                    "Durability:", weapon.GetDurability().ToString(),
                    "Weight:", weapon.GetWeight().ToString(),
                    "", ""
                });
            }
            else if (itemData.GetType() == typeof(Shield))
            {
                Shield shield = (Shield)itemData;
                SetForStats(1, new string[]{
                    "Durability", shield.GetDurability().ToString(),
                    "Weight:", shield.GetWeight().ToString(),
                    "", "",
                    "", "",
                    "", "",
                    "", "",
                    "", "",
                    "", ""
                });
            }
            else if (itemData.GetType() == typeof(Armor))
            {
                Armor armor = (Armor)itemData;
                Resistances[] resistances = armor.GetResistances();
                SetForStats(2, new string[]{
                    "Durability", armor.GetDurability().ToString(),
                    "Weight:", armor.GetWeight().ToString(),
                    "Armor Points", armor.GetArmorPoints().ToString(),
                    "", "",
                    "Resistances", "",
                    resistances[0].damageType.ToString(), resistances[0].resistance.ToString(),
                    resistances[1].damageType.ToString(), resistances[1].resistance.ToString(),
                    resistances[2].damageType.ToString(), resistances[2].resistance.ToString()
                });
            }
        }
        else
        {
            interactableName.SetText("Travel to a fight?");
            ClearStats();
        }
        EnableButtons(_interactable);
    }

    void ClearStats()
    {
        for (int i = 0; i < 8; i++)
        {
            textPairs[i].Item1.SetText("");
            textPairs[i].Item2.SetText("");
        }

        textSet1.SetActive(false);
        textSet2.SetActive(false);
        ((RectTransform)transform).sizeDelta = noSetSize;
    }

    void SetForStats(int sets, string[] texts)
    {
        if(sets == 0)
        {
            textSet1.SetActive(false);
            textSet2.SetActive(false);
            ((RectTransform)transform).sizeDelta = noSetSize;
        }
        else if(sets == 1)
        {
            textSet1.SetActive(true);
            textSet2.SetActive(false);
            ((RectTransform)transform).sizeDelta = oneSetSize;
        }
        else
        {
            textSet1.SetActive(true);
            textSet2.SetActive(true);
            ((RectTransform)transform).sizeDelta = twoSetSize;
        }

        for (int i = 0; i < 8; i++)
        {
            textPairs[i].Item1.SetText(texts[i*2]);
            textPairs[i].Item2.SetText(texts[i*2+1]);
        }
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
