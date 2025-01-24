using System;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    /// <summary>
    /// Needed to determine spawn positions range.
    /// Can be used to scale the level prefab
    /// </summary>
    [SerializeField] private Vector2 m_levelSize;
    [SerializeField] private List<Transform> spawnPositions;

    public Vector3 GetSpawnPosition(Team team)
    {
        switch (team)
        {
            case Team.Player:
                return spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Count - 1)].position;
            case Team.Enemy:
                return GetRandomPosition();
            default:
                break;
        }

        Debug.LogError($"Can not determine spawn position for team {team}");
        return Vector2.zero;
    }

    private Vector3 GetRandomPosition()
    {
        float x = UnityEngine.Random.Range(-m_levelSize.x, m_levelSize.x);
        float z = UnityEngine.Random.Range(-m_levelSize.y, m_levelSize.y);
        return new Vector3(x, 0, z);
    }
}

[Serializable]
public struct SpawnPositions
{
    [SerializeField] private Transform spawnPoint;
    public Team team;
    public Vector3 position => spawnPoint.position;
}
