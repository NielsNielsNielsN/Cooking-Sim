using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class QTEManager : MonoBehaviour
{
    public static QTEManager Instance { get; private set; }

    public GameObject qteUIPrefab;

    private QTEUI currentUI;

    private void Awake()
    {
        Instance = this;
    }

    public IEnumerator RunQTE(float window, Action<bool> callback)
    {
        KeyCode[] keys = { KeyCode.Space, KeyCode.F };
        KeyCode chosen = keys[UnityEngine.Random.Range(0, keys.Length)];

        if (qteUIPrefab)
        {
            GameObject uiObj = Instantiate(qteUIPrefab, transform);
            currentUI = uiObj.GetComponent<QTEUI>();
            if (currentUI) currentUI.Setup(chosen.ToString(), window);
        }

        float timer = window;
        bool success = false;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;

            if (currentUI) currentUI.UpdateTimer(timer);

            if (Input.GetKeyDown(chosen))
            {
                success = true;
                break;
            }

            yield return null;
        }

        if (currentUI)
        {
            currentUI.Hide();
            Destroy(currentUI.gameObject, 0.1f);
            currentUI = null;
        }

        callback?.Invoke(success);
    }
}
