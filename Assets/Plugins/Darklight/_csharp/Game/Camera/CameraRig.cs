using UnityEngine;
using System.Collections;
using Darklight.Game;
using System.Collections.Generic;
using Darklight.Utility;
using Darklight.UnityExt.Editor;
using static Darklight.UnityExt.Editor.CustomInspectorGUI;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.Game.Camera
{
    /// <summary>
    /// This Camera Rig is the main Monobehaviour reference for the full Camera System. 
    /// It should be set as the parent object for all cameras in the scene.
    /// </summary>
    [ExecuteAlways]
    public class CameraRig : MonoBehaviour
    {

        // << GET ALL CAMERAS IN CHILDREN >> -----------------------------------
        [Header("Cameras")]
        [Tooltip("All cameras that are children of this object.")]
        [SerializeField] UnityEngine.Camera[] _camerasInChildren = new UnityEngine.Camera[0];
        public void GetCamerasInChildren()
        {
            _camerasInChildren = GetComponentsInChildren<UnityEngine.Camera>();
        }

        // << FOCUS TARGET >> -------------------------------------------------
        [Header("Focus Target")]
        [Tooltip("The target that the camera will focus on.")]
        [SerializeField] private Transform _focusTarget;
        [SerializeField, ShowOnly] private Vector3 _focusTargetPosition = Vector3.zero;
        [SerializeField, ShowOnly] private Vector3 _focusTargetPositionOffset = Vector3.zero;
        [SerializeField, Range(-5f, 5)] float _focusOffsetY = 0f;
        public void SetFocusTarget(Transform target)
        {
            _focusTarget = target;
        }

        [SerializeField, ShowOnly] private Vector3 _offsetPosition;
        public void SetOffsetRotation(Transform mainTarget, Transform secondTarget)
        {
            float mainX = mainTarget.position.x;
            float secondX = secondTarget.position.x;
            float middleX = (secondX - mainX) / 2;
        }

        [Header("Lerp Speed")]
        [SerializeField, Range(0, 10)] private float _positionLerpSpeed = 5f;
        [SerializeField, Range(0, 10)] private float _rotationLerpSpeed = 5f;
        [SerializeField, Range(0, 10)] private float _fovLerpSpeed = 5f;

        // TODO : Rotate the Camera around the target
        // this is instead of including _distanceX

        [Space(10), Header("Distance")]
        [SerializeField, Range(-50, 50)] private float _distanceY = 0f; // distance from the target on the Y axis
        [SerializeField, Range(0, 100)] private float _distanceZ = 10f; // distance from the target on the Z axis



        [Header("Field of View")]
        [SerializeField, Range(0.1f, 190)] private float _baseFOV = 5f;
        [SerializeField, ShowOnly] private float _offsetFOV;
        public void SetOffsetFOV(float value)
        {
            _offsetFOV = value;
        }
        [SerializeField, ShowOnly] private float _currentFOV;
        public float GetCurrentFOV()
        {
            _currentFOV = _baseFOV + _offsetFOV;
            return _currentFOV;
        }

        public virtual void Update()
        {
            GetCamerasInChildren();

            // << IF THERE IS NO FOCUS TARGET, SET THE POSITION TO ZERO >>
            if (_focusTarget == null)
            {
                _focusTargetPosition = Vector3.zero;
            }
            else
            {
                _focusTargetPosition = _focusTarget.position;
            }

            // set the offsets
            _offsetPosition = new Vector3(0, _distanceY, -_distanceZ);
            _focusTargetPositionOffset = new Vector3(0, _focusOffsetY, 0);

            // set the position
            Vector3 newPosition = _focusTargetPosition + _offsetPosition;
            transform.position = Vector3.Lerp(transform.position, newPosition, _positionLerpSpeed * Time.deltaTime);

            // set the rotation
            Quaternion newRotation = GetLookRotation(newPosition, _focusTargetPosition + _focusTargetPositionOffset);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, _rotationLerpSpeed * Time.deltaTime);

            // << UPDATE ALL CAMERAS >>
            foreach (UnityEngine.Camera camera in _camerasInChildren)
            {
                if (camera != null)
                {
                    // Reset the local position and rotation of the camera
                    camera.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

                    // Lerp the field of view
                    camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, GetCurrentFOV(), _fovLerpSpeed * Time.deltaTime);
                }
            }
        }

        Quaternion GetLookRotation(Vector3 originPosition, Vector3 targetPosition)
        {
            Vector3 direction = (targetPosition - originPosition).normalized;
            if (direction == Vector3.zero) return Quaternion.identity;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            return lookRotation;
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(CameraRig))]
    public class CameraRigCustomEditor : Editor
    {
        SerializedObject _serializedObject;
        CameraRig _script;
        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (CameraRig)target;
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
}
