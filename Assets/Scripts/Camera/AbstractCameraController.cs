using System;
using UnityEngine;

[Serializable]
public struct CameraSettings
{
    [Range(0f, 1f)]
    [SerializeField] private float m_smoothness;
    public float smoothness => m_smoothness;
    [field: SerializeField] public float fieldOfView { get; private set; }
    [field: SerializeField] public Vector3 rotation { get; private set; }
    [field: SerializeField] public Vector3 offset { get; private set; }
}

public abstract class AbstractCameraController : MonoBehaviour
{
    protected CameraSettings m_cameraSettings;
    protected AbstractCameraMovementBehaviour m_cameraMovementBehaviour;

    public virtual void Initialize(AbstractCameraMovementBehaviour movementBehaviour, CameraSettings cameraSettings)
    {
        m_cameraSettings = cameraSettings;
        m_cameraMovementBehaviour = movementBehaviour;
        m_cameraMovementBehaviour.ApplySettings(m_cameraSettings);
    }
}
