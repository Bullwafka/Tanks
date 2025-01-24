using UnityEngine;

public class InterpolatedCameraMovement : AbstractCameraMovementBehaviour
{
    private float m_linearStep;

    public override void ApplySettings(CameraSettings settings)
    {
        m_linearStep = settings.smoothness;
    }

    public override void MoveToPosition(Vector3 destination)
    {
        Vector3 delta = Vector3.Lerp(m_rootMovable.position, destination, m_linearStep * m_linearStep * m_linearStep);
        m_rootMovable.position = delta;
    }
}
