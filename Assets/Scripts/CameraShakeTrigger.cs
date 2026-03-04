using System.Collections;
using UnityEngine;

public class CameraShakeTrigger : MonoBehaviour
{
    public static CameraShakeTrigger Instance;

    [Header("Assign Main Camera Transform (optional)")]
    public Transform cameraTransform;

    [Header("Defaults")]
    public float defaultStrength = 0.08f;  // küçük değerler iyi
    public float defaultDuration = 0.12f;
    public float frequency = 35f;

    Vector3 originalLocalPos;
    Coroutine routine;

    void Awake()
    {
        Instance = this;

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (cameraTransform != null)
            originalLocalPos = cameraTransform.localPosition;
    }

    public void Shake(float strength = -1f, float duration = -1f)
    {
        if (cameraTransform == null) return;

        if (strength < 0) strength = defaultStrength;
        if (duration < 0) duration = defaultDuration;

        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(ShakeRoutine(strength, duration));
    }

    IEnumerator ShakeRoutine(float strength, float duration)
    {
        originalLocalPos = cameraTransform.localPosition;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;

            float x = (Mathf.PerlinNoise(Time.time * frequency, 0f) - 0.5f) * 2f * strength;
            float y = (Mathf.PerlinNoise(0f, Time.time * frequency) - 0.5f) * 2f * strength;

            cameraTransform.localPosition = originalLocalPos + new Vector3(x, y, 0f);
            yield return null;
        }

        cameraTransform.localPosition = originalLocalPos;
        routine = null;
    }
}