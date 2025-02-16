using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;

public class CameraTracker : MonoBehaviour
{
    public Camera mainCamera;
    public UIManager uiManager;
    [Header("目標物件")]
    public Transform targetTransform;
    [Header("目前位置")]
    public Vector3 currentPosition;
    public Quaternion currentRotation;
    [Header("位移曲線")]
    public AnimationCurve dampCurve;
    [Header("是否在跟隨")]
    public bool isFollowing;
    public bool isLockMovement;
    public float currentFollowingTime;
    [Header("位移總時長")]
    public float followingPeriod;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        uiManager = UIManager.instance;
        ResetCurrentTransformDetail();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetCameraTarget(Transform _targetTransform)
    {
        if (isLockMovement) { return; }
        targetTransform = _targetTransform;
        currentFollowingTime = 0;
        SetCameraIsFollowing(true);
        ResetCurrentTransformDetail();
        DampCamera();
    }
    public void SetCameraIsFollowing(bool value)
    {
        isFollowing = value;
    }
    public float GetDampCurveValue()
    {
        float value = dampCurve.Evaluate(currentFollowingTime / followingPeriod);
        return value;
    }
    public async void DampCamera()
    {
        isLockMovement = true;
        while (isFollowing)
        {
            mainCamera.transform.position = Vector3.Lerp(currentPosition, targetTransform.position, GetDampCurveValue());
            mainCamera.transform.rotation = Quaternion.Lerp(currentRotation, targetTransform.rotation, GetDampCurveValue());

            if (currentFollowingTime < followingPeriod)
            {
                currentFollowingTime += Time.deltaTime;
            }
            else
            {
                SetCameraIsFollowing(false);
                ResetCurrentTransformDetail();
                currentFollowingTime = 0;
                isLockMovement = false;
            }
            await Task.Yield();
        }
    }
    public void ResetCurrentTransformDetail()
    {
        currentPosition = transform.position;
        currentRotation = transform.rotation;
    }
}
