using Unity.VisualScripting;
using UnityEngine;

public class ArrowDetection : MonoBehaviour
{
    [SerializeField] private Bow bow;

    private void OnTriggerEnter(Collider other)
    {
        bow.TryLoadArrow(other);
    }
}
