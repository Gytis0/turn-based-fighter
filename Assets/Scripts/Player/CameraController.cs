using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    GameObject player;
    Camera camera;

    [SerializeField] float clippingOffset;
    InputController inputActions;
    bool movingAround = false;

    Vector3 cameraStartPosition;

    Vector2 mouseMovement;
    float mouseScroll;
    float zoomIn = 0;

    [SerializeField]
    [Range(0.01f, 2f)]
    float movingAroundSpeed;

    [SerializeField]
    [Range(0.01f, 1f)]
    float zoomInRange;

    [SerializeField]
    [Range(0.01f, 1f)]
    float zoomInSpeed;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        camera = transform.GetChild(0).GetComponent<Camera>();
        cameraStartPosition = camera.transform.localPosition;

        inputActions = new InputController();

        inputActions.FindAction("StartMovingAround").started += x => MovingAroundState(true);
        inputActions.FindAction("StartMovingAround").canceled += x => MovingAroundState(false);
        inputActions.FindAction("MoveAround").performed += x => mouseMovement = x.ReadValue<Vector2>();

        inputActions.FindAction("Scroll").performed += x => mouseScroll = x.ReadValue<float>();
    }

    void Update()
    {
        if (mouseScroll > 0)
        {
            zoomIn += zoomInSpeed;
            zoomIn = Mathf.Clamp(zoomIn, 0f, zoomInRange);
            mouseScroll = 0;
        }
        else if (mouseScroll < 0)
        {
            zoomIn -= zoomInSpeed;
            zoomIn = Mathf.Clamp(zoomIn, 0f, zoomInRange);
            mouseScroll = 0;
        }

        if (movingAround)
        {
            transform.rotation = Quaternion.Euler(Mathf.Clamp(transform.eulerAngles.x + mouseMovement.y * movingAroundSpeed, 0, 40), transform.eulerAngles.y + mouseMovement.x * movingAroundSpeed, 0);
        }

        transform.position = player.transform.position;
        camera.transform.localPosition = Vector3.Lerp(cameraStartPosition, new Vector3(0, 2, 0), zoomIn);
        
        // Clip objects that are too close
        camera.nearClipPlane = Mathf.Clamp(Vector3.Distance(camera.transform.position, transform.position) - clippingOffset, 0.01f , 10f);
    }

    public void MovingAroundState(bool state)
    {
        movingAround = state;
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
