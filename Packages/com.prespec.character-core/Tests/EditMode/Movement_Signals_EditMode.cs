#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;
using Prespec.CharacterCore.Runtime.Movement;
using Prespec.CharacterCore.Runtime.Movement.Signals;
using Prespec.CharacterCore.Contracts.Movement;

public class Movement_Signals_EditMode
{
    [Test]
    public void Overlay_Starts_And_Ends_Fires_Signals()
    {
        var go = new GameObject("movement");
        var svc = go.AddComponent<MovementServiceComponent>();
        var profile = ScriptableObject.CreateInstance<LocomotionProfile>();
        typeof(MovementServiceComponent)
            .GetField("_profile", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(svc, profile);

        bool started = false, ended = false;
        svc.Signals.OverlayStarted += () => started = true;
        svc.Signals.OverlayEnded += () => ended = true;

        // Register a short overlay, tick once to start and complete.
        var tok = svc.RegisterDisplacementOverlay(new DisplacementOverlay(1f, 0f, 0.01f));
        svc.SetMoveIntent(new MoveIntent(0, 0));
        svc.SendMessage("FixedUpdate"); // simulate one tick
        Assert.IsTrue(started);

        // On second tick it should finish and fire ended.
        svc.SendMessage("FixedUpdate");
        Assert.IsTrue(ended);

        Object.DestroyImmediate(profile);
        Object.DestroyImmediate(go);
    }
}
#endif
