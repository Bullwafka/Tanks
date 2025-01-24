using UnityEngine;

public abstract class AbstractCameraMovementBehaviour 
{
    protected Transform m_rootMovable;

    public virtual void Initialize(Transform rootMovable, CameraSettings settings)
    {
        m_rootMovable = rootMovable;
        ApplySettings(settings);
    }

    public abstract void ApplySettings(CameraSettings settings);
    public abstract void MoveToPosition(Vector3 destination);
    public virtual void TeleportToPosition(Vector3 position)
    {
        m_rootMovable.position = position;
    }
}
