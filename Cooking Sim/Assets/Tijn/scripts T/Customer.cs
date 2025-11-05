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

    // ------------------------------------------------------------
    // SIGNAL TO SPAWNER WHEN THIS CUSTOMER IS DESTROYED
    // ------------------------------------------------------------
    public System.Action OnCustomerDestroy;

    private const float MoveSpeed = 1.38f;
    private string myOrder;
    private Transform myWaitingSpot;
    private bool orderCompleted = false;
    private bool hasRotatedAtWaitSpot = false;

    public Action OnOrderComplete;

    private Animator animator;
    private Counter counter;
    private float checkInterval = 1.5f;
    private bool checkingForTray = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        SetBestelt(false);
        SetWalking(false);
    }

    private void Start()
    {
        counter = FindObjectOfType<Counter>();
        StartCoroutine(CustomerRoutine());
    }

    private IEnumerator CustomerRoutine()
    {
        // === 1. GO TO ORDER SCREEN ===
        if (orderScreenPoint != null)
        {
            orderScreenPoint.isOccupied = true;
            Debug.Log($"[Customer] {name} occupies order screen.");
        }
        yield return FollowPath(pathToOrderScreen);

        if (orderScreenPoint != null)
        {
            yield return MoveTo(orderScreenPoint.transform.position);
            SetBestelt(true);
        }

        // === 2. WAIT FOR PLAYER ===
        float waitTime = UnityEngine.Random.Range(10f, 30f);
        yield return new WaitForSeconds(waitTime);

        // === 3. GENERATE ORDER ===
        myOrder = orderSystem.GenerateOrder(this);
        orderSystem.RegisterCustomerOrder(myOrder, this);

        // === 4. FREE ORDER SCREEN ===
        if (orderScreenPoint != null)
        {
            orderScreenPoint.isOccupied = false;
            Debug.Log($"[Customer] {name} frees order screen.");
        }
        SetBestelt(false);

        // === 5. GO TO WAITING SPOT (RESERVE ON ARRIVAL) ===
        myWaitingSpot = FindFreeWaitingSpotAndReserve();
        if (myWaitingSpot != null)
        {
            Debug.Log($"[Customer] {name} moving to waiting spot: {myWaitingSpot.name}");
            yield return MoveTo(myWaitingSpot.position);
        }
        else
        {
            Debug.LogWarning($"[Customer] {name} — NO FREE WAITING SPOT!");
        }

        // === 6. CHECK TRAY ===
        checkingForTray = true;
        StartCoroutine(CheckForTray());

        while (!orderCompleted) yield return null;

        // === 7. PICKUP VIA PATH ===
        yield return FollowPath(pathToPickup);
        if (pickupPoint != null)
        {
            yield return MoveTo(pickupPoint.position);
            Tray tray = FindTrayAtPickupPoint();
            if (tray != null)
            {
                tray.transform.SetParent(transform);
                tray.transform.localPosition = new Vector3(0, 1f, 0.5f);
                yield return new WaitForSeconds(0.5f);
            }
        }

        yield return new WaitForSeconds(1f);

        // === 8. EXIT ===
        yield return FollowPath(pathToExit);
        if (exitPoint != null) yield return MoveTo(exitPoint.position);

        // === 9. FREE WAITING SPOT + SIGNAL SPAWNER ===
        if (myWaitingSpot != null)
        {
            var ws = myWaitingSpot.GetComponent<WaitingSpot>();
            if (ws != null) ws.isOccupied = false;
            Debug.Log($"[Customer] {name} frees waiting spot: {myWaitingSpot.name}");
        }

        // ← SIGNAL SPAWNER TO SPAWN NEW CUSTOMER
        OnCustomerDestroy?.Invoke();

        Destroy(gameObject);
    }

    private Transform FindFreeWaitingSpotAndReserve()
    {
        if (waitingSpots == null) return null;
        foreach (Transform spot in waitingSpots)
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

    private Tray FindTrayAtPickupPoint()
    {
        if (counter == null || counter.traySpot == null) return null;
        foreach (Transform child in counter.traySpot)
        {
            Tray tray = child.GetComponent<Tray>();
            if (tray != null && tray.orderName == myOrder) return tray;
        }
        return null;
    }

    private IEnumerator CheckForTray()
    {
        while (checkingForTray && !orderCompleted)
        {
            if (FindTrayAtPickupPoint() != null)
            {
                checkingForTray = false;
                CompleteOrder();
                yield break;
            }
            yield return new WaitForSeconds(checkInterval);
        }
    }

    public void CompleteOrder() => orderCompleted = true;

    public void GoToWaitingSpot(Transform spot)
    {
        if (spot == null) return;
        myWaitingSpot = spot;
        StartCoroutine(MoveTo(spot.position));
    }

    private void SetWalking(bool walking)
    {
        if (animator != null) animator.SetBool("isWalking", walking);
    }

    private void SetBestelt(bool value)
    {
        if (animator != null) animator.SetBool("Bestelt", value);
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
            Vector3 dir = (targetPos - transform.position).normalized;
            if (dir != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);
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
        Quaternion start = transform.rotation;
        Quaternion end = start * Quaternion.Euler(0f, 180f, 0f);
        float t = 0f;
        float duration = 0.5f;
        while (t < duration)
        {
            transform.rotation = Quaternion.Slerp(start, end, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        transform.rotation = end;
    }

    private IEnumerator FollowPath(List<Transform> waypoints)
    {
        if (waypoints == null || waypoints.Count == 0) yield break;
        foreach (var p in waypoints)
            if (p != null) yield return MoveTo(p.position);
    }

    public void ForcePickupOrder()
    {
        if (orderCompleted) return;
        StopAllCoroutines();
        checkingForTray = false;
        StartCoroutine(ForcedPickupRoutine());
    }

    private IEnumerator ForcedPickupRoutine()
    {
        yield return FollowPath(pathToPickup);
        if (pickupPoint != null)
        {
            yield return MoveTo(pickupPoint.position);
            Tray tray = FindTrayAtPickupPoint();
            if (tray != null)
            {
                tray.transform.SetParent(transform);
                tray.transform.localPosition = new Vector3(0, 1f, 0.5f);
                yield return new WaitForSeconds(0.5f);
            }
        }

        yield return new WaitForSeconds(1f);
        yield return FollowPath(pathToExit);
        if (exitPoint != null) yield return MoveTo(exitPoint.position);

        if (myWaitingSpot != null)
        {
            var ws = myWaitingSpot.GetComponent<WaitingSpot>();
            if (ws != null) ws.isOccupied = false;
        }

        OnCustomerDestroy?.Invoke();
        Destroy(gameObject);
    }
}