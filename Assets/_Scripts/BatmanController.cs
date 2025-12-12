using UnityEngine;
using UnityEngine.InputSystem;

public enum BatmanState { Normal, Stealth, Alert }

public class BatmanController : MonoBehaviour
{
    [Header("State Settings")]
    public BatmanState currentState = BatmanState.Normal;

    [Header("Models (Drag and Drop here!)")]
    [SerializeField] private GameObject batmanModel; 
    [SerializeField] private GameObject carModel;    

    [Header("Movement Settings")]
    [SerializeField] private float normalSpeed = 5f;
    [SerializeField] private float stealthSpeed = 2f;
    [SerializeField] private float carSpeed = 15f;   
    [SerializeField] private float turnSpeed = 120f;

    [Header("Gadgets")]
    [SerializeField] private GameObject batSignalObject;
    [SerializeField] private Light redLight;
    [SerializeField] private Light blueLight;
    [SerializeField] private AudioSource sirenAudio; 
    [SerializeField] private Light mainLight;
    

    private Animator animator;
    private float currentSpeed;
    private float alertTimer;
    private bool isInCar = false; 

    void Start()
    {
        if(batmanModel != null)
            animator = batmanModel.GetComponent<Animator>();
        
        UpdateStateEffects();
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        HandleInput();
        HandleMovement();
        HandleStateLogic();
    }

    private void HandleInput()
    {
        // 1. Bat-Signal (B)
        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            if (batSignalObject != null)
                batSignalObject.SetActive(!batSignalObject.activeSelf);
        }

        // 2. State Switching (N, C, Space)
        if (Keyboard.current.nKey.wasPressedThisFrame) ChangeState(BatmanState.Normal);
        if (Keyboard.current.cKey.wasPressedThisFrame) ChangeState(BatmanState.Stealth);
        if (Keyboard.current.spaceKey.wasPressedThisFrame) ChangeState(BatmanState.Alert);

        // 3. CAR TOGGLE (V)
        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            ToggleCar();
        }
    }

    private void ToggleCar()
    {
        isInCar = !isInCar; // Flip the switch

        // Swap Models
        if (batmanModel) batmanModel.SetActive(!isInCar);
        if (carModel) carModel.SetActive(isInCar);

        // If in car, force Normal state (optional)
        if (isInCar) ChangeState(BatmanState.Normal);
    }

    private void ChangeState(BatmanState newState)
    {
        currentState = newState;
        UpdateStateEffects();
    }

    private void UpdateStateEffects()
    {
        // Reset everything first
        if (redLight) redLight.enabled = false;
        if (blueLight) blueLight.enabled = false;
        if (sirenAudio) sirenAudio.Stop();

        // Apply Logic
        switch (currentState)
        {
            case BatmanState.Alert:
                if (sirenAudio) sirenAudio.Play();
                break;
        }

        // Set main light intensity depending on Normal state
        if (mainLight != null)
        {
            mainLight.intensity = currentState == BatmanState.Normal ? 0.7f : 0.2f;
        }
    }

    private void HandleStateLogic()
    {
        // Bat-Signal Rotation
        if (batSignalObject != null && batSignalObject.activeSelf)
            batSignalObject.transform.Rotate(Vector3.up * 10f * Time.deltaTime);

        // Flashing Lights (Only in Alert Mode)
        if (currentState == BatmanState.Alert)
        {
            alertTimer += Time.deltaTime;
            if (alertTimer > 0.2f)
            {
                if (redLight && blueLight)
                {
                    bool isRedOn = redLight.enabled;
                    redLight.enabled = !isRedOn;
                    blueLight.enabled = isRedOn;
                }
                alertTimer = 0f;
            }
        }
    }

    private void HandleMovement()
    {
        float moveInput = 0f;
        if (Keyboard.current.wKey.isPressed) moveInput = 1f;
        else if (Keyboard.current.sKey.isPressed) moveInput = -1f;

        float turnInput = 0f;
        if (Keyboard.current.dKey.isPressed) turnInput = 1f;
        else if (Keyboard.current.aKey.isPressed) turnInput = -1f;

        // ANIMATION (Only if Batman is visible)
        if (!isInCar && animator != null)
        {
            bool isMoving = moveInput != 0f;
            animator.SetBool("IsMoving", isMoving);
        }

        // SPEED CALCULATION
        float baseSpeed = normalSpeed;

        if (isInCar)
        {
            baseSpeed = carSpeed; // SUPER FAST
        }
        else if (currentState == BatmanState.Stealth)
        {
            baseSpeed = stealthSpeed;
        }

        // Boost Logic (Shift)
        if (Keyboard.current.leftShiftKey.isPressed && currentState != BatmanState.Stealth)
            currentSpeed = baseSpeed * 2f;
        else
            currentSpeed = baseSpeed;

        transform.Translate(Vector3.forward * moveInput * currentSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * turnInput * turnSpeed * Time.deltaTime);
    }
}