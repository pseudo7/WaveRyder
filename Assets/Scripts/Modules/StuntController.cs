using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuntController : MonoBehaviour
{
    public static bool doingStunt;

    public const string RAMP_TAG = "Ramp";

    public float torque = 6;
    public float tunnelTorque = 2;

    //bool gestureRecieved;
    Rigidbody playerRB;

    void Start()
    {
        playerRB = GetComponent<Rigidbody>();
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.A))
            PerformStunt(StuntType.BACK_FLIP);
        if (Input.GetKey(KeyCode.S))
            PerformStunt(StuntType.FRONT_FLIP);
        if (Input.GetKey(KeyCode.D))
            PerformStunt(StuntType.RIGHT_360);
        if (Input.GetKey(KeyCode.F))
            PerformStunt(StuntType.LEFT_360);
        if (Input.GetKey(KeyCode.Q))
            PerformStunt(StuntType.TUNNEL_LEFT);
        if (Input.GetKey(KeyCode.W))
            PerformStunt(StuntType.TUNNEL_RIGHT);
#else
        if (Input.touchCount == 2)
            SetStuntType(Input.GetTouch(0), Input.GetTouch(1));
#endif
    }

    void SetStuntType(Touch touch1, Touch touch2)
    {
        if (!doingStunt) return;

        float x1 = touch1.position.x;
        float y1 = touch1.position.y;
        float x2 = touch2.position.x;
        float y2 = touch2.position.y;

        //Debug.Log(x1 + " " + y1 + " " + x2 + " " + y2);

        int xMedian = Screen.width / 2;
        int yMedian = Screen.height / 2;

        if (Mathf.Abs(x1) - Mathf.Abs(x2) > xMedian)
        {
            if (y1 < yMedian && y2 < yMedian)
                PerformStunt(StuntType.BACK_FLIP);
            else if (y1 > yMedian && y2 > yMedian)
                PerformStunt(StuntType.FRONT_FLIP);
            else if (y1 > yMedian && y2 < yMedian)
                PerformStunt(StuntType.TUNNEL_LEFT);
            else if (y1 < yMedian && y2 > yMedian)
                PerformStunt(StuntType.TUNNEL_RIGHT);
        }
        else
        {
            if (x1 < xMedian && x2 < xMedian)
                PerformStunt(StuntType.LEFT_360);
            else if (x1 > xMedian && x2 > xMedian)
                PerformStunt(StuntType.RIGHT_360);
        }

        //if (touch1.rawPosition.x < Screen.width / 2)
        //{
        //    if (delta1.y < 0 && delta2.y > 0) PerformStunt(StuntType.TUNNEL_LEFT);
        //    else if (delta1.y > 0 && delta2.y < 0) PerformStunt(StuntType.TUNNEL_RIGHT);
        //}
        //else
        //{
        //    if (delta1.y < 0 && delta2.y > 0) PerformStunt(StuntType.TUNNEL_RIGHT);
        //    else if (delta1.y > 0 && delta2.y < 0) PerformStunt(StuntType.TUNNEL_LEFT);
        //}
        //if (Mathf.Abs(delta1.x) < Mathf.Abs(delta1.y))
        //{
        //    if (delta1.y < 0 && delta2.y < 0) PerformStunt(StuntType.BACK_FLIP);
        //    else if (delta1.y > 0 && delta2.y > 0) PerformStunt(StuntType.FRONT_FLIP);
        //}
        //else if (Mathf.Abs(delta1.x) > Mathf.Abs(delta1.y))
        //{
        //    if (delta1.x > 0 && delta2.x > 0) PerformStunt(StuntType.RIGHT_360);
        //    else if (delta1.x < 0 && delta2.x < 0) PerformStunt(StuntType.LEFT_360);
        //}
    }

    //void ReEnableStunt()
    //{
    //    doingStunt = false;
    //}

    void PerformStunt(StuntType type)
    {
        Debug.Log(type);
        //if (doingStunt) return;
        //doingStunt = true;
        switch (type)
        {
            case StuntType.BACK_FLIP:
                playerRB.AddTorque(transform.right * -torque, ForceMode.Acceleration);
                break;
            case StuntType.FRONT_FLIP:
                playerRB.AddTorque(transform.right * torque, ForceMode.Acceleration);
                break;
            case StuntType.RIGHT_360:
                playerRB.AddTorque(transform.up * torque, ForceMode.Acceleration);
                break;
            case StuntType.LEFT_360:
                playerRB.AddTorque(transform.up * -torque, ForceMode.Acceleration);
                break;
            case StuntType.TUNNEL_RIGHT:
                playerRB.AddTorque(transform.forward * -tunnelTorque, ForceMode.Acceleration);
                break;
            case StuntType.TUNNEL_LEFT:
                playerRB.AddTorque(transform.forward * tunnelTorque, ForceMode.Acceleration);
                break;
        }
    }
}
public enum StuntType
{
    BACK_FLIP, FRONT_FLIP, RIGHT_360, LEFT_360, TUNNEL_RIGHT, TUNNEL_LEFT, NONE
}