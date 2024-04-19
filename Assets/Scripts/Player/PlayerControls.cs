using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.InputSystem.HID;
using System;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    Camera cam;
    InputController inputActions;
    PlayerMovementController playerMovementController;

    Interactable focus;
    public LayerMask mouseRaycastLayers;
    bool waitingToGetInRadius = false;

    int UILayer;
    bool pointerOverUI = false;

    public bool restrictedMovement = false;

    void Awake()
    {
        UILayer = LayerMask.NameToLayer("UI");
        cam = Camera.main;
        inputActions = new InputController();
        playerMovementController = transform.GetComponent<PlayerMovementController>();

        inputActions.FindAction("LeftClick").performed += x => LeftClick();
        inputActions.FindAction("RightClick").performed += x => RightClick();
        inputActions.FindAction("Esc").performed += x => TogglePauseMenu();
    }

    void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
    }

    private void Update()
    {
        pointerOverUI = IsPointerOverUIElement();
    }


    public void LeftClick()
    {
        if (restrictedMovement) return;
        //if we're pressing on UI, return
        if (pointerOverUI) return;

        // Shooting a ray to where the mouse points
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        //checking what did we hit
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, mouseRaycastLayers);

        if (hits.Length > 0)
        {
            if(InteractionMenuBox.Instance.isActiveAndEnabled)
            {
                CloseInteractionBox();
            }
            else
            {
                RaycastHit hit = FindClosestHit(hits);

                playerMovementController.StartWalking(hit.point);
            }
        }
    }

    void RightClick()
    {
        if (restrictedMovement) return;

        //if we're pressing on UI, return
        if (pointerOverUI) return;

        // Shooting a ray to where the mouse points
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        Interactable interactable;

        //checking what did we hit
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, mouseRaycastLayers);
        if (hits.Length > 0)
        {
            RaycastHit hit = FindClosestHit(hits);

            if (interactable = hit.transform.GetComponent<Interactable>())
            {
                //get reference to the object
                focus = interactable;

                //check if the player is in the radius
                if (focus.radius < Vector3.Distance(gameObject.transform.position, focus.transform.position + focus.GetRadiusOffset()))
                {
                    playerMovementController.Reach(focus.transform.position + focus.GetRadiusOffset(), focus.radius);
                    waitingToGetInRadius = true;
                }
                else
                {
                    waitingToGetInRadius = true;
                    OpenInteractionBox();
                }
            }
            else
            {
                if (InteractionMenuBox.Instance.isActiveAndEnabled)
                {
                    CloseInteractionBox();
                }
                else
                {
                    playerMovementController.StartRunning(hit.point);
                }
            }
        }
    }

    void OpenInteractionBox()
    {
        if(waitingToGetInRadius)
        {
            Vector3 targetPos = cam.WorldToScreenPoint(focus.transform.position);
            InteractionMenuBox.Instance.gameObject.SetActive(true);
            InteractionMenuBox.Instance.OpenInteractionBox(focus, targetPos);
            waitingToGetInRadius = false;
        }   
    }

    void CloseInteractionBox()
    {
        InteractionMenuBox.Instance.CloseInteractionBox();
        InteractionMenuBox.Instance.gameObject.SetActive(false);
        waitingToGetInRadius = false;
    }

    private void OnEnable()
    {
        inputActions.Enable();
        PlayerMovementController.onPlayerReachedFollower += OpenInteractionBox;
    }

    private void OnDisable()
    {
        inputActions.Disable();
        PlayerMovementController.onPlayerReachedFollower -= OpenInteractionBox;
    }

    private bool IsPointerOverUIElement()
    {
        List<RaycastResult> eventSystemRaycastResults = GetEventSystemRaycastResults();
        for (int index = 0; index < eventSystemRaycastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaycastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer)
                return true;
        }
        return false;
    }

    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

    RaycastHit FindClosestHit(RaycastHit[] hits)
    {
        float closest = 999f;
        RaycastHit result = hits[0];
        foreach (RaycastHit hit in hits)
        {
            if (hit.distance < closest)
            {
                result = hit;
                closest = hit.distance;
            }
        }

        return result;
    }
}
