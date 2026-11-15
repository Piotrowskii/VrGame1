using UnityEngine;

public class SpawnCircle : MonoBehaviour
{
    [Header("Spawn Area")]
    [SerializeField] private float innerRadius = 5f;

    public Vector3 GetRandomSpawnPoint()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float distance = Mathf.Sqrt(Random.value) * innerRadius;

        Vector3 offset = new Vector3(
            Mathf.Cos(angle),
            0f,
            Mathf.Sin(angle)
        ) * distance;

        return transform.position + offset;
    }
}
