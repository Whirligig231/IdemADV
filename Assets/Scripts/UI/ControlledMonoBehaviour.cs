using System.Collections.Generic;
using UnityEngine;

public abstract class ControlledMonoBehaviour : MonoBehaviour
{
    public float priorityDepth; // Higher = lower-priority
    private static ControlledMonoBehaviour currentBoss;
    private static float currentMinPriorityDepth;

    private void LateUpdate()
    {
        if (currentBoss == null || !currentBoss.enabled)
            currentBoss = this;
        if (currentBoss == this)
        {
            currentMinPriorityDepth = float.PositiveInfinity;
            foreach (ControlledMonoBehaviour control in FindObjectsByType<ControlledMonoBehaviour>(FindObjectsSortMode.None))
            {
                if (control.enabled && control.priorityDepth < currentMinPriorityDepth)
                    currentMinPriorityDepth = control.priorityDepth;
            }
        }
    }

    protected bool HasPriority()
    {
        return (priorityDepth <= currentMinPriorityDepth);
    }
}
