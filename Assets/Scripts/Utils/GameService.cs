using UnityEngine;

/// <summary>
/// Abstract game service class. This will populate the service locator.
/// </summary>
public abstract class AbsGameService : MonoBehaviour
{
    /// <summary>
    /// Some states require unsubscribing from observer patterns on clean up. The service locator runs 
    /// a clean up on all of them on destroy.
    /// </summary>
    public abstract void CleanUp();
}