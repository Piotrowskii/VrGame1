using UnityEngine;

public enum ProjectileType
{
    Bullet,
    Arrow
}

public class Projectile : MonoBehaviour
{
    [Header("Parameters")]
    public int damage = 20;

    [Header("Lifetime")]
    [SerializeField] private float lifetime = 5f;

    [Header("Visuals")]
    [SerializeField] private GameObject projectileModel;
    [SerializeField] private TrailRenderer trailRenderer;

    [Header("Type")]
    [SerializeField] private ProjectileType type = ProjectileType.Bullet;

    private bool hasHit;
    private bool isDestroying;
    public bool isEnabled;

    private Collider projectileCollider;
    private Rigidbody projectileRigidbody;

    protected virtual void Awake()
    {
        projectileCollider = GetComponent<Collider>();
        projectileRigidbody = GetComponent<Rigidbody>();
        if (trailRenderer != null)
        {
            trailRenderer.Clear();
            trailRenderer.emitting = false;
            trailRenderer.enabled = false;
        }
    }

    public void EnableProjectile()
    {
        isEnabled = true;

        if (trailRenderer != null)
        {
            trailRenderer.emitting = true;
            trailRenderer.enabled = true;
        }
        Invoke(nameof(DestroyProjectile), lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit || isDestroying || !isEnabled) return;

        hasHit = true;
        IShootable shootable = collision.collider.GetComponentInParent<IShootable>();

        if (shootable != null)
        {
            Vector3 hitPoint = collision.GetContact(0).point;
            shootable.OnShot(this, hitPoint);
            ParticleManager.Instance.PlayEffect(ParticleType.ProjectileImpact, hitPoint, new Vector3(0.25f,0.25f,0.25f));
        }

        DestroyProjectile();
    }

    private void DestroyProjectile()
    {
        if (isDestroying) return;
        isDestroying = true;

        CancelInvoke(nameof(DestroyProjectile));

        if(projectileModel != null) projectileModel.SetActive(false);
        if(projectileCollider != null) projectileCollider.enabled = false;
        if(projectileRigidbody != null)
        {
            projectileRigidbody.linearVelocity = Vector3.zero;
            projectileRigidbody.isKinematic = true;
        }
        if (trailRenderer != null)
        {
            Destroy(gameObject, trailRenderer.time);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
