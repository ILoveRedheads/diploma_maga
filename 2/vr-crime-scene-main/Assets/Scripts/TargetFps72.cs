using UnityEngine;

/// <summary>
/// Limits frame rate to 72 FPS (Meta Quest 3S) for Editor profiling and play mode.
/// </summary>
public sealed class TargetFps72 : MonoBehaviour
{
    public const int TargetFps = 72;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void EnableBeforeFirstScene()
    {
        ApplyTargetFps();
    }

    void Awake()
    {
        ApplyTargetFps();
    }

    static void ApplyTargetFps()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = TargetFps;
    }
}
