using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : SingletonMonobehaviour<PlayerController>
{
    public PlayerMovement PlayerMovement { get; private set; }

    public PlayerAnimation PlayerAnimation { get; private set; }

    public PlayerInputHandler PlayerInput { get; private set; }

    protected override void Awake() 
    {
        base.Awake();

        PlayerMovement = GetComponent<PlayerMovement>();
        PlayerAnimation = GetComponent<PlayerAnimation>();
        PlayerInput = GetComponent<PlayerInputHandler>();
    }

    public Vector3 GetPlayerCentrePosition()
    {
        return new Vector3(transform.position.x, transform.position.y + Settings.playerCentreYOffset, transform.position.z);
    }
}
