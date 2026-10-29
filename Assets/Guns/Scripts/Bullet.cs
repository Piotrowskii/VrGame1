using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Parameters")]
    public int damage = 1;

    [Header("Lifetime")]
    [SerializeField] private float lifetime = 5f;

    [Header("Visuals")]
    [SerializeField] private GameObject bulletModel;
    [SerializeField] private TrailRenderer trailRenderer;

    private bool hasHit;
    private bool isDestroying;

    private Collider bulletCollider;
    private Rigidbody bulletRigidbody;

    private void Awake()
    {
        bulletCollider = GetComponent<Collider>();
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (trailRenderer != null)
        {
            trailRenderer.Clear();
            trailRenderer.emitting = true;
        }

        Invoke(nameof(DestroyBullet), lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit || isDestroying) return;

        hasHit = true;
        Shootable shootable = collision.collider.GetComponentInParent<Shootable>();

        if (shootable != null)
        {
            Vector3 hitPoint = collision.GetContact(0).point;
            shootable.RegisterHit(this, hitPoint);
        }

        DestroyBullet();
    }

    private void DestroyBullet()
    {
        if (isDestroying) return;
        isDestroying = true;

        CancelInvoke(nameof(DestroyBullet));

        if (bulletModel != null) bulletModel.SetActive(false);
        if(bulletCollider != null) bulletCollider.enabled = false;
        if(bulletRigidbody != null)
        {
            bulletRigidbody.linearVelocity = Vector3.zero;
            bulletRigidbody.isKinematic = true;
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
