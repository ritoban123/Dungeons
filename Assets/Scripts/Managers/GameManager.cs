using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public event Action<float> OnUpdate;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void SetupTimer(float seconds, Action<float> updateAction, Action expireAction)
    {
        float timer = 0;
        Action expireWrapper = null;
        Action<float> updateWrapper = null;
        updateWrapper =
            (dt) =>
            {
                timer += dt;
                if (updateAction != null)
                    updateAction(dt);
                if (timer >= seconds)
                    expireWrapper();
            };

        OnUpdate += updateWrapper;

        expireWrapper =
            () =>
            {
                OnUpdate -= updateWrapper;
                if (expireAction != null)
                    expireAction();
            };
    }

    private void Update()
    {
        if (OnUpdate != null)
            OnUpdate(Time.deltaTime);
    }
}
