using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineAnimate))]
public class TrainAnimationController : MonoBehaviour
{
    private SplineAnimate splineAnimate;

    private void Start()
    {
        Debug.Log("[Train] Initializing animation controller");

        splineAnimate = GetComponent<SplineAnimate>();
        if (splineAnimate == null)
        {
            Debug.LogError("[Train] Missing SplineAnimate component!");
            return;
        }

        splineAnimate.PlayOnAwake = false;
        splineAnimate.Loop = SplineAnimate.LoopMode.Once;

        Debug.Log($"[Train] Spline Container: {splineAnimate.Container != null}");
        Debug.Log($"[Train] Duration: {splineAnimate.Duration}");
    }

    public void PlayAnimation()
    {
        Debug.Log("[Train] PlayAnimation called");

        if (splineAnimate == null)
        {
            Debug.LogError("[Train] SplineAnimate reference null!");
            return;
        }

        if (splineAnimate.Container == null)
        {
            Debug.LogError("[Train] No spline container assigned!");
            return;
        }

        Debug.Log("[Train] Restarting animation");
        splineAnimate.Restart(false);
        splineAnimate.Play();

        Debug.Log($"[Train] Animation state - Playing: {splineAnimate.IsPlaying}, " +
                 $"Elapsed: {splineAnimate.ElapsedTime}/{splineAnimate.Duration}");
    }
}