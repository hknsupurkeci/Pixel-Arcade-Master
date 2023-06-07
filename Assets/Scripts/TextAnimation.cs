using UnityEngine;
using TMPro;

public class TextAnimation : MonoBehaviour
{
    public float startDelay = 1f;
    public float moveDuration = 2f;
    public float waitDuration = 1f;
    public float moveDistance = 100f;
    
    [SerializeField] private TextMeshProUGUI m_TextMeshProUGUI;
    public void WaveTextStart(string level)
    {
        StartCoroutine(AnimateText(level));
    }
    private System.Collections.IEnumerator AnimateText(string level)
    {
        m_TextMeshProUGUI.text = level;
        yield return new WaitForSeconds(startDelay);

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(0f, -moveDistance, 0f);

        float timer = 0f;

        while (timer < moveDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / moveDuration);
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        yield return new WaitForSeconds(waitDuration);

        // Reset position
        transform.position = startPos;
    }
}
