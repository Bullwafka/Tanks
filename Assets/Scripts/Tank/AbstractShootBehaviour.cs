using UnityEngine;

// Can be extended to add extra features like reload etc.
public abstract class AbstractShootBehaviour
{
    public bool IsActive => m_tank.IsActive;

    protected readonly Tank m_tank;
    protected AbstractProjectile m_projectiile;

    public AbstractShootBehaviour(Tank tank, AbstractProjectile projectiile)
    {
        m_tank = tank;
        m_projectiile = projectiile;
        m_projectiile.Team = m_tank.Team;
        InitializeInternal();
    }

    public abstract void Shoot(Vector3 direction);
    protected virtual void InitializeInternal() { }
}
