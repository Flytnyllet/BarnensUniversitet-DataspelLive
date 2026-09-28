using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputManager inputManager;
    PlayerLocomotion3rd playerLocomotion3Rd;
    PlayerTopDownController playerTopDownController;
    
    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerLocomotion3Rd = GetComponent<PlayerLocomotion3rd>();
        playerTopDownController = GetComponent<PlayerTopDownController>();
    }

    private void Update()
    {
        inputManager.HandleAllInputs();
    }

    private void FixedUpdate()
    {
        //playerLocomotion3Rd.HandleAllMovement();
        playerTopDownController.HandleAllMovement();
    }
}
