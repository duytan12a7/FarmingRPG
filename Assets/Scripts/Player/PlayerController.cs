using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : SingletonMonobehaviour<PlayerController>
{

    private PlayerMovement playerMovement;
    public PlayerMovement PlayerMovement => playerMovement;

    private PlayerAnimation playerAnimation;
    public PlayerAnimation PlayerAnimation => playerAnimation;

    protected override void Awake() 
    {
        base.Awake();

        playerMovement = GetComponent<PlayerMovement>();
        playerAnimation = GetComponent<PlayerAnimation>();
        
    }
}
