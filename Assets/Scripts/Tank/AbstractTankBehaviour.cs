using UnityEngine;

public abstract class AbstractTankBehaviour
{
    public bool IsActive => m_tank.IsActive;
    protected readonly Rigidbody m_rigidbody;
    protected readonly Tank m_tank;

    protected float m_moveSpeed;
    protected float m_rotationSpeed;

    protected AbstractTankBehaviour(Tank tank)
    {
        m_tank = tank;
        m_rigidbody = m_tank.GetComponent<Rigidbody>();
        InitializeInternal();
    }

    public abstract void Move(Vector3 direction);
    public abstract void Rotate(float degrees);
    public abstract void Stop();

    public virtual void ApplySettings(TankConfig config)
    {
        m_moveSpeed = config.moveSpeed;
        m_rotationSpeed = config.rotationSpeed;
    }

    protected virtual void InitializeInternal() {}
}
