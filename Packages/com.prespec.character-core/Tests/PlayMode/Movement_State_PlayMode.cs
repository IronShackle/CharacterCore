using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using Prespec.CharacterCore.Runtime.Movement;
using Prespec.CharacterCore.Contracts.Movement;

public class Movement_State_PlayMode
{
    [UnityTest]
    public IEnumerator State_Updates_Desired_And_Overlay_Flags()
    {
        var go = new GameObject("movement");
        var svc = go.AddComponent<MovementServiceComponent>();
        var profile = ScriptableObject.CreateInstance<LocomotionProfile>();
        profile.baseSpeed = 5f;
        typeof(MovementServiceComponent)
            .GetField("_profile", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(svc, profile);

        // Move right for a frame
        svc.SetMoveIntent(new MoveIntent(1f, 0f));
        yield return new WaitForFixedUpdate();
        var s1 = svc.State;
        Assert.GreaterOrEqual(s1.CurrentSpeed, 0f);
        Assert.IsFalse(s1.OverlayActive);

        // Trigger an overlay; flag should go true during its lifetime.
        svc.RegisterDisplacementOverlay(new DisplacementOverlay(1f, 0f, 0.05f));
        yield return new WaitForFixedUpdate();
        var s2 = svc.State;
        Assert.IsTrue(s2.OverlayActive);

        // Let it finish
        yield return new WaitForSeconds(0.06f);
        var s3 = svc.State;
        Assert.IsFalse(s3.OverlayActive);

        Object.Destroy(profile);
        Object.Destroy(go);
    }
}
