using UnityEngine;

public abstract class AbstractProjectile : MonoBehaviour, IPoolable
{
    public abstract bool Pooled { get; }
    public Team Team {get; set;}
    public abstract void Launch(Vector3 direction);
    protected abstract void OnHit();
}
