using System.Collections;
using UnityEngine;

public class CameraTargetShaker : MonoBehaviour
{
    public static CameraTargetShaker Instance;

    [Header("Defaults")]
    public float defaultStrength = 0.08f;
    public float defaultDuration = 0.12f;
    public float frequency = 35f;

    private Vector3 baseLocalPos;
    private Coroutine routine;

    void Awake()
    {
        Instance = this;
        baseLocalPos = transform.localPosition;
    }

    public void Shake(float strength = -1f, float duration = -1f)
    {
        if (strength < 0) strength = defaultStrength;
        if (duration < 0) duration = defaultDuration;

        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(ShakeRoutine(strength, duration));
    }

    IEnumerator ShakeRoutine(float strength, float duration)
    {
        baseLocalPos = transform.localPosition;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;

            float x = (Mathf.PerlinNoise(Time.time * frequency, 0f) - 0.5f) * 2f * strength;
            float y = (Mathf.PerlinNoise(0f, Time.time * frequency) - 0.5f) * 2f * strength;

            transform.localPosition = baseLocalPos + new Vector3(x, y, 0f);
            yield return null;
        }

        transform.localPosition = baseLocalPos;
        routine = null;
    }
}