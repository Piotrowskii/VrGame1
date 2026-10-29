using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class FloatingText : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private Color textOutlineColor = Color.black;
    [SerializeField] private float textOutlineWidth = 0.1f;
    [SerializeField, Min(1)] private int fontSize = 64;
    [SerializeField] private float heightOffset = 2f;


    [Header("Animation durations")]
    [SerializeField, Min(0f)] private float showDuration = 0.25f;
    [SerializeField, Min(0f)] private float hideDuration = 0.3f;
    [SerializeField, Min(0f)] private float stayDuration = 1.5f;
    [SerializeField, Min(0f)] private float popScale = 1.2f;

    [Header("Show Animation Curves")]
    [SerializeField]
    private AnimationCurve showScaleCurve = new AnimationCurve(
        new Keyframe(0f, 0f),
        new Keyframe(0.7f, 1.2f),
        new Keyframe(1f, 1f)
    );

    [SerializeField]
    private AnimationCurve showAlphaCurve = AnimationCurve.EaseInOut( 0f, 0f, 1f, 1f);

    [Header("Hide Animation Curves")]
    [SerializeField]
    private AnimationCurve hideScaleCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    [SerializeField]
    private AnimationCurve hideAlphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    [Header("Audio")]
    [SerializeField] private AudioSource appearSound;
    [SerializeField, Range(0f, 1f)] private float soundVolume = 1f;

    private Camera targetCamera;


    private TextMeshPro textMesh;
    private Material textInstancedMaterial;
    private Transform textTransform;
    private Coroutine animationRoutine;
    private GameObject textObject;

    private readonly Vector3 baseScale = Vector3.one;
    private Vector3 initialLocalPosition;
    private bool initialized;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (initialized && textTransform != null) return;
        if (this == null || transform == null) return;
        initialized = true;

        targetCamera = Camera.main;

        textObject = new GameObject("Floating text");

        textMesh = textObject.AddComponent<TextMeshPro>();
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.fontSize = fontSize;
        textMesh.richText = true;

        textTransform = textObject.transform;
        textTransform.SetParent(transform, false);
        initialLocalPosition = Vector3.up * heightOffset;
        textTransform.localPosition = initialLocalPosition;
        textTransform.localRotation = Quaternion.identity;
        textTransform.localScale = Vector3.zero;

        textMesh.textWrappingMode = TextWrappingModes.NoWrap;

        textInstancedMaterial = textMesh.fontMaterial;
        textInstancedMaterial.SetColor(ShaderUtilities.ID_OutlineColor, textOutlineColor);
        textInstancedMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, textOutlineWidth);
        textInstancedMaterial.EnableKeyword("OUTLINE_ON");

        MeshRenderer textRenderer = textObject.GetComponent<MeshRenderer>();
        textRenderer.sortingOrder = 10;

        textObject.SetActive(false);
    }

    public void ShowText(string message)
    {
        if (this == null || !isActiveAndEnabled) return;
        Initialize();

        if (animationRoutine != null)
        {
            StopCoroutine(animationRoutine);
            animationRoutine = null;
        }

        textMesh.text = message;
        textMesh.color = textColor;

        initialLocalPosition = Vector3.up * heightOffset;
        textTransform.localPosition = initialLocalPosition;
        textTransform.localScale = Vector3.zero;

        SetAlpha(0f);
        textObject.SetActive(true);

        PlayAppearSound();

        animationRoutine = StartCoroutine(PlayAnimation());
    }

    private IEnumerator PlayAnimation()
    {
        /*Show animation*/
        if(showDuration <= 0f)
        {
            textTransform.localScale = baseScale;
            SetAlpha(1f);
        }
        else
        {
            float elapsed = 0f;

            while(elapsed < showDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed/showDuration);

                float scale = showScaleCurve.Evaluate(t) * popScale;
                float alpha = Mathf.Clamp01(showAlphaCurve.Evaluate(t));

                textTransform.localScale = baseScale * scale;
                SetAlpha (alpha);

                yield return null;
            }

            textTransform.localScale = baseScale;
            SetAlpha(1f);
        }

        /*Wait*/
        if(stayDuration > 0f) yield return new WaitForSeconds(stayDuration);

        /*Hide animation*/
        if (hideDuration <= 0f)
        {
            textTransform.localScale = Vector3.zero;
            SetAlpha(0f);
        }
        else
        {
            float elapsed = 0f;

            while (elapsed < hideDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / hideDuration);

                float scale = Mathf.Max(0f, hideScaleCurve.Evaluate(t));
                float alpha = Mathf.Clamp01(hideAlphaCurve.Evaluate(t));

                textTransform.localScale = baseScale * scale;
                SetAlpha(alpha);

                yield return null;
            }
        }

        textTransform.localScale = Vector3.zero;
        SetAlpha(0f);
        textObject.SetActive(false);

        animationRoutine = null;
    }

    private void LateUpdate()
    {
        if (textTransform == null || !textTransform.gameObject.activeSelf) return;
        if (targetCamera == null) targetCamera = Camera.main;

        /* Billboard */
        if(targetCamera != null)
        {
            Vector3 direction = textTransform.position - targetCamera.transform.position;

            if(direction.sqrMagnitude > 0.001f)
            {
                textTransform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }

        }

        textTransform.localPosition = initialLocalPosition;
    }

    private void PlayAppearSound()
    {
        if (appearSound != null && appearSound.clip != null)
        {
            appearSound.PlayOneShot(appearSound.clip, soundVolume);
        }
    }

    private void SetAlpha(float alpha)
    {
        if (textMesh == null || textInstancedMaterial == null) return;

        Color faceColor = textColor;
        faceColor.a = alpha;

        Color outlineColor = textOutlineColor;
        outlineColor.a = alpha;

        textInstancedMaterial.SetColor(ShaderUtilities.ID_FaceColor, faceColor);
        textInstancedMaterial.SetColor(ShaderUtilities.ID_OutlineColor, outlineColor);
    }

    private void OnDestroy()
    {
        if (textInstancedMaterial != null) Destroy(textInstancedMaterial);
    }
}
