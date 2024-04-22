using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capstone {
    public class PlayerCamMove : MonoBehaviour
    {
        private float minZoom = 90f;
        private float maxZoom = 50f;
        private float zoomRateFOV = 720f;
        private float cameraSensitivityHorizontal = 60.0f;
        private float cameraSensitivityRotate = 980.0f;
        // Start is called before the first frame update
        public void checkCameraMove(Camera playerCamera, float dTime)
        {
            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetMouseButton(2))
            {
                float MouseX = Input.GetAxis("Mouse X"); 
                float MouseY = Input.GetAxis("Mouse Y");

                playerCamera.transform.Rotate(Vector3.up, MouseX * cameraSensitivityRotate * dTime);
                playerCamera.transform.Rotate(Vector3.left, MouseY * cameraSensitivityRotate * dTime);

                Vector3 currentRotation = playerCamera.transform.eulerAngles;

                if (currentRotation.x > 180)
                    currentRotation.x -= 360;
                currentRotation.x = Mathf.Clamp(currentRotation.x, -0.1f, 55);
                if (currentRotation.x < 0)
                    currentRotation.x += 360;
                currentRotation.z = 0f;
                playerCamera.transform.eulerAngles = currentRotation;
            } 

            /// ---------- Camera Movement code given player input -------------------
            float Horizontal = Input.GetAxis("Horizontal");
            float Vertical = Input.GetAxis("Vertical");

            if (Horizontal != 0f || Vertical != 0f)
            {
                Transform cameraTransform = playerCamera.transform;

                Vector3 cameraForwardXZ = cameraTransform.forward;
                cameraForwardXZ.y = 0f;
                cameraForwardXZ.Normalize();

                Vector3 cameraRightXZ = cameraTransform.right;
                cameraRightXZ.y = 0f;
                cameraRightXZ.Normalize();

                // Creates a Vector3 based on the moving direction
                Vector3 MoveDirection = cameraForwardXZ * Vertical + cameraRightXZ * Horizontal;
                
                if (MoveDirection.sqrMagnitude > 1)
                {
                    MoveDirection.Normalize();
                }
                // Uses MoveDirection to apply the transformation. Space.world specifies the modifications are
                // being performed on the object's world coordinates not a local coordinate system.
                cameraTransform.Translate(MoveDirection * cameraSensitivityHorizontal * dTime, Space.World);
            }

            float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
            if (scrollWheel > 0f) // Scrolling Up
            {
                if (playerCamera.fieldOfView > maxZoom)
                {
                    // playerCamera.transform.position = new Vector3(currX, currY - zoomRateY * dTime, currZ);
                    // playerCamera.transform.position = new Vector3(currX, Mathf.Clamp(playerCamera.transform.position.y, 12, 20), currZ);
                    playerCamera.fieldOfView -= zoomRateFOV * dTime;
                    playerCamera.fieldOfView = Mathf.Clamp(playerCamera.fieldOfView, 50, 110);
                }
            }
            else if (scrollWheel < 0f) // Scrolling Down
            {
                if (playerCamera.fieldOfView < minZoom)
                {
                    // playerCamera.transform.position = new Vector3(currX, currY + zoomRateY * dTime, currZ);
                    // playerCamera.transform.position = new Vector3(currX, Mathf.Clamp(playerCamera.transform.position.y, 12, 20), currZ);
                    playerCamera.fieldOfView += zoomRateFOV * dTime; 
                    playerCamera.fieldOfView = Mathf.Clamp(playerCamera.fieldOfView, 50, 110);
                }
            }
        }
    }
}
