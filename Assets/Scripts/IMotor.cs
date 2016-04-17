using UnityEngine;

public interface IMotor {

    float GetSpeed();
    Vector3 GetDirection();
    void SetRelativeDestination(Vector3 amount);
    void UpdateMotion(float deltaTime);
}
