using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] [Required] private Image healthbarSprite;
    [SerializeField] private Color healthbarColor = Color.red;
    [SerializeField] private float reduceSpeed = 2;

    private float target = 1;
    private Camera mainCamera;

    public void Awake()
    {
        healthbarSprite.color = healthbarColor;
    }

    public void Start()
    {
        mainCamera = Camera.main;
    }

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        if (maxHealth == 0) return;
        target = currentHealth / maxHealth;
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
        healthbarSprite.fillAmount = Mathf.MoveTowards(healthbarSprite.fillAmount, target, reduceSpeed * Time.deltaTime);
    }

    public void SetGameObjectActive(bool status)
    {
        gameObject.SetActive(status);
    }
}
