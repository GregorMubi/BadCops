using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField]
    private float Speed = 10f;

    private Quaternion targetRotation;
    public float smooth = 1f;

    private enum CarDirection
    {
        Up,
        Down,
        Left,
        Right,
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

        Move();

    }

    private void Turn(CarDirection direction) {
        float angle = 0;
        switch (direction) {
            case CarDirection.Up:
                angle = 180;
                break;
            case CarDirection.Down:
                angle = 0;
                break;
            case CarDirection.Left:
                angle = 90;
                break;
            case CarDirection.Right:
                angle = -90;
                break;
        }
        targetRotation = Quaternion.AngleAxis(angle, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10 * smooth * Time.deltaTime);

    }

    private void OnButtonPressed(KeyCode keyCode) {
        switch (keyCode) {
            case KeyCode.LeftArrow:
                Turn(CarDirection.Left);
                break;
            case KeyCode.RightArrow:
                Turn(CarDirection.Right);
                break;
            case KeyCode.UpArrow:
                Turn(CarDirection.Up);
                break;
            case KeyCode.DownArrow:
                Turn(CarDirection.Down);
                break;
                }
    }

    private void Move() {
        transform.localPosition += transform.forward * Speed/100;
    }
}
