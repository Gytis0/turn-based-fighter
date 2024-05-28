using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timeline : MonoBehaviour
{
    [SerializeField] RectTransform parentCanvas;
    [SerializeField] RectTransform playerItemsTransform;
    [SerializeField] RectTransform enemyItemsTransform;
    [SerializeField] float timelineTotalSeconds = 15f;

    [SerializeField] GameObject action;

    float oneSecondWidth;

    [SerializeField] Color greenColor;
    [SerializeField] Color redColor;

    private void Start()
    {
        oneSecondWidth = transform.GetComponent<RectTransform>().sizeDelta.x / timelineTotalSeconds;
    }

    void FixedUpdate()
    {
        playerItemsTransform.Translate(Vector2.left * oneSecondWidth * 0.02f * parentCanvas.localScale.x);
        enemyItemsTransform.Translate(Vector2.left * oneSecondWidth * 0.02f * parentCanvas.localScale.x);
    }

    public void AddAction(float time, float duration, bool isPlayer, string name, Direction direction = Direction.None)
    {
        GameObject actionTemp;
        if (isPlayer)
            actionTemp = Instantiate(action, playerItemsTransform);
        else
            actionTemp = Instantiate(action, enemyItemsTransform);
        RectTransform rect = actionTemp.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(oneSecondWidth * duration, 40);

        if (isPlayer)
        {
            rect.anchorMax = new Vector2(0, 1);
            rect.anchorMin = new Vector2(0, 1);
            rect.anchoredPosition = new Vector3(time * oneSecondWidth + (oneSecondWidth * duration) / 2, -25f, 0f);
            actionTemp.transform.GetComponent<Image>().color = greenColor;
        }
        else
        {
            rect.anchorMax = new Vector2(0, 0);
            rect.anchorMin = new Vector2(0, 0);
            rect.anchoredPosition = new Vector3(time * oneSecondWidth + (oneSecondWidth * duration) / 2, 25f, 0f);
            actionTemp.transform.GetComponent<Image>().color = redColor;
        }

        actionTemp.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().SetText(name);
        if (direction == Direction.Forward) actionTemp.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, -90f);
        else if (direction == Direction.Right) actionTemp.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, -180f);
        else if (direction == Direction.Backward) actionTemp.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, -270f);
        else if (direction == Direction.None) actionTemp.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
    }

    public void RemoveActions(int count, bool isPlayer)
    {
        List<Transform> children = new List<Transform>();
        Transform targetTransform;
        if (isPlayer)
            targetTransform = playerItemsTransform;
        else
            targetTransform = enemyItemsTransform;

        int childCount = targetTransform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            children.Add(targetTransform.GetChild(i));
        }

        for (int i = children.Count - 1; i >= children.Count - count; i--)
        {
            DestroyAllChildren(children[i]);
        }
    }

    void DestroyAllChildren(Transform t)
    {
        if (t.childCount == 0) return;

        for (int i = t.childCount - 1; i >= 0; i--)
        {
            Destroy(t.GetChild(i).gameObject);
        }
        Destroy(t.gameObject);
    }
}
