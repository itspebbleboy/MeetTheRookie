using UnityEngine;
using System.Collections;
using Darklight.Game;
using System.Collections.Generic;
using Darklight.Game.Utility;
using static Darklight.UnityExt.CustomInspectorGUI;

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
        /// <summary>
        /// This is the active focus target for the camera rig.
        /// It acts as an "origin position" for this transform to be located at.
        /// </summary>
        [SerializeField] private Transform _focusTarget;
        public Transform FocusTarget { get { return _focusTarget; } set { _focusTarget = value; } }

        /// <summary>
        /// This offset is used to adjust the position of the camera rig in relation to the focus target.
        /// </summary>
        [SerializeField, ShowOnly] private Vector3 _positionOffset;

        /// <summary>
        /// This offset is used to adjust the rotation of the camera rig in relation to the focus target.
        /// </summary>
        [SerializeField, ShowOnly] private Vector3 _rotationOffset;

        /// <summary>
        /// This offset is used to adjust the field of view of the camera rig based on the default FOV value
        /// </summary>
        [SerializeField, ShowOnly] private float _cameraFovOffset;

        [Header("CameraRig Settings")]
        [SerializeField, Range(0, 10)] private float _followSpeed = 5f;
        [SerializeField, Range(0, 10)] private float _rotationSpeed = 5f;
        [SerializeField, Range(0, 10)] private float _zoomSpeed = 5f;
        [SerializeField, Range(1, 100)] private float _distanceFromTarget = 10f; // typically on the z-axis
        [SerializeField, Range(0.1f, 90)] private float _defaultFOV = 5f;

        public float cameraFOVOffset { get { return _cameraFovOffset; } set { _cameraFovOffset = value; } }
        public float cameraFOV => _defaultFOV + _cameraFovOffset;

        [Header("Cameras")]
        [SerializeField] UnityEngine.Camera[] _cameras = new UnityEngine.Camera[0];

        public virtual void Update()
        {
            // set the offsets
            _positionOffset = new Vector3(0, 0, -_distanceFromTarget);
            _rotationOffset = Vector3.zero;

            // if there is a focus target
            if (_focusTarget)
            {
                // set the position
                Vector3 rigPosition = _focusTarget.position + _positionOffset;
                transform.position = Vector3.Lerp(transform.position, rigPosition, _followSpeed * Time.deltaTime);

                // set the rotation
                Quaternion rigRotation = GetLookRotation(rigPosition, _focusTarget.position + _rotationOffset);
                transform.rotation = Quaternion.Slerp(transform.rotation, rigRotation, _rotationSpeed * Time.deltaTime);
            }

            // update the camera fov
            foreach (UnityEngine.Camera camera in _cameras)
            {
                if (camera != null)
                {
                    // Reset the local position and rotation of the camera
                    camera.transform.localPosition = Vector3.zero;
                    camera.transform.localRotation = Quaternion.identity;

                    // 
                    camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, cameraFOV, _zoomSpeed * Time.deltaTime);
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
}
