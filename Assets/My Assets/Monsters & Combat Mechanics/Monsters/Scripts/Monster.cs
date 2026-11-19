using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(DestructionEffect))]
public class Monster : MonoBehaviour, IShootable
{

    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth = 100;

    [Header("Model")]
    [SerializeField] private GameObject modelPrefab;
    [SerializeField] private Vector3 scaleMultiplier = Vector3.one;

    [Header("Damage text")]
    [SerializeField] private Vector3 damageTextOffset = new Vector3(0, 0.5f, 0);

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3.5f;

    [Header("Audio")]
    [SerializeField] private AudioClip deathSound;
    [SerializeField, Range(0f, 1f)] private float deathVolume = 1f;

    public UnityEvent OnDeath = new();

    private bool isDead = false;
    private NavMeshAgent agent;
    private Healthbar healthbar;
    private Transform navMeshTargetTransform;
    private DestructionEffect destructionEffect;
    private GameObject modelInstance;

    private void Awake()
    {
        destructionEffect = GetComponent<DestructionEffect>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;

        healthbar = GetComponentInChildren<Healthbar>();
        if (modelPrefab != null)
        {
            modelInstance = Instantiate(modelPrefab, transform);
            modelInstance.transform.localScale = Vector3.Scale(modelInstance.transform.localScale, scaleMultiplier);
        }
    }

    void Start()
    {
        navMeshTargetTransform = GameManager.instance.tower.transform;
        agent.SetDestination(navMeshTargetTransform.position);
    }

    public void OnShieldCircleEntered()
    {
        if (isDead) return;
        Die(transform.position);
    }

    public void OnShot(Projectile projectile, Vector3 shotPoint)
    {
        if (isDead) return;

        currentHealth -= projectile.damage;
        currentHealth = Mathf.Max(0, currentHealth);

        var damageTextPosition = healthbar.gameObject.transform.position + damageTextOffset;
        FloatingTextManager.Instance.ShowText($"-{projectile.damage}", damageTextPosition, Color.red);

        healthbar.UpdateHealthBar(maxHealth, currentHealth);

        if (currentHealth <= 0)
        {
            Die(shotPoint);
        }
    }

    private void Die(Vector3 shotPoint)
    {
        if (isDead) return;

        isDead = true;

        agent.isStopped = true;
        agent.enabled = false;

        if(modelInstance != null) modelInstance.SetActive(false);
        if(healthbar != null) healthbar.SetGameObjectActive(false);

        SoundManager.Instance.PlaySound(SoundType.PointSound);

        OnDeath.Invoke();

        destructionEffect.Spawn(
            transform.position,
            transform.rotation,
            shotPoint
        );

        Destroy(gameObject, destructionEffect.DestroyDelay);
    }
}
