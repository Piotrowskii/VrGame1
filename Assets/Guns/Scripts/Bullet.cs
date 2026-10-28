using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Parameters")]
    public int damage = 1;

    [Header("Lifetime")]
    [SerializeField] private float maxDistanceFromOrigin = 100f;

    private bool hasHit;
    private bool isDestroying;

    private TrailRenderer trailRenderer;
    private Renderer bulletRenderer;
    private Collider bulletCollider;
    private Rigidbody bulletRigidbody;

    private void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        bulletRenderer = GetComponent<MeshRenderer>();
        bulletCollider = GetComponent<Collider>();
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (trailRenderer != null)
        {
            trailRenderer.Clear();
            trailRenderer.AddPosition(transform.position);
        }
    }

    private void FixedUpdate()
    {
        if(transform.position.sqrMagnitude > maxDistanceFromOrigin * maxDistanceFromOrigin)
        {
            DestroyBullet();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;

        Shootable shootable = collision.collider.GetComponentInParent<Shootable>();

        if(shootable != null)
        {
            hasHit = true;
            Vector3 hitPoint = collision.GetContact(0).point;
            shootable.RegisterHit(this, hitPoint);
            DestroyBullet();
        }
    }

    private void DestroyBullet()
    {
        if (isDestroying) return;
        isDestroying = true;

        if(bulletRenderer != null) bulletRenderer.enabled = false;
        if(bulletCollider != null) bulletRenderer.enabled = false;
        if(bulletRigidbody != null)
        {
            //bulletRigidbody.linearVelocity = Vector3.zero;
            //bulletRigidbody.isKinematic = true;
        }

        if (trailRenderer != null)
        {
            trailRenderer.emitting = false;
            trailRenderer.transform.SetParent(null, true);
            Destroy(gameObject, trailRenderer.time);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
