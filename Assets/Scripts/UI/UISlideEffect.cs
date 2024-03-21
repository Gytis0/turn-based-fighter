using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UISlideEffect : MonoBehaviour
{
    RectTransform window;
    Vector2 openPos, closePos;

    [SerializeField]
    bool openOnStart = false;
    bool moving = false;
    bool open = false;

    float elapsedTime;
    float percentageComplete;

    public enum Directions
    { North, East, South, West };

    public Directions dock = Directions.North;

    InputController inputActions;


    [Header("Slide properties")]
    [SerializeField]
    float duration = 2f;
    [SerializeField]
    AnimationCurve curve;
    [SerializeField]
    string ActionName;

    private void Awake()
    {
        if(ActionName != null)
        {
            inputActions = new InputController();
            if(ActionName != string.Empty)
                inputActions.FindAction(ActionName).performed += x => OpenClose();
        }
        
    }
    private void Start()
    {
        window = transform.GetComponent<RectTransform>();
        openPos = window.position;

        if (dock == Directions.North)
            closePos = openPos + new Vector2(0, window.rect.height);
        else if (dock == Directions.East)
            closePos = openPos + new Vector2(window.rect.width, 0);
        else if(dock == Directions.South)
            closePos = openPos + new Vector2(0, -window.rect.height);
        else if(dock == Directions.West)
            closePos = openPos + new Vector2(-window.rect.width, 0);

        window.position = closePos;


        if(openOnStart)
        {
            OpenClose();
        }
    }

    public void OpenClose()
    {
        open = !open;
        moving = true;

        elapsedTime = 0;
    }

    private void Update()
    {
        if (!moving)
            return;

        elapsedTime += Time.deltaTime;
        percentageComplete = elapsedTime / duration;
        if(!open)
        {
            window.position = Vector2.Lerp(openPos, closePos, curve.Evaluate(percentageComplete));
            if (percentageComplete >= 1)
            {
                moving = false;
            }
                
        }
        else
        {
            window.position = Vector2.Lerp(closePos, openPos, curve.Evaluate(percentageComplete));
            if (percentageComplete >= 1)
            {
                moving = false;
            }
            
        }
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}
