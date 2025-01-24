using UnityEngine;

public sealed class DefaultShootBehaviour : AbstractShootBehaviour
{
    private float m_lastShootTime;

    private ObjectPool<AbstractProjectile> m_projectilesPool;
    public DefaultShootBehaviour(Tank tank, AbstractProjectile projectiile) : base(tank, projectiile) { }

    public override void Shoot(Vector3 direction)
    {
        if(Time.realtimeSinceStartup - m_tank.Config?.reloadTime < m_lastShootTime)
        {
            return;
        }

        AbstractProjectile projectile = m_projectilesPool.GetFromPool();
        projectile.transform.position = m_tank.ProjectileSpawnPosition;
        projectile.transform.forward = direction.normalized;
        projectile.gameObject.SetActive(true);
        projectile.Launch(direction);
        m_lastShootTime = Time.realtimeSinceStartup;
    }

    protected override void InitializeInternal()
    {
        base.InitializeInternal();
        m_projectilesPool = new ObjectPool<AbstractProjectile>(m_projectiile);
    }
}