using System;
using UnityEngine;

public class AutoFollowPlayerCameraController : AbstractCameraController
{
    private Transform m_following;
    [SerializeField] private Camera m_camera;

    public override void Initialize(AbstractCameraMovementBehaviour movementBehaviour, CameraSettings cameraSettings)
    {
        base.Initialize(movementBehaviour, cameraSettings);
        
        m_following = AllSystems.GetSystem<TanksSpawnSystem>().Player.transform;

        Vector3 focusedOnPlayerPosition = m_following.position + m_cameraSettings.offset;

        movementBehaviour.TeleportToPosition(focusedOnPlayerPosition);
    }

    private void LateUpdate()
    {
        Vector3 desiredPosition = m_following.position + m_cameraSettings.offset;
        m_cameraMovementBehaviour.MoveToPosition(desiredPosition);
    }
}
