using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : SingletonMonobehaviour<PlayerController>
{
    public PlayerMovement PlayerMovement { get; private set; }

    public PlayerAnimation PlayerAnimation { get; private set; }

    protected override void Awake() 
    {
        base.Awake();

        PlayerMovement = GetComponent<PlayerMovement>();
        PlayerAnimation = GetComponent<PlayerAnimation>();
    }
}
