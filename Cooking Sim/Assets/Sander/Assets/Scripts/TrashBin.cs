using UnityEngine;

public class TrashBin : MonoBehaviour
{
    public string promptMessage = "Press E to throw away";

    public void Interact(PlayerGrabber player)
    {
        player.ThrowHeld();
    }
}
