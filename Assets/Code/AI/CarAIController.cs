using UnityEngine;

public class CarAIController : MonoBehaviour {

    private CarAIData Data;
    private SimpleCarController CarController;
    private int TargetKeyIndex = 0;
    private bool CanMove = false;

    public void Init(CarAIData data) {
        Data = data;

        if (Data.Keys.Count > 0) {
            SimpleCarController prefab = LevelManager.Instance.GetCarsList()[Data.CarIndex];
            CarController = Instantiate(prefab);
            CarController.transform.SetParent(transform);
            if (Data.Keys.Count > 1) {
                Vector3 spawnLocation = Data.GetKey(0);
                Vector3 targetVector = Data.GetKey(1) - Data.GetKey(0);
                CarController.SetTransform(spawnLocation, Mathf.Atan2(targetVector.x, targetVector.z) * Mathf.Rad2Deg);
                CanMove = true;
                TargetKeyIndex = 1;
            }
        }

    }

    void FixedUpdate() {
        if (CarController == null || !CanMove) {
            return;
        }

        float dist = (Data.GetKey(TargetKeyIndex) - CarController.transform.position).magnitude;
        if (dist < 1.0f) {
            TargetKeyIndex = (TargetKeyIndex + 1) % Data.Keys.Count;
        }

        float motor = 0;
        float steering = 0;
        Vector3 targetVector = Data.GetKey(TargetKeyIndex) - CarController.transform.position;
        Vector3 forwardVector = CarController.transform.forward;
        float targetAngle = (float)StaticGameHelper.AngleBetweenVectors(targetVector, forwardVector);
        float maxAngle = CarController.GetMaxSteeringAngle();
        if (targetAngle < maxAngle && targetAngle > -maxAngle) {
            motor = 1;
            steering = targetAngle / maxAngle;
        } else {
            if (targetAngle > maxAngle) {
                motor = 1 - (targetAngle - maxAngle) / (180 - maxAngle) * 0.5f;
                steering = 1;
            } else if (targetAngle < -maxAngle) {
                steering = -1;
                motor = 1 - (targetAngle + maxAngle) / (-180 + maxAngle) * 0.5f;
            }
        }
        CarController.UpdateInput(motor * Data.MotorMultiplier, steering * Data.WheelMultiplier);
    }

    public SimpleCarController GetCarController() {
        return CarController;
    }
}
