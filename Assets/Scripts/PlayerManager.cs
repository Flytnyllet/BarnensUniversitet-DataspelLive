using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputManager inputManager;
    PlayerLocomotion3rd _playerLocomotion3Rd;
    PlayerTopDownController _playerTopDownController;
    
    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        _playerLocomotion3Rd = GetComponent<PlayerLocomotion3rd>();
        _playerTopDownController = GetComponent<PlayerTopDownController>();
    }

    private void Update()
    {
        inputManager.HandleAllInputs();
    }

    private void FixedUpdate()
    {
        //_playerLocomotion3Rd.HandleAllMovement();
        _playerTopDownController.HandleAllMovement();
    }
}
