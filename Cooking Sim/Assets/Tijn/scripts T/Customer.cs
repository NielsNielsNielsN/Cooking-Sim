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

    public System.Action OnCustomerDestroy;

    private const float MoveSpeed = 1.38f;
    private string myOrder;
    private Transform myWaitingSpot;
    private bool orderCompleted = false;
    private bool hasRotatedAtWaitSpot = false;

    private Animator animator;
    private Counter counter;
    private bool checkingForTray = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        SetBestelt(false);
        SetWalking(false);
        SetHeeftBestelling(false);
    }

    private void Start()
    {
        counter = FindObjectOfType<Counter>();
        if (counter == null) Debug.LogError("[Customer] Counter not found!");
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

        yield return new WaitForSeconds(UnityEngine.Random.Range(10f, 30f));

        myOrder = orderSystem.GenerateOrder(this);
        orderSystem.RegisterCustomerOrder(myOrder, this);
        Debug.Log($"[Customer] {name}: ORDER '{myOrder}'");

        if (orderScreenPoint != null) orderScreenPoint.isOccupied = false;
        SetBestelt(false);

        myWaitingSpot = FindFreeWaitingSpotAndReserve();
        if (myWaitingSpot != null)
            yield return MoveTo(myWaitingSpot.position);

        checkingForTray = true;
        StartCoroutine(CheckForTray());

        while (!orderCompleted) yield return null;

        yield return FollowPath(pathToPickup);
        if (pickupPoint != null)
        {
            yield return MoveTo(pickupPoint.position);

            // DESTROY TRAY FROM COUNTER
            bool trayDestroyed = DestroyTrayFromCounter();
            if (trayDestroyed)
            {
                Debug.Log($"[Customer] {name}: TRAY DESTROYED FOR ORDER '{myOrder}'!");
            }
            else
            {
                Debug.LogWarning($"[Customer] {name}: NO TRAY TO DESTROY FOR '{myOrder}'");
            }

            SetHeeftBestelling(true);
            SetWalking(true);
            yield return new WaitForSeconds(1.5f);
        }

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

    // ------------------------------------------------------------
    // DESTROY TRAY FROM traySpot (MATCHING ORDER)
    // ------------------------------------------------------------
    private bool DestroyTrayFromCounter()
    {
        if (counter == null || counter.traySpot == null || counter.traySpot.childCount == 0)
            return false;

        for (int i = 0; i < counter.traySpot.childCount; i++)
        {
            Transform child = counter.traySpot.GetChild(i);
            Tray tray = child.GetComponent<Tray>();
            if (tray != null && !string.IsNullOrEmpty(tray.orderName) &&
                string.Equals(tray.orderName, myOrder, System.StringComparison.OrdinalIgnoreCase))
            {
                Destroy(child.gameObject);
                Debug.Log($"[Customer] {name}: DESTROYED TRAY '{myOrder}' FROM traySpot!");
                return true;
            }
        }
        return false;
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

    private IEnumerator CheckForTray()
    {
        while (checkingForTray && !orderCompleted)
        {
            if (counter != null && counter.HasMatchingTray(myOrder))
            {
                checkingForTray = false;
                CompleteOrder();
                yield break;
            }
            yield return new WaitForSeconds(1f);
        }
    }

    public void CompleteOrder()
    {
        orderCompleted = true;
        Debug.Log($"[Customer] {name}: ORDER READY → GOING TO PICKUP!");
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
            DestroyTrayFromCounter();
            SetHeeftBestelling(true);
            SetWalking(true);
            yield return new WaitForSeconds(1.5f);
            counter.ClearTraySpot();

        }

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

    private void SetWalking(bool walking) => animator?.SetBool("isWalking", walking);
    private void SetBestelt(bool value) => animator?.SetBool("Bestelt", value);
    private void SetHeeftBestelling(bool value) => animator?.SetBool("HeeftBestelling", value);

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
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);
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
}