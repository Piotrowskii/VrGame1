using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 1f;

    // Update is called once per frame
    void Update()
    {
        transform.rotation *= Quaternion.Euler(0, rotateSpeed * Time.deltaTime, 0);
    }
}
