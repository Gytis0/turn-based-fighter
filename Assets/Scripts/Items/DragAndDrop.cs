using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public delegate void PlayerItemDropped(ItemData item);
    public static event PlayerItemDropped onDrop;

    ItemDataWrapper itemDataWrapper;
    RectTransform background;
    RectTransform imageRect;
    Canvas canvas;

    public void Start()
    {
        itemDataWrapper = GetComponent<ItemDataWrapper>();
        background = GameObject.FindGameObjectWithTag("Inventory").GetComponent<RectTransform>();
        canvas = GameObject.FindGameObjectWithTag("UI Canvas").GetComponent<Canvas>();
        imageRect = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        if(itemDataWrapper.itemData != null)
        {
            imageRect.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(!IsRectInside(imageRect, background))
        {
            onDrop(itemDataWrapper.itemData);
        }

        imageRect.localPosition = new Vector2(0, 0);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    bool IsRectInside(RectTransform R1, RectTransform R2)
    {
        Vector3[] imageVector = new Vector3[4];
        Vector3[] backgroundVector = new Vector3[4];

        imageRect.GetWorldCorners(imageVector);
        background.GetWorldCorners(backgroundVector);

        for (int i = 0; i < 4; i++)
        {
            if (Contains(imageVector[i], backgroundVector[0][0], backgroundVector[1][0], backgroundVector[0][1], backgroundVector[2][1]))
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