using System.Collections;
using UnityEngine;

public class DrawCardAnimation : MonoBehaviour
{
    /* Animation Metrics */
    public readonly Vector3 minScale = Vector3.one;
    public Vector3 maxScale = new Vector3(1.1f, 1.1f, 1.1f);

    public IEnumerator AssigningCardAnimation(Vector3 targetPos, Vector3 posAtDraw)
    {
        yield return new WaitForSeconds(0.1f);

        // Move to the left and enlarging
        yield return MovePosition(transform.position + posAtDraw, 
            (float)AnimationMetrics.MovingDuration, (float)AnimationMetrics.MovingSpeed);

        yield return ScaleDrawingCard(minScale, maxScale);

        // Stops moving to the left, start moving to the player's position
        yield return MovePosition(targetPos, 
            (float)AnimationMetrics.MovingDuration, (float)AnimationMetrics.MovingSpeed);
        yield return ScaleDrawingCard(maxScale, minScale);

        // Stays there for a sec
        yield return new WaitForSeconds((float)AnimationMetrics.SecsInEnd/2);
        Destroy(gameObject);
    }

    IEnumerator ScaleDrawingCard(Vector3 startScale, Vector3 endScale)
    {
        float t = 0.0f;
        float rate = (1f / (float)AnimationMetrics.ScalingDuration) * (float)AnimationMetrics.ScalingSpeed;

        while (t < 1f)
        {
            t += Time.deltaTime * rate;
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
    }

    IEnumerator MovePosition(Vector3 targetPos, float duration, float speed)
    {
        float t = 0.0f;
        float rate = (1f / duration) * speed;

        while (t < 1f)
        {
            t += Time.deltaTime * rate;
            transform.position = Vector3.Lerp(transform.position, targetPos, (float)AnimationMetrics.SpeedToMiddle * Time.deltaTime);
            yield return null;
        }
    }
}
