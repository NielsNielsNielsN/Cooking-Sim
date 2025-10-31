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

    private const float MoveSpeed = 1.38f;
    private string myOrder;
    private Transform myWaitingSpot;
    private bool orderCompleted = false;
    private bool hasRotatedAtWaitSpot = false;

    public Action OnOrderComplete;
    private Animator animator;

    // Counter & tray check
    private Counter counter;
    private float checkInterval = 1.5f;
    private bool checkingForTray = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        SetBestelt(false);
    }

    private void Start()
    {
        counter = FindObjectOfType<Counter>();
        StartCoroutine(CustomerRoutine());
    }

    private IEnumerator CustomerRoutine()
    {
        if (orderScreenPoint != null) orderScreenPoint.isOccupied = true;
        yield return FollowPath(pathToOrderScreen);

        if (orderScreenPoint != null)
        {
            yield return MoveTo(orderScreenPoint.transform.position);
            SetBestelt(true);
        }

        float waitTime = UnityEngine.Random.Range(10f, 30f);
        yield return new WaitForSeconds(waitTime);

        myOrder = orderSystem.GenerateOrder();
        orderSystem.RegisterCustomerOrder(myOrder, this);

        if (orderScreenPoint != null) orderScreenPoint.isOccupied = false;
        OnOrderComplete?.Invoke();
        SetBestelt(false);

        myWaitingSpot = GetFreeWaitingSpot();
        if (myWaitingSpot != null)
        {
            yield return MoveTo(myWaitingSpot.position);
        }

        // Start checking for tray
        checkingForTray = true;
        StartCoroutine(CheckForTray());

        // Wait until the customer gets the correct tray
        while (!orderCompleted) yield return null;

        // Then move to pickup point and exit
        yield return FollowPath(pathToPickup);
        if (pickupPoint != null) yield return MoveTo(pickupPoint.position);
        yield return new WaitForSeconds(2f);

        yield return FollowPath(pathToExit);
        if (exitPoint != null) yield return MoveTo(exitPoint.position);

        ReturnWaitingSpot();
        Destroy(gameObject);
    }

    private IEnumerator CheckForTray()
    {
        while (checkingForTray && !orderCompleted)
        {
            if (counter != null && counter.traySpot.childCount > 0)
            {
                foreach (Transform trayTransform in counter.traySpot)
                {
                    Tray tray = trayTransform.GetComponent<Tray>();
                    if (tray != null && IsTrayOrderCorrect(tray))
                    {
                        checkingForTray = false;
                        yield return MoveTo(counter.traySpot.position);

                        tray.transform.SetParent(transform);
                        tray.transform.localPosition = new Vector3(0, 1f, 0.5f);

                        yield return new WaitForSeconds(1f);
                        CompleteOrder();
                        yield break;
                    }
                }
            }
            yield return new WaitForSeconds(checkInterval);
        }
    }

    // ✅ Tray order check
    private bool IsTrayOrderCorrect(Tray tray)
    {
        return tray.orderName == myOrder;
    }

    public void CompleteOrder()
    {
        orderCompleted = true;
    }

    // ✅ Fixed GoToWaitingSpot (actually moves & rotates)
    public void GoToWaitingSpot(Transform spot)
    {
        if (spot == null) return;
        myWaitingSpot = spot;
        StartCoroutine(MoveTo(spot.position));
    }

    private Transform GetFreeWaitingSpot()
    {
        if (waitingSpots == null) return null;
        foreach (var spot in waitingSpots)
        {
            var ws = spot.GetComponent<WaitingSpot>();
            if (ws != null && !ws.isOccupied)
            {
                ws.isOccupied = true;
                return spot;
            }
        }
        return null;
    }

    private void ReturnWaitingSpot()
    {
        if (myWaitingSpot == null) return;
        var ws = myWaitingSpot.GetComponent<WaitingSpot>();
        if (ws != null) ws.isOccupied = false;
    }

    private void SetWalking(bool walking)
    {
        if (animator != null)
            animator.SetBool("isWalking", walking);
    }

    private void SetBestelt(bool value)
    {
        if (animator != null)
            animator.SetBool("Bestelt", value);
    }

    private IEnumerator MoveTo(Vector3 targetPos)
    {
        if (Vector3.Distance(transform.position, targetPos) <= 0.1f)
        {
            transform.position = targetPos;
            SetWalking(false);
            TryRotateAtWaitingSpot(targetPos);
            yield break;
        }

        SetWalking(true);

        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            Vector3 direction = (targetPos - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPos, MoveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        SetWalking(false);
        TryRotateAtWaitingSpot(targetPos);
    }

    private void TryRotateAtWaitingSpot(Vector3 pos)
    {
        if (hasRotatedAtWaitSpot) return;

        foreach (Transform spot in waitingSpots)
        {
            if (spot != null && Vector3.Distance(pos, spot.position) < 0.2f)
            {
                hasRotatedAtWaitSpot = true;
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
        if (waypoints == null || waypoints.Count == 0) yield break;

        foreach (var point in waypoints)
        {
            if (point != null)
                yield return MoveTo(point.position);
        }
    }
}
