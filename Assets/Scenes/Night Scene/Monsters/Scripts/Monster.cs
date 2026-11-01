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

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        healthbar = GetComponentInChildren<Healthbar>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navMeshTargetTransform = GameManager.instance.campfire.transform;
        agent.SetDestination(navMeshTargetTransform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnFireCircleEntered()
    {
        Debug.Log("Monster entered");
    }

    public void OnShot(Bullet bullet, Vector3 shotPoint)
    {
        if(currentHealth - bullet.damage > 0)
        {
            currentHealth -= bullet.damage;
            healthbar.UpdateHealthBar(maxHealth, currentHealth);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
