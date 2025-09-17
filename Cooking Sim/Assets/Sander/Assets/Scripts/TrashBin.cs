using UnityEngine;

public class TrashBin : MonoBehaviour
{
    [Header("Optional UI Prompt")]
    public string promptMessage = "Press E to throw away";

    // This method will be called by PlayerGrabber when the player presses E while looking at the bin
    public void Interact(PlayerGrabber player)
    {
        if (player == null) return;

        // Destroy whatever the player is holding
        player.ThrowHeld();
    }
}
