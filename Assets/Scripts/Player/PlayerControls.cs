using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    Camera cam;
    InputController inputActions;
    PlayerMovementController playerMovementController;

    Interactable focus;
    public LayerMask layerMask;
    bool waitingToGetInRadius = false;

    void Awake()
    {
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

    public void LeftClick()
    {
        //if we're pressing on UI, return
        if (EventSystem.current.IsPointerOverGameObject()) return;

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
        if (EventSystem.current.IsPointerOverGameObject()) return;

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
                if (focus.radius < Vector3.Distance(gameObject.transform.position, focus.transform.position))
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
            InteractionMenuBox.Instance.gameObject.SetActive(true);
            InteractionMenuBox.Instance.OpenInteractionBox(focus, focus.transform.position + new Vector3(0f, 2f, 0f));
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
}
