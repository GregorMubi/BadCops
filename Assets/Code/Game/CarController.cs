using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField]
    private float Speed = 10f;
    [SerializeField]
    private MovementType movementType = MovementType.AxisLocked;

    private Quaternion targetRotation;
    public float smooth = 1f;

    private float currentSpeed = 0.0f;
    private float acceleration = 1.0f;
    private float turnSpeed = 1.0f;

    private enum MovementType {
        AxisLocked,
        Continuous,
    }

    private enum CarDirection
    {
        Up,
        Down,
        Left,
        Right,
    }

    public void ToggleCarMovementType() {
        if (movementType == MovementType.AxisLocked)
        {
            movementType = MovementType.Continuous;
        }
        else {
            movementType = MovementType.AxisLocked;
        }
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            OnButtonPressed(KeyCode.LeftArrow);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            OnButtonPressed(KeyCode.RightArrow);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            OnButtonPressed(KeyCode.UpArrow);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            OnButtonPressed(KeyCode.DownArrow);
        }
    }

    private void OnButtonPressed(KeyCode keyCode)
    {
        if (movementType == MovementType.AxisLocked)
        {
            float angle = 0;
            switch (keyCode)
            {
                case KeyCode.LeftArrow:
                    angle = 90;
                    break;
                case KeyCode.RightArrow:
                    angle = -90;
                    break;
                case KeyCode.UpArrow:
                    angle = 180;
                    break;
                case KeyCode.DownArrow:
                    angle = 0;
                    break;
            }
            targetRotation = Quaternion.AngleAxis(angle, Vector3.up);

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10 * smooth * Time.deltaTime);
            transform.localPosition -= transform.forward * Speed / 10;
        }
        else if (movementType == MovementType.Continuous)
        {
            switch (keyCode)
            {
                case KeyCode.LeftArrow:
                    transform.Rotate(Vector3.up, 1 * turnSpeed);
                    break;
                case KeyCode.RightArrow:
                    transform.Rotate(Vector3.up, -1 * turnSpeed);
                    break;
                case KeyCode.UpArrow:
                    currentSpeed = Mathf.SmoothStep(currentSpeed, Speed, acceleration);
                    break;
                case KeyCode.DownArrow:
                    currentSpeed = Mathf.SmoothStep(currentSpeed, 0, -acceleration);
                    break;
            }
            transform.localPosition -= transform.forward * Speed / 10;
        }
    }
}
