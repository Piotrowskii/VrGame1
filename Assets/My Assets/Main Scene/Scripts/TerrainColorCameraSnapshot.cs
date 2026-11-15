using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TerrainColorCameraSnapshot : MonoBehaviour
{
    public Terrain targetTerrain;
    public int extraFramesToWait = 16;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.enabled = false;
    }

    IEnumerator Start()
    {
        if (targetTerrain == null)
        {
            targetTerrain = Terrain.activeTerrain;
        }

        while (targetTerrain == null || targetTerrain.terrainData == null)
        {
            yield return null;
        }

        targetTerrain.Flush();

        for (int i = 0; i < extraFramesToWait; i++)
        {
            yield return new WaitForEndOfFrame();
        }

        cam.Render();

        gameObject.SetActive(false);
    }
}
