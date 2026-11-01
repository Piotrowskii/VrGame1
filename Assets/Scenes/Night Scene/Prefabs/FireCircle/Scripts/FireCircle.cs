using UnityEngine;

public class FireCircle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Monster monster = other.GetComponentInParent<Monster>();

        if(monster != null)
        {
            monster.OnFireCircleEntered();
        }
    }
}
