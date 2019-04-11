using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using WaterBuoyancy;

public class JetSkiController : MonoBehaviour
{
    [Header("Styles")]
    public float boostDrag = 2f;
    public float defaultDrag = 5f;
    public float boostTimeout = 5f;

    [Header("Mechanics")]
    public Transform elevatorPivot;
    public Transform handlePivot;
    public TrailRenderer mainTrail;
    public TrailRenderer boostTrail;
    public int maxTurningAngle = 30;
    public int maxElevationAngle = 20;
    public float minEmissionVelocity = 20;
    public float elevatorDampening = 15;
    public float handleRotationRate = 2.5f;

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
    float boostCountdown;
    bool applyBoost;

    void Start()
    {
        boostCountdown = boostTimeout;
    }

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
        boostTrail.emitting = applyBoost && Vector3.Angle(Vector3.up, transform.up) < 30 && IsObjectInWater && Velocity > minEmissionVelocity;
        if (IsObjectInWater)
            boostTrail.transform.localPosition = new Vector3(0, transform.up.y > 0 ? -.8f : .8f, .8f);

        mainTrail.emitting = Vector3.Angle(Vector3.up, transform.up) < 30 && IsObjectInWater && Velocity > minEmissionVelocity;
        if (IsObjectInWater)
            mainTrail.transform.localPosition = new Vector3(0, transform.up.y > 0 ? -.8f : .8f, .8f);
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
                if (Input.GetTouch(0).rawPosition.x > Screen.width / 2) ApplyBrake();
                break;

            default:
                MoveForward();
                break;
        }

        handleRotation += (handleRotation > 0 ? -handleRotationRate : handleRotationRate);
        ReduceBoost();
        Turn(Input.acceleration.x);
    }

    void CheckInputEditor()
    {
        if (IsObjectInWater)
        {
            float move = Input.GetAxisRaw("Vertical");
            if (move < 0) ApplyBrake();
            else if (move > 0) ApplyBoost();
            else MoveForward();

            float turn = Input.GetAxisRaw("Horizontal");
            if (turn != 0) Turn(turn < 0);
            else handleRotation += (handleRotation > 0 ? -handleRotationRate : handleRotationRate);

            ReduceBoost();
        }
    }

    void ReduceBoost()
    {
        if (applyBoost)
            if (boostCountdown <= 0)
            {
                floatingObject.dragInWater = defaultDrag;
                boostCountdown = boostTimeout;
                applyBoost = false;
            }
            else boostCountdown -= Time.deltaTime;
        UIManager.Instance.UpdateBoost(boostCountdown);
    }

    void MoveForward()
    {
        jetSkiRB.AddForce(transform.forward * forwardSpeed);
    }

    void ApplyBrake()
    {
        jetSkiRB.AddForce(transform.forward * -reverseSpeed);
    }

    public void ApplyBoost()
    {
        if (applyBoost) return;
        applyBoost = true;
        boostCountdown = boostTimeout;
        floatingObject.dragInWater = boostDrag;
    }

    void PivotHandling()
    {
        elevatorPivot.localRotation = Quaternion.Euler(maxElevationAngle - Mathf.Clamp(Velocity / elevatorDampening, 0, maxElevationAngle), 0, 0);
        handlePivot.localRotation = Quaternion.Lerp(handlePivot.localRotation, Quaternion.Euler(0, handleRotation, 0), .25f);
        if (Vector3.Angle(Vector3.up, transform.up) > 100 && IsObjectInWater) RestartLevel();
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(0);
    }

    void Turn(bool turnLeft)
    {
        jetSkiRB.AddTorque(transform.up * (turnLeft ? -turningSpeed : turningSpeed));
        handleRotation = Mathf.Clamp(handleRotation += turnLeft ? -handleRotationRate : handleRotationRate, -maxTurningAngle, maxTurningAngle);
    }

    void Turn(float turnValue)
    {
        jetSkiRB.AddTorque(transform.up * turnValue * turningSpeed);
        handleRotation = Mathf.Clamp(handleRotation += turnValue < 0 ? -handleRotationRate : handleRotationRate, -maxTurningAngle, maxTurningAngle);
    }
}
