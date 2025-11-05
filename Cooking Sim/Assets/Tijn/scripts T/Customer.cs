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
    private bool hasRotatedAtWaitSpot = false;   // ← prevents double-rotate

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
        if (orderScreenPoint != null) orderScreenPoint.isOccupied = true;
        yield return FollowPath(pathToOrderScreen);

        if (orderScreenPoint != null)
        {
            yield return MoveTo(orderScreenPoint.transform.position);
            SetBestelt(true);
        }

        float waitTime = UnityEngine.Random.Range(10f, 30f);
        yield return new WaitForSeconds(waitTime);

        myOrder = orderSystem.GenerateOrder(this);
        orderSystem.RegisterCustomerOrder(myOrder, this);

        if (orderScreenPoint != null) orderScreenPoint.isOccupied = false;
        OnOrderComplete?.Invoke();
        SetBestelt(false);

        myWaitingSpot = GetFreeWaitingSpot();
        if (myWaitingSpot != null)
        {
            // <-- THIS CALL NOW STOPS WALKING + ROTATES 180°
            yield return MoveTo(myWaitingSpot.position);
        }

        checkingForTray = true;
        StartCoroutine(CheckForTray());

        while (!orderCompleted) yield return null;

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
                foreach (Transform t in counter.traySpot)
                {
                    Tray tray = t.GetComponent<Tray>();
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

    private bool IsTrayOrderCorrect(Tray tray) => tray.orderName == myOrder;

    public void CompleteOrder() => orderCompleted = true;

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
        if (animator != null) animator.SetBool("isWalking", walking);
    }

    private void SetBestelt(bool value)
    {
        if (animator != null) animator.SetBool("Bestelt", value);
    }

    // ------------------------------------------------------------
    // MOVE → STOP → ROTATE 180° (ALL IN ONE COROUTINE)
    // ------------------------------------------------------------
    private IEnumerator MoveTo(Vector3 targetPos)
    {
        // Snap if already there
        if (Vector3.Distance(transform.position, targetPos) <= 0.1f)
        {
            transform.position = targetPos;
            SetWalking(false);
            yield break;
        }

        SetWalking(true);

        // ----- MOVE LOOP -----
        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            Vector3 dir = (targetPos - transform.position).normalized;
            if (dir != Vector3.zero)
            {
                Quaternion look = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 10f);
            }
            transform.position = Vector3.MoveTowards(transform.position, targetPos, MoveSpeed * Time.deltaTime);
            yield return null;
        }

        // ----- ARRIVED -----
        transform.position = targetPos;
        SetWalking(false);               // ← **FORCED STOP**

        // ----- ROTATE 180° ONLY AT WAITING SPOTS -----
        if (!hasRotatedAtWaitSpot)
        {
            foreach (Transform spot in waitingSpots)
            {
                if (spot != null && Vector3.Distance(targetPos, spot.position) < 0.2f)
                {
                    hasRotatedAtWaitSpot = true;
                    yield return StartCoroutine(Rotate180Smooth());
                    break;
                }
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

    // ------------------------------------------------------------
    // BUTTON CALL
    // ------------------------------------------------------------
    public void ForcePickupOrder()
    {
        if (orderCompleted) return;
        StopAllCoroutines();
        checkingForTray = false;
        StartCoroutine(ForcedPickupRoutine());
    }

    private IEnumerator ForcedPickupRoutine()
    {
        if (counter != null)
        {
            yield return MoveTo(counter.traySpot.position);

            foreach (Transform t in counter.traySpot)
            {
                Tray tray = t.GetComponent<Tray>();
                if (tray != null && IsTrayOrderCorrect(tray))
                {
                    tray.transform.SetParent(transform);
                    tray.transform.localPosition = new Vector3(0, 1f, 0.5f);
                    yield return new WaitForSeconds(0.5f);
                    CompleteOrder();
                    break;
                }
            }
        }

        yield return FollowPath(pathToPickup);
        if (pickupPoint != null) yield return MoveTo(pickupPoint.position);
        yield return new WaitForSeconds(1f);

        yield return FollowPath(pathToExit);
        if (exitPoint != null) yield return MoveTo(exitPoint.position);

        ReturnWaitingSpot();
        Destroy(gameObject);
    }
}