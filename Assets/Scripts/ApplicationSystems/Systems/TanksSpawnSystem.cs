using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class TanksSpawnSystem : MonoBehaviour, IApplicationSystem
{
    private const string SAVE_DATA_FILE_NAME = "data";

    [SerializeField] private string m_configId;
    [SerializeField] private Tank m_playerPrefab;
    [SerializeField] private Tank m_enemyPrefab;
    [SerializeField] private AbstractProjectile m_projectile;
    [SerializeField] private Level m_level;

    private SavingSystem m_savingSystem;
    private SaveData m_saveData;

    private List<Tank> m_allTanks;
    private List<Tank> m_enemies;
    private GameConfig m_config;

    public Tank Player { get; private set; }

    public void Initialize()
    {
        m_config = AllSystems.GetSystem<ConfigsSystem>().GetConfig<GameConfig>(m_configId);
        m_savingSystem = AllSystems.GetSystem<SavingSystem>();

        if(m_savingSystem.TryLoad(SAVE_DATA_FILE_NAME, out SaveData data))
        {
            m_saveData = data;
        }

        m_allTanks = new(m_config.enemiesCount + 1);
        m_enemies = new(m_config.enemiesCount);

        InitializePlayer();
        InitializeEnemies();
    }

    private void InitializePlayer()
    {
        TanksSaveData saveData = GetInitialTransformData(Team.Player, 0);
        Player = Instantiate(m_playerPrefab, saveData.position, saveData.rotation);

        DefaultTankMoveBehaviour defaultTankMoveBehaviour = new DefaultTankMoveBehaviour(Player);
        DefaultShootBehaviour shootBehaviour = new DefaultShootBehaviour(Player, m_projectile);
        PlayerTankController playerTankController = new PlayerTankController(Player, defaultTankMoveBehaviour, shootBehaviour);

        Player.Initialize(playerTankController, defaultTankMoveBehaviour, Team.Player);
        Player.OnDestroyed += OnPlayerDestroyedHandler;
        m_allTanks.Add(Player);

        Player.SetIsActiveState(saveData.activeState);
        if(!Player.IsActive)
        {
            DelayedSpawn(Player);
        }
    }

    private void InitializeEnemies()
    {
        for (int i = 0; i < m_config.enemiesCount; i++)
        {
            TanksSaveData saveData = GetInitialTransformData(Team.Enemy, i + 1);
            Tank tank = Instantiate(m_enemyPrefab, saveData.position, saveData.rotation);
            DefaultTankMoveBehaviour moveBehaviour = new DefaultTankMoveBehaviour(tank);
            DefaultShootBehaviour shootBehaviour = new DefaultShootBehaviour(tank, m_projectile);
            NpcTankController npcTankController = new NpcTankController(tank, moveBehaviour, shootBehaviour);
            tank.Initialize(npcTankController, moveBehaviour, Team.Enemy);

            tank.OnDestroyed += OnNpcDestroyedHandler;

            m_enemies.Add(tank);
            m_allTanks.Add(tank);

            tank.SetIsActiveState(saveData.activeState);
        }
    }

    public void Shutdown()
    {
        m_saveData = PrepareSaveData();
        m_savingSystem.Save(m_saveData, SAVE_DATA_FILE_NAME);

        Player.OnDestroyed -= OnPlayerDestroyedHandler;
        Destroy(Player.gameObject);
        foreach (Tank tank in m_enemies.ToArray())
        {
            tank.OnDestroyed -= OnNpcDestroyedHandler;
            Destroy(tank.gameObject);
        }
    }

    private SaveData PrepareSaveData()
    {
        SaveData data = new SaveData();
        data.tanksSaveData = new TanksSaveData[m_allTanks.Count];

        for (int i = 0; i < m_allTanks.Count; i++)
        {
            Tank tank = m_allTanks[i];
            Transform tankTransform = tank.transform;
            data.tanksSaveData[i].position = tankTransform.position;
            data.tanksSaveData[i].rotation = tankTransform.rotation;
            data.tanksSaveData[i].activeState = tank.IsActive;
        }

        return data;
    }

    private void Spawn(Tank tank)
    {
        tank.transform.position = GetSpawnPosition(tank.Team);
        tank.SetIsActiveState(true);
    }

    private void DelayedSpawn(Tank tank)
    {
        StartCoroutine(RespawnCoroutine(tank));
    }

    private IEnumerator RespawnCoroutine(Tank tank)
    {
        yield return new WaitForSeconds(m_config.respawnTime);
        Spawn(tank);
    }

    private void RespawnAllEnemies()
    {
        foreach(Tank tank in m_enemies) 
        {
            Spawn(tank);
        }
    }

    private Vector3 GetSpawnPosition(Team team)
    {
        return m_level.GetSpawnPosition(team);
    }


    private TanksSaveData GetInitialTransformData(Team team, int index)
    {
        if(m_saveData == null || index >= m_saveData.tanksSaveData.Length)
        {
            return new TanksSaveData()
            {
                position = GetSpawnPosition(team),
                rotation = Quaternion.identity,
                activeState = true
            };
        }

        return m_saveData.tanksSaveData[index];
    }

    private void OnPlayerDestroyedHandler(Tank destroyed)
    {
        DelayedSpawn(destroyed);
    }

    private void OnNpcDestroyedHandler(Tank destroyedTank)
    {
        foreach (Tank tank in m_enemies)
        {
            if(tank.IsActive)
            {
                return;
            }
        }

        RespawnAllEnemies();
    }

}
