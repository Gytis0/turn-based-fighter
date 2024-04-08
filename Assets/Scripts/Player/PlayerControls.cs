using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    Camera cam;
    InputController inputActions;
    PlayerMovementController playerMovementController;

    Interactable focus;
    public LayerMask layerMask;
    bool waitingToGetInRadius = false;

    int UILayer;
    bool pointerOverUI = false;

    void Awake()
    {
        UILayer = LayerMask.NameToLayer("UI");
        Debug.Log("UILayer: " + UILayer);
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
        //if we're pressing on UI, return
        if (pointerOverUI) return;

        // Shooting a ray to where the mouse points
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        //checking what did we hit
        if (Physics.Raycast(ray, out hit))
        {
            CloseInteractionBox();

            playerMovementController.StartWalking(hit.point);
        }
    }

    void RightClick()
    {
        //if we're pressing on UI, return
        if (pointerOverUI) return;

        // Shooting a ray to where the mouse points
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        Interactable interactable;

        //checking what did we hit
        if (Physics.Raycast(ray, out hit, layerMask))
        {
            CloseInteractionBox();

            if (interactable = hit.transform.GetComponent<Interactable>())
            {
                //get reference to the object
                focus = hit.transform.GetComponent<Interactable>();

                //check if the player is in the radius
                if (focus.radius < Vector3.Distance(gameObject.transform.position, focus.transform.position + focus.GetRadiusOffset()))
                {
                    playerMovementController.StartFollowing(hit.transform, focus.radius);
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
                playerMovementController.StartRunning(hit.point);
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
}
