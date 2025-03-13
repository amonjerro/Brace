using UnityEngine.SceneManagement;


public static class SceneTransitionManager
{
    public static void LoadScene(int sceneId)
    {
        ServiceLocator.Instance.RunServiceCleanup();
        // Add Fade Out


        SceneManager.LoadScene(sceneId);
    }
}