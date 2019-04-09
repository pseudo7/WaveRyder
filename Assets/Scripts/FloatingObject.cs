using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    Rigidbody rb;
    bool enteredWater;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter\nCenter: " + other.bounds.center + " " + other.bounds.extents);
        enteredWater = true;
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exit\nCenter: " + other.bounds.center + " " + other.bounds.extents);
    }

    void FixedUpdate()
    {
        rb.AddForce(enteredWater ? Vector3.up * 10 : Vector3.zero);
    }
}
