using TMPro;
using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    public TMP_Text fpsText;
    public float updateInterval = 0.5f;
    private float elapsedTime;
    private int frames;
    private float fps;

    void Update()
    {
        frames++;
        elapsedTime += Time.unscaledDeltaTime;

        if (elapsedTime >= updateInterval)
        {
            fps = frames / elapsedTime;

            if (fpsText != null)
            {
                fpsText.text = $"FPS: {fps:0}";
            }

            frames = 0;
            elapsedTime = 0f;
        }
    }
}
