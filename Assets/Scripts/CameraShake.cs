using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    public float defaultDuration = 0.15f;
    public float defaultMagnitude = 0.08f;

    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;

    void Awake()
    {
        Instance = this;
        originalPosition = transform.localPosition;
    }

    void OnEnable()
    {
        originalPosition = transform.localPosition;
    }

    public void TriggerShake()
    {
        StartShake(defaultDuration, defaultMagnitude);
    }

    public void TriggerShake(float duration, float magnitude)
    {
        StartShake(duration, magnitude);
    }

    public void ShakeCamera()
    {
        StartShake(defaultDuration, defaultMagnitude);
    }

    public void ShakeCamera(float duration, float magnitude)
    {
        StartShake(duration, magnitude);
    }

    public void Shake(float duration, float magnitude)
    {
        StartShake(duration, magnitude);
    }

    public void StartShake(float duration, float magnitude)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }

        shakeCoroutine = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPosition + new Vector3(x, y, 0f);

            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }

        StopShake();
    }

    public void StopShake()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            shakeCoroutine = null;
        }

        transform.localPosition = originalPosition;
    }
}