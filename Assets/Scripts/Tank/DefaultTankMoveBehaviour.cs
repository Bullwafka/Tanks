using UnityEngine;
using UnityEngine.InputSystem;

public class DefaultTankMoveBehaviour : AbstractTankBehaviour
{
    private Transform m_transform => m_rigidbody.transform;
    public DefaultTankMoveBehaviour(Tank tank) : base(tank) {}

    public override void Move(Vector3 direction)
    {
        m_rigidbody.velocity = direction * m_moveSpeed;
    }

    public override void Stop()
    {
        m_rigidbody.velocity = Vector3.zero;
    }

    public override void Rotate(float degrees)
    {
        Quaternion rotation = Quaternion.Euler(
                            m_transform.eulerAngles.x, 
                            m_transform.eulerAngles.y + (degrees * m_rotationSpeed), 
                            m_transform.eulerAngles.z);

        m_transform.rotation = rotation;
    }
}
