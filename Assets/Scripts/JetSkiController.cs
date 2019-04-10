using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using WaterBuoyancy;

public class JetSkiController : MonoBehaviour
{
    [Header("Mechanics")]
    public Transform elevatorPivot;
    public Transform handlePivot;
    public TrailRenderer trail;
    public int maxTurningAngle = 30;
    public int maxElevationAngle = 20;
    public float minEmissionVelocity = 20;
    public float elevatorDampening = 15;

    [Header("Physics")]
    public FloatingObject floatingObject;
    public Rigidbody jetSkiRB;
    public float forwardSpeed = 30;
    public float reverseSpeed = 20;
    public float turningSpeed = 15;
    public float submergeCheck = .1f;

    public float Velocity { private set; get; }
    public bool IsObjectInWater { get { return floatingObject.SubmergedVolume > submergeCheck; } }

    float handleRotation;

    void FixedUpdate()
    {
        Trail();
        PivotHandling();
        UpdateVelocity();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

        if (!IsObjectInWater) return;

#if UNITY_EDITOR
        CheckInputEditor();
#else
        CheckInputMobile();
#endif
    }

    void Trail()
    {
        trail.emitting = Vector3.Angle(Vector3.up, transform.up) < 30 && IsObjectInWater && Velocity > minEmissionVelocity;
        if (IsObjectInWater)
            trail.transform.localPosition = new Vector3(0, transform.up.y > 0 ? -.8f : .8f, .8f);
    }

    void UpdateVelocity()
    {
        Velocity = jetSkiRB.velocity.sqrMagnitude;
    }

    void CheckInputMobile()
    {
        switch (Input.touchCount)
        {
            case 1:
                Turn(Input.GetTouch(0).rawPosition.x < Screen.width / 2);
                break;

            case 2:
                if (Mathf.Abs(Input.GetTouch(0).rawPosition.x - Input.GetTouch(1).rawPosition.x) > Screen.width / 2)
                    ApplyBrake();
                break;

            default:
                MoveForward();
                handleRotation += (handleRotation > 0 ? -1 : 1);
                break;
        }
    }

    void CheckInputEditor()
    {
        if (IsObjectInWater)
        {
            float move = Input.GetAxisRaw("Vertical");
            if (move < 0) ApplyBrake();
            else MoveForward();

            float turn = Input.GetAxisRaw("Horizontal");
            if (turn != 0) Turn(turn < 0);
            else handleRotation += (handleRotation > 0 ? -1 : 1);
        }
    }

    void MoveForward()
    {
        jetSkiRB.AddForce(transform.forward * forwardSpeed);
    }

    void ApplyBrake()
    {
        jetSkiRB.AddForce(transform.forward * -reverseSpeed);
    }

    void PivotHandling()
    {
        elevatorPivot.localRotation = Quaternion.Euler(maxElevationAngle - Mathf.Clamp(Velocity / elevatorDampening, 0, maxElevationAngle), 0, 0);
        handlePivot.localRotation = Quaternion.Lerp(handlePivot.localRotation, Quaternion.Euler(0, handleRotation, 0), .25f);
        if (Vector3.Angle(Vector3.up, transform.up) > 100 && IsObjectInWater) RestartLevel();
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(0);
    }

    void Turn(bool turnLeft)
    {
        jetSkiRB.AddTorque(transform.up * (turnLeft ? -turningSpeed : turningSpeed));
        handleRotation = Mathf.Clamp(handleRotation += turnLeft ? -1 : 1, -maxTurningAngle, maxTurningAngle);
    }
}
