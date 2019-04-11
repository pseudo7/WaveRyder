using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuntController : MonoBehaviour
{
    public static bool doingStunt;

    public float senstivity = 4;
    public float torque = 6;
    public float tunnelTorque = 2;

    bool gestureRecieved;
    Touch touch1, touch2;

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.A))
            PerformStunt(StuntType.BACK_FLIP);
        if (Input.GetKeyDown(KeyCode.S))
            PerformStunt(StuntType.FRONT_FLIP);
        if (Input.GetKeyDown(KeyCode.D))
            PerformStunt(StuntType.RIGHT_360);
        if (Input.GetKeyDown(KeyCode.F))
            PerformStunt(StuntType.LEFT_360);
        if (Input.GetKeyDown(KeyCode.Q))
            PerformStunt(StuntType.TUNNEL_LEFT);
        if (Input.GetKeyDown(KeyCode.W))
            PerformStunt(StuntType.TUNNEL_RIGHT);
#else
        if (Input.touchCount == 2)
        {
            touch1 = Input.GetTouch(0);
            touch2 = Input.GetTouch(1);
            if (touch1.phase == TouchPhase.Moved && touch2.phase == TouchPhase.Moved)
                SetStuntType(touch1.deltaPosition, touch2.deltaPosition);
            else if (touch1.phase == TouchPhase.Ended && touch2.phase == TouchPhase.Ended)
                gestureRecieved = false;
        }
#endif
    }

    void SetStuntType(Vector2 delta1, Vector2 delta2)
    {
        if (gestureRecieved) return;
        gestureRecieved = true;

        if (delta1.sqrMagnitude > senstivity && delta2.sqrMagnitude > senstivity)
        {
            if (touch1.rawPosition.x < Screen.width / 2)
            {
                if (delta1.y < 0 && delta2.y > 0) PerformStunt(StuntType.TUNNEL_LEFT);
                else if (delta1.y > 0 && delta2.y < 0) PerformStunt(StuntType.TUNNEL_RIGHT);
            }
            else
            {
                if (delta1.y < 0 && delta2.y > 0) PerformStunt(StuntType.TUNNEL_RIGHT);
                else if (delta1.y > 0 && delta2.y < 0) PerformStunt(StuntType.TUNNEL_LEFT);
            }
            if (Mathf.Abs(delta1.x) < Mathf.Abs(delta1.y))
            {
                if (delta1.y < 0 && delta2.y < 0) PerformStunt(StuntType.BACK_FLIP);
                else if (delta1.y > 0 && delta2.y > 0) PerformStunt(StuntType.FRONT_FLIP);
            }
            else if (Mathf.Abs(delta1.x) > Mathf.Abs(delta1.y))
            {
                if (delta1.x > 0 && delta2.x > 0) PerformStunt(StuntType.RIGHT_360);
                else if (delta1.x < 0 && delta2.x < 0) PerformStunt(StuntType.LEFT_360);
            }
        }
    }

    void PerformStunt(StuntType type)
    {
        if (doingStunt) return;
        doingStunt = true;
        Rigidbody playerRB = GetComponent<Rigidbody>();
        switch (type)
        {
            case StuntType.BACK_FLIP:
                playerRB.AddTorque(transform.right * -torque, ForceMode.Impulse);
                break;
            case StuntType.FRONT_FLIP:
                playerRB.AddTorque(transform.right * torque, ForceMode.Impulse);
                break;
            case StuntType.RIGHT_360:
                playerRB.AddTorque(transform.up * -torque, ForceMode.Impulse);
                break;
            case StuntType.LEFT_360:
                playerRB.AddTorque(transform.up * torque, ForceMode.Impulse);
                break;
            case StuntType.TUNNEL_RIGHT:
                playerRB.AddTorque(transform.forward * -tunnelTorque, ForceMode.Impulse);
                break;
            case StuntType.TUNNEL_LEFT:
                playerRB.AddTorque(transform.forward * tunnelTorque, ForceMode.Impulse);
                break;
        }
    }
}
public enum StuntType
{
    BACK_FLIP, FRONT_FLIP, RIGHT_360, LEFT_360, TUNNEL_RIGHT, TUNNEL_LEFT, NONE
}