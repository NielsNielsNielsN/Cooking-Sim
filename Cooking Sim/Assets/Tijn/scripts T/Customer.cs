using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    [HideInInspector] public CustomerOrder orderSystem;
    [HideInInspector] public OrderScreenPoint orderScreenPoint;
    [HideInInspector] public Transform[] waitingSpots;
    [HideInInspector] public Transform pickupPoint;
    [HideInInspector] public Transform exitPoint;

    [HideInInspector] public List<Transform> pathToOrderScreen;
    [HideInInspector] public List<Transform> pathToPickup;
    [HideInInspector] public List<Transform> pathToExit;

    private float moveSpeed = 1.38f;
    private string myOrder;
    private Transform myWaitingSpot;
    private bool orderCompleted = false;
    private Animator animator;

    public Rigidbody rb;

    // Event dat de spawner kan gebruiken (voor SendToWaitingSpot)
    public Action OnOrderComplete;

    private void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(CustomerRoutine());
    }

    private IEnumerator CustomerRoutine()
    {
        // Loop naar bestelscherm
        PlayAnim("Loopje");
        if (orderScreenPoint != null) orderScreenPoint.isOccupied = true;
        yield return FollowPath(pathToOrderScreen);
        if (orderScreenPoint != null) yield return MoveTo(orderScreenPoint.transform.position);

        // Stop met lopen, bestel
        PlayAnim("Order");
        float waitTime = UnityEngine.Random.Range(10f, 30f);
        yield return new WaitForSeconds(waitTime);

        myOrder = orderSystem.GenerateOrder();
        orderSystem.RegisterCustomerOrder(myOrder, this);

        if (orderScreenPoint != null) orderScreenPoint.isOccupied = false;

        // Laat weten dat bestelling is geplaatst
        OnOrderComplete?.Invoke();

        // Naar wachtrij
        myWaitingSpot = GetFreeWaitingSpot();
        if (myWaitingSpot != null)
        {
            PlayAnim("Na order lopen");
            yield return MoveTo(myWaitingSpot.position);
        }

        // Wacht tot bestelling klaar is
        PlayAnim("Wachten");
        while (!orderCompleted) yield return null;

        // Naar afhaalpunt
        PlayAnim("lopen naar bestelling");
        yield return FollowPath(pathToPickup);
        if (pickupPoint != null) yield return MoveTo(pickupPoint.position);

        // Animatie voor oppakken
        PlayAnim("bestelling ophalen");
        yield return new WaitForSeconds(2f);

        // Naar uitgang
        PlayAnim("Bestelling lopen");
        yield return FollowPath(pathToExit);
        if (exitPoint != null) yield return MoveTo(exitPoint.position);

        PlayAnim("bestelling doorlopen");
        Destroy(gameObject);
    }

    public void CompleteOrder()
    {
        orderCompleted = true;
    }

    public void GoToWaitingSpot(Transform spot)
    {
        if (spot == null) return;
        myWaitingSpot = spot;
    }

    private Transform GetFreeWaitingSpot()
    {
        if (waitingSpots == null) return null;
        foreach (var spot in waitingSpots)
        {
            WaitingSpot ws = spot.GetComponent<WaitingSpot>();
            if (ws != null && !ws.isOccupied)
            {
                ws.isOccupied = true;
                return spot;
            }
        }
        return null;
    }

    private IEnumerator MoveTo(Vector3 targetPos)
    {
        animator.SetBool("isWalking", true);

        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            Vector3 direction = (targetPos - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            bool isWalking = Vector3.Distance(transform.position, targetPos) > 0.1f;
            animator.SetBool("isWalking", isWalking);

            yield return null;
        }

        animator.SetBool("isWalking", false);
        transform.position = targetPos;

        // 🔹 Turn 180 degrees if at a waiting spot
        foreach (Transform spot in waitingSpots)
        {
            if (Vector3.Distance(targetPos, spot.position) < 0.2f)
            {
                StartCoroutine(Rotate180Smooth());
                break;
            }
        }
    }

    private IEnumerator Rotate180Smooth()
    {
        Quaternion startRot = transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0f, 180f, 0f);
        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startRot, endRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRot;
    }

    private IEnumerator FollowPath(List<Transform> waypoints)
    {
        if (waypoints == null) yield break;
        foreach (var point in waypoints)
        {
            if (point == null) continue;
            yield return MoveTo(point.position);
        }
    }

    private void PlayAnim(string animName)
    {
        if (animator != null)
        {
            animator.Play(animName);
        }
    }
}
