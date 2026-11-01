using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingTextManager : MonoBehaviour
{
    public static FloatingTextManager Instance;

    [Header("Text settings")]
    public TMP_FontAsset font;
    public float defaultFontSize = 5f;

    private Camera mainCamera;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void ShowText(
        string message,
        Vector3 position,
        Color color,
        float riseSpeed = 1f,
        float fadeDelay = 1f,
        float fadeDuration = 1f,
        float fontSize = -1f
    ){
        GameObject textObject = new GameObject("FloatingText");
        textObject.transform.position = position;

        TextMeshPro tmp = textObject.AddComponent<TextMeshPro>();

        tmp.text = message;
        tmp.color = color;
        tmp.fontSize = fontSize > 0 ? fontSize : defaultFontSize;
        tmp.alignment = TextAlignmentOptions.Center;

        if (font != null) tmp.font = font;

        StartCoroutine(AnimateText(
            textObject,
            tmp,
            riseSpeed,
            fadeDelay,
            fadeDuration
        ));
    }

    private IEnumerator AnimateText(
        GameObject obj,
        TextMeshPro tmp,
        float riseSpeed,
        float fadeDelay,
        float fadeDuration
    ){
        float elapsed = 0f;
        Color originalColor = tmp.color;

        /* Move up */
        while(elapsed < fadeDelay)
        {
            if (obj == null) yield break;

            obj.transform.position += Vector3.up * riseSpeed * Time.deltaTime;

            Billboard(obj.transform);

            elapsed += Time.deltaTime;

            yield return null;
        }

        /* Fade out */
        elapsed = 0f;

        while(elapsed < fadeDuration)
        {
            if (obj == null) yield break;
            float alpha = 1f - (elapsed / fadeDuration);

            tmp.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            obj.transform.position += Vector3.up * riseSpeed * Time.deltaTime;

            Billboard(obj.transform);

            elapsed += Time.deltaTime;

            yield return null;
        }

        Destroy(obj);
    }

    private void Billboard(Transform textTransform)
    {
        if (mainCamera != null)
        {
            textTransform.rotation = mainCamera.transform.rotation;
        }
    }

}
