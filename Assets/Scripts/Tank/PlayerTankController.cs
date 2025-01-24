using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTankController : AbstractTankController
{
    private const string MOVE_ACTION_NAME = "Move";
    private const string ROTATE_ACTION_NAME = "Rotate";
    private const string SHOOT_ACTION_NAME = "Shoot";

    private InputAction m_moveAction;
    private InputAction m_rotateAction;
    private InputAction m_shootAction;

    public PlayerTankController(Tank tank, AbstractTankBehaviour moveBehaviour, 
                                AbstractShootBehaviour shootBehaviour) : 
                                base(tank, moveBehaviour, shootBehaviour) {}

    protected override void InitializeInternal()
    {
        base.InitializeInternal();

        InputsSystem inputsSystem = AllSystems.GetSystem<InputsSystem>();

        m_moveAction = inputsSystem.GetInputAction(MOVE_ACTION_NAME);
        m_rotateAction = inputsSystem.GetInputAction(ROTATE_ACTION_NAME);
        m_shootAction = inputsSystem.GetInputAction(SHOOT_ACTION_NAME);

        m_shootAction.performed += ShootActionPerformedHandler;
    }

    private void ShootActionPerformedHandler(InputAction.CallbackContext context)
    {
        if(!m_tank.IsActive)
        {
            return;
        }

        m_shootBehaviour.Shoot(m_tank.transform.forward);
    }

    public override void Update()
    {
        base.Update();

        float moveInput = m_moveAction.ReadValue<float>();
        m_moveBehaviour.Move(m_tank.transform.forward * moveInput);

        float rotationInput = m_rotateAction.ReadValue<float>();
        m_moveBehaviour.Rotate(rotationInput);
    }
}
