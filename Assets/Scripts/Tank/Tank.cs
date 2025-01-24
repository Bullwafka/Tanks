using System;
using UnityEngine;

public enum Team
{
    Player, Enemy
}

public class Tank : MonoBehaviour
{
    public Team Team { get; private set; }
    public bool IsActive { get; private set; }

    [SerializeField] private string m_configId;
    [SerializeField] private Rigidbody m_rigidboy;
    [SerializeField] private Transform m_projectileSpawnPoint;

    private TankConfig m_config;
    public TankConfig Config => m_config;

    private AbstractTankController m_controller;
    private AbstractTankBehaviour m_moveBehaviour;
    private AbstractShootBehaviour m_shootBehaviour;

    public Vector3 ProjectileSpawnPosition => m_projectileSpawnPoint.position;

    public event Action OnWallCollision;
    public event Action<Tank> OnTankCollision;
    public event Action<Tank> OnDestroyed;

    public void Initialize(AbstractTankController controller,
                           AbstractTankBehaviour behaviour, Team team)
    {
        m_controller = controller;
        m_moveBehaviour = behaviour;
        Team = team;

        m_config = AllSystems.GetSystem<ConfigsSystem>().GetConfig<TankConfig>(m_configId);
        m_moveBehaviour.ApplySettings(m_config);

        // Should be moved into separate component
        if (Team == Team.Player)
        {
            OnTankCollision += OnTankCollisionHandler;
        }
    }

    private void FixedUpdate()
    {
        m_controller?.Update();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // TODO: move to project constants 
        if (collision.gameObject.CompareTag("Wall"))
        {
            OnWallCollision?.Invoke();
        }

        if (collision.gameObject.TryGetComponent(out Tank tank))
        {
            OnTankCollision?.Invoke(tank);
        }

        if (collision.gameObject.TryGetComponent(out AbstractProjectile projectile))
        {
            if (projectile.Team != Team)
            {
                SetIsActiveState(false);
                OnDestroyed?.Invoke(this);
            }
        }
    }

    private void OnTankCollisionHandler(Tank tank)
    {
        if (tank.Team != Team)
        {
            SetIsActiveState(false);
            OnDestroyed?.Invoke(this);
        }
    }

    private void OnDestroy()
    {
        OnTankCollision -= OnTankCollisionHandler;
        m_controller.Shutdown();
    }

    public void SetIsActiveState(bool state)
    {
        IsActive = state;
        gameObject.SetActive(state);
    }
}
