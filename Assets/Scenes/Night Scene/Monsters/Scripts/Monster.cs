using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(NavMeshAgent))]
public class Monster : MonoBehaviour, IShootable
{

    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth = 100;

    [Header("Model")]
    [SerializeField] private GameObject modelPrefab;

    [Header("Damage text")]
    [SerializeField] private Vector3 damageTextOffset = new Vector3(0, 0.5f, 0);

    private Animator animator;
    private NavMeshAgent agent;
    private Healthbar healthbar;
    private Transform navMeshTargetTransform;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        healthbar = GetComponentInChildren<Healthbar>();
        if (modelPrefab != null)
        {
            Instantiate(modelPrefab, transform);
            animator = GetComponentInChildren<Animator>();
        }
    }

    void Start()
    {
        navMeshTargetTransform = GameManager.instance.campfire.transform;
        agent.SetDestination(navMeshTargetTransform.position);
    }

    public void OnFireCircleEntered()
    {
        Debug.Log("Monster entered");
    }

    public void OnShot(Projectile projectile, Vector3 shotPoint)
    {
        var damageTextPosition = healthbar.gameObject.transform.position + damageTextOffset;
        FloatingTextManager.Instance.ShowText($"-{projectile.damage}", damageTextPosition, Color.red);

        if (currentHealth - projectile.damage > 0)
        {
            currentHealth -= projectile.damage;
            healthbar.UpdateHealthBar(maxHealth, currentHealth);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
