using UnityEngine;

public class ShieldCircle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Monster monster = other.GetComponentInParent<Monster>();

        if (monster != null)
        {
            monster.OnShieldCircleEntered();
        }
    }
}
