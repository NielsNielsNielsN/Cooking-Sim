using UnityEngine;

public class CustomerAI : MonoBehaviour
{
    public float speed = 2f;
    private Vector3 target;

    void Update()
    {
        MoveToTarget();
    }

    private void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        transform.LookAt(target);
    }

    // --- Add these public methods so Queue can call them ---

    public void GoToWaitingSpot(Transform spot)
    {
        target = spot.position;
    }

    public void GoToCounter(Transform counter, Transform exit)
    {
        target = counter.position;
        // optional: store exit for later
        nextExit = exit;
    }

    public void GoToExit(Transform exit)
    {
        target = exit.position;
    }

    // optional: store exit internally if needed
    private Transform nextExit;
}
