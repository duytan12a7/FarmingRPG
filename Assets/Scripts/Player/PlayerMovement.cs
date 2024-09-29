using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController playerCtrl;

    private enum PlayerState
    {
        Idle,
        Walking,
        Running
    }
    private PlayerState currentState = PlayerState.Idle;

    public bool IsWalking() => currentState == PlayerState.Walking;
    public bool IsRunning() => currentState == PlayerState.Running;
    public bool IsIdle() => currentState == PlayerState.Idle;

    private Rigidbody2D rigidBody;
    private Camera mainCamera;

    private float xInput;
    public float XInput() => xInput;

    private float yInput;
    public float YInput() => yInput;

    private float movementSpeed;

    private bool _playerInputIsDisabled = false;

    public bool PlayerInputIsDisabled
    {
        get => _playerInputIsDisabled;
        set => _playerInputIsDisabled = value;
    }

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        playerCtrl = GetComponent<PlayerController>();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        Vector2 move = new(xInput * movementSpeed * Time.deltaTime, yInput * movementSpeed * Time.deltaTime);
        rigidBody.MovePosition(rigidBody.position + move);
    }

    public void UpdatePlayerState(float xInput, float yInput)
    {
        this.xInput = xInput;
        this.yInput = yInput;

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentState = PlayerState.Walking;
            movementSpeed = Settings.walkingSpeed;
        }
        else if (xInput != 0 || yInput != 0)
        {
            currentState = PlayerState.Running;
            movementSpeed = Settings.runningSpeed;
        }
        else
        {
            currentState = PlayerState.Idle;
            movementSpeed = 0;
        }
    }

    public void UpdatePlayerDirection(float xInput, float yInput)
    {
        if (xInput < 0) playerCtrl.PlayerInput.playerDirection = Direction.left;
        else if (xInput > 0) playerCtrl.PlayerInput.playerDirection = Direction.right;
        else if (yInput < 0) playerCtrl.PlayerInput.playerDirection = Direction.down;
        else if (yInput > 0) playerCtrl.PlayerInput.playerDirection = Direction.up;
    }

    public Vector3 GetPlayerViewportPosition() => mainCamera.WorldToViewportPoint(transform.position);

    public void ResetMovement()
    {
        xInput = 0f;
        yInput = 0f;
        currentState = PlayerState.Idle;
    }

    public void DisableMovement()
    {
        movementSpeed = 0f;
    }

    public void EnableMovement(float speed)
    {
        movementSpeed = speed;
    }

    public void DisablePlayerInputAndResetMovement()
    {
        DisablePlayerInput();
        playerCtrl.PlayerAnimation.CallMovementEvent();
    }

    public void EnablePlayerInput() => PlayerInputIsDisabled = false;

    public void DisablePlayerInput() => PlayerInputIsDisabled = true;
}
