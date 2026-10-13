
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class AreaWaveEffect : MonoBehaviour
{
    [SerializeField] private float duration = 0.3f;
    [SerializeField] private float lineWidth = 0.07f;

    private LineRenderer line;
    private float timer;
    private float targetRadius;
    private bool isPlaying;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();

        line.useWorldSpace = false;
        line.loop = true;
        line.positionCount = 65;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.sortingOrder = 10;

        for (int i = 0; i < line.positionCount; i++)
        {
            float angle = i * Mathf.PI * 2f / 64f;

            line.SetPosition(i, new Vector3(
                Mathf.Cos(angle),
                Mathf.Sin(angle),
                0f
            ));
        }
    }

    public void Play(float radius)
    {
        targetRadius = radius;
        timer = 0f;
        isPlaying = true;

        transform.localScale = Vector3.zero;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!isPlaying)
            return;

        timer += Time.deltaTime;

        float progress = Mathf.Clamp01(timer / duration);

        // La ola se expande.
        float easedProgress = 1f - Mathf.Pow(1f - progress, 3f);
        float currentRadius = Mathf.Lerp(
            0f, targetRadius, easedProgress
        );

        transform.localScale = Vector3.one * currentRadius;

        // El anillo se desvanece mientras se expande.
        float alpha = 1f - progress;
        Color waveColor = new Color(0.3f, 0.85f, 1f, alpha);

        line.startColor = waveColor;
        line.endColor = waveColor;

        if (progress >= 1f)
        {
            isPlaying = false;
            Destroy(gameObject);
        }
    }
}
