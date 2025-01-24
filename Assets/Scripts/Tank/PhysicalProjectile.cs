using UnityEngine;

public class PhysicalProjectile : AbstractProjectile
{
    public override bool Pooled => !gameObject.activeSelf;

    // Can be moved to configs
    [SerializeField] private float m_speed;
    [SerializeField] private Rigidbody m_rigidbody;


    public override void Launch(Vector3 direction)
    {
        m_rigidbody.velocity = direction.normalized * m_speed;
    }

    protected override void OnHit()
    {
        m_rigidbody.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Projectile colided with {collision.gameObject.name}");
        OnHit();
    }
}
