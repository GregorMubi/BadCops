using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour {

    private SimpleCarController CarController = null;
    private bool ControlEnabled = true;

    private static PlayerInputController Instance;

    public static Vector3 GetPlayerCarPosition() {
        if (Instance != null && Instance.CarController != null) {
            return Instance.CarController.transform.position;
        } else {
            return Vector3.zero;
        }
    }

    void Start() {
        Instance = this;
    }

    public SimpleCarController GetCarController() {
        return CarController;
    }

    public void Init(SimpleCarController carController) {
        CarController = carController;
        CarController.tag = "Player";
    }

    public void SetControllEnabled(bool enabled) {
        ControlEnabled = enabled;
    }

    void FixedUpdate() {
        if (CarController != null && ControlEnabled) {
            float motor = Input.GetAxis("Vertical");
            float steering = Input.GetAxis("Horizontal");
            CarController.UpdateInput(motor, steering);
        }
    }
}
