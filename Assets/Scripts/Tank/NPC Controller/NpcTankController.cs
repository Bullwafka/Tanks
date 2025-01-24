using UnityEngine;

public sealed class NpcTankController : AbstractTankController
{
    private const string CONFIG_ID = "DefaultEnemyBehaviourConfig";

    private EnemyDefaultBehaviorConfig m_config;
    private AbstractStateMachine<AbstractState> m_stateMachine;
    public NpcTankController(Tank tank, AbstractTankBehaviour moveBehaviour, 
                                        AbstractShootBehaviour shootBehaviour) : 
                                        base(tank, moveBehaviour, shootBehaviour) { }

    protected override void InitializeInternal()
    {
        base.InitializeInternal();

        m_config = AllSystems.GetSystem<ConfigsSystem>().GetConfig<EnemyDefaultBehaviorConfig>(CONFIG_ID);

        IdleState idleState = new IdleState(m_config.idleStateDuration.minValue, m_config.idleStateDuration.maxValue);

        m_stateMachine = new AbstractStateMachine<AbstractState>(idleState);

        DefaultMoveForwardState forwardMoveState = new DefaultMoveForwardState(m_config.forwardMoveDuration.minValue, 
                                                                               m_config.forwardMoveDuration.maxValue,
                                                                               m_tank, m_moveBehaviour);

        RandomRotatedMoveState rotatedMoveState = new RandomRotatedMoveState(m_config.rotatedMoveDuration.minValue,
                                                                               m_config.rotatedMoveDuration.maxValue,
                                                                               m_tank, m_moveBehaviour);

        RotationState rotationState = new RotationState(m_config.rotationDuration.minValue, 
                                                                               m_config.rotationDuration.maxValue,
                                                                               m_moveBehaviour);

        BackMoveState backMoveState = new BackMoveState(m_config.backwardMoveDuration.minValue,
                                                        m_config.backwardMoveDuration.maxValue, m_moveBehaviour, m_tank);

        m_stateMachine.AddState(forwardMoveState, true);
        m_stateMachine.AddState(rotatedMoveState, true);
        m_stateMachine.AddState(rotationState);
        m_stateMachine.AddState(backMoveState);
        m_stateMachine.AddState(idleState);

        m_stateMachine.Start();

        m_tank.OnTankCollision += OnTankCollidedHanlder;
        m_tank.OnWallCollision += OnWallCollidedHanlder;
    }

    public override void Update()
    {
        base.Update();

        m_stateMachine.Update();

        if (m_stateMachine.CurrentState.Finished)
        {
            m_stateMachine.NextState();
        }
    }

    public override void Shutdown()
    {
        base.Shutdown();
        m_tank.OnTankCollision -= OnTankCollidedHanlder;
        m_tank.OnWallCollision -= OnWallCollidedHanlder;
    }

    private void OnWallCollidedHanlder()
    {
        m_moveBehaviour.Stop();
        m_stateMachine.SetCurrentState<IdleState>();
        m_stateMachine.SetNextState<BackMoveState>();
        m_stateMachine.SetNextState<RotationState>();
    }

    private void OnTankCollidedHanlder(Tank tank)
    {
        m_stateMachine.SetCurrentState<RotationState>();
    }
}

public abstract class TimeConstrainedState : AbstractState
{
    public override bool Finished => Time.realtimeSinceStartup - m_startTime >= m_currentDuration;
    public float MinDuration => m_minDuration;
    public float MaxDuration => m_maxDuration;

    protected readonly float m_minDuration;
    protected readonly float m_maxDuration;

    private float m_startTime;
    private float m_currentDuration;

    protected TimeConstrainedState(float minDuration, float maxDuration)
    {
        m_minDuration = minDuration;
        m_maxDuration = maxDuration;
    }

    public override void Start()
    {
        Debug.Log($"{this} started");
        m_startTime = Time.realtimeSinceStartup;
        m_currentDuration = Random.Range(m_minDuration, m_maxDuration);
    }
}

public sealed class IdleState : TimeConstrainedState
{
    public IdleState(float minDuration, float maxDuration) : base(minDuration, maxDuration) { }

    public override void Start()
    {
        base.Start();
    }

    public override void Stop() { }

    public override void Update() { }
}

public sealed class DefaultMoveForwardState : TimeConstrainedState
{
    private AbstractTankBehaviour m_moveBehaviour;
    private Tank m_tank;
    private Vector3 m_direction;

    public DefaultMoveForwardState(float minDuration, float maxDuration, Tank tank,
                            AbstractTankBehaviour moveBehaviour) : base(minDuration, maxDuration)
    {
        m_moveBehaviour = moveBehaviour;
        m_tank = tank;
    }

    public override void Start()
    {
        base.Start();
        m_direction = m_tank.transform.forward;
    }

    public override void Stop()
    {
        m_direction = Vector3.zero;
    }

    public override void Update()
    {
        m_moveBehaviour.Move(m_direction);
    }
}

public sealed class RandomRotatedMoveState : TimeConstrainedState
{
    private AbstractTankBehaviour m_moveBehaviour;
    private Tank m_tank;
    private int m_rotationValue;

    public RandomRotatedMoveState(float minDuration, float maxDuration,
                            Tank tank, AbstractTankBehaviour moveBehaviour) : base(minDuration, maxDuration)
    {
        m_moveBehaviour = moveBehaviour;
        m_tank = tank;
    }

    public override void Start()
    {
        base.Start();
        m_rotationValue = Random.Range(-1, 1) > -1 ? -1 : 1;
    }

    public override void Stop()
    {
        m_rotationValue = 0;
    }

    public override void Update()
    {
        m_moveBehaviour.Rotate(m_rotationValue);
        m_moveBehaviour.Move(m_tank.transform.forward);
    }
}

public sealed class RotationState : TimeConstrainedState
{
    private AbstractTankBehaviour m_moveBehaviour;
    public RotationState(float minDuration, float maxDuration, AbstractTankBehaviour moveBehaviour) : base(minDuration, maxDuration)
    {
        m_moveBehaviour = moveBehaviour;
    }

    private int m_rotationValue;

    public override void Start()
    {
        base.Start();
        m_moveBehaviour.Stop();
        m_rotationValue = Random.Range(-1, 1) > -1 ? -1 : 1;
    }

    public override void Stop()
    {
        m_rotationValue = 0;
    }

    public override void Update()
    {
        m_moveBehaviour.Stop();
        m_moveBehaviour.Rotate(m_rotationValue);
    }
}

public sealed class BackMoveState : TimeConstrainedState
{
    private AbstractTankBehaviour m_moveBehaviour;
    private Tank m_tank;
    private Vector3 m_direction;

    public BackMoveState(float minDuration, float maxDuration, AbstractTankBehaviour moveBehaviour, Tank tank) : base(minDuration, maxDuration)
    {
        m_moveBehaviour = moveBehaviour;
        m_tank = tank;
    }

    public override void Stop()
    {
        m_moveBehaviour.Stop();
    }

    public override void Update()
    {
        m_direction = -m_tank.transform.forward;
        m_moveBehaviour.Move(m_direction);
    }
}