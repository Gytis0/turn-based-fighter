using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    public delegate void PlayerItemDropped(int index);
    public static event PlayerItemDropped onDrop;
    public delegate void PlayerItemSwitch(int indexFrom, int indexTo);
    public static event PlayerItemSwitch onSwitch;

    ItemSlot itemSlot;
    RectTransform background;
    RectTransform imageRect;
    Canvas canvas;
    CanvasGroup canvasGroup;


    public void Start()
    {
        itemSlot = GetComponent<ItemSlot>();
        background = GameObject.FindGameObjectWithTag("Inventory").GetComponent<RectTransform>();
        canvas = GameObject.FindGameObjectWithTag("UI Canvas").GetComponent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        imageRect = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(itemSlot.itemData != null)
        {
            //imageRect.anchoredPosition += eventData.delta / canvas.scaleFactor;
            imageRect.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(!IsRectInside(imageRect, background))
        {
            onDrop(itemSlot.index);
        }

        imageRect.localPosition = new Vector2(0, 0);

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnDrop(PointerEventData eventData)
    {
        int indexFrom = eventData.pointerDrag.GetComponent<ItemSlot>().index, indexTo = itemSlot.index;
        if (indexFrom == indexTo) return;

        Debug.Log("Dropping on: " + itemSlot.index);
        onSwitch(indexFrom, indexTo);
    }

    bool IsRectInside(RectTransform R1, RectTransform R2)
    {
        Vector3[] imageVector = new Vector3[4];
        Vector3[] backgroundVector = new Vector3[4];

        imageRect.GetWorldCorners(imageVector);
        background.GetWorldCorners(backgroundVector);

        for (int i = 0; i < 4; i++)
        {
            if (Contains(imageVector[i], backgroundVector[0][0], backgroundVector[2][0], backgroundVector[0][1], backgroundVector[1][1]))
            {
                return true;
            }
        }
        return false;
    }

    public bool Contains(Vector3 point, float xMin, float xMax, float yMin, float yMax)
    {
        return point.x >= xMin && point.x < xMax && point.y >= yMin && point.y < yMax;
    }

   
}