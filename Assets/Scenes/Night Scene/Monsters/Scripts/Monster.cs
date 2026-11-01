using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(NavMeshAgent))]
public class Monster : MonoBehaviour, IShootable
{
    private int maxHealth = 100;
    private int currentHealth = 100;

    private NavMeshAgent agent;
    private Healthbar healthbar;
    private Transform navMeshTargetTransform;

    [SerializeField] private Vector3 damageTextOffset = new Vector3(0, 0.5f, 0);

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        healthbar = GetComponentInChildren<Healthbar>();
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

    public void OnShot(Bullet bullet, Vector3 shotPoint)
    {
        var damageTextPosition = healthbar.gameObject.transform.position + damageTextOffset;
        FloatingTextManager.Instance.ShowText($"-{bullet.damage}", damageTextPosition, Color.red);

        if (currentHealth - bullet.damage > 0)
        {
            currentHealth -= bullet.damage;
            healthbar.UpdateHealthBar(maxHealth, currentHealth);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
