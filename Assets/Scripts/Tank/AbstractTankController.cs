using UnityEngine;

public abstract class AbstractTankController 
{
    public bool IsActive => m_tank.IsActive;

    protected readonly Tank m_tank;
    protected readonly AbstractTankBehaviour m_moveBehaviour;
    protected readonly AbstractShootBehaviour m_shootBehaviour;

    protected AbstractTankController(Tank tank, AbstractTankBehaviour moveBehaviour, AbstractShootBehaviour shootBehaviour)
    {
        m_tank = tank;
        m_moveBehaviour = moveBehaviour;
        m_shootBehaviour = shootBehaviour;
        InitializeInternal();
    }

    public virtual void Update() 
    {
        if (!IsActive)
        {
            return;
        }
    }
    protected virtual void InitializeInternal() {}

    public virtual void Shutdown() {}
}
