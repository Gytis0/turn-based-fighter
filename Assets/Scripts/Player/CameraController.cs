using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    GameObject player;

    InputController inputActions;
    bool movingAround = false;

    Vector2 mouseMovement;
    [SerializeField]
    [Range(0.01f, 2f)]
    float movingAroundSpeed;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        inputActions = new InputController();

        inputActions.FindAction("StartMovingAround").started += x => MovingAroundState(true);
        inputActions.FindAction("StartMovingAround").canceled += x => MovingAroundState(false);

        inputActions.FindAction("MoveAround").performed += x => mouseMovement = x.ReadValue<Vector2>();
    }

    void Update()
    {
        transform.position = player.transform.position;
        
        if(movingAround)
        {
            transform.Rotate(Vector3.up, mouseMovement.x * movingAroundSpeed);
        }
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
