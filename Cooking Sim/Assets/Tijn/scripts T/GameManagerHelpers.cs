using UnityEngine;

internal static class GameManagerHelpers
{
    public static GameManager Instance
    {
        get
        {
            // If the instance hasn’t been set yet, try to find it in the scene
            if (GameManager.Instance == null)
            {
                var gm = Object.FindAnyObjectByType<GameManager>();
                if (gm != null)
                {
                    return gm;
                }

                Debug.LogError("⚠️ No GameManager found in the scene!");
                return null;
            }

            return GameManager.Instance;
        }
    }
}
