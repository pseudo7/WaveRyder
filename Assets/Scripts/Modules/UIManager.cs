using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { private set; get; }

    public Slider boostBar;

    void Awake()
    {
        if (!Instance) Instance = this;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    public void UpdateBoost(float val)
    {
        boostBar.value = val / 5;
    }
}
