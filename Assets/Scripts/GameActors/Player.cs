using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capstone
{
    public class Player : GameActor
    {
        private Camera playerCamera;
        private GameObject playerUI;
        private Unit[] selectedUnits;

        // --------- Camera Variables ----------
        private float minZoom = 130;
        private int minZoomY = 8;
        private float maxZoom = 50;
        private int maxZoomY = 4;
        private float zoomRateFOV = 5;
        private float zoomRateY = 0.25f;
        private float cameraSensitivity = 10.0f;
        // -------------------------------------

        // Start is called before the first frame update
        void Start()
        {
            playerCamera = new GameObject("PlayerCamera").AddComponent<Camera>();
            GameObject ui = Resources.Load("Prefabs/UI/PlayerUI") as GameObject;
            //playerCamera.transform.parent = cameraParent.transform;
            //cameraParent.transform.position = new Vector3(0,6,0);
            //cameraParent.transform.rotation = Quaternion.Euler(45.0f, 0f, 0f);
            playerCamera.transform.position = new Vector3(0,6,0);
            playerCamera.transform.rotation = Quaternion.Euler(45.0f, 0f, 0f);

            if (ui != null)
            {
                playerUI = Instantiate(ui);

                Camera uiCamera = playerUI.GetComponentInChildren<Camera>();

                if (uiCamera != null)
                {
                    uiCamera.transform.parent = playerCamera.transform;
                }
            }
            if (Camera.main != null) {
                Camera.main.tag = "Untagged";
            }
            
            playerCamera.tag = "MainCamera";
            playerUI.name = this.name + " UI";
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetMouseButton(2))
            {
                float MouseX = Input.GetAxis("Mouse X"); 
                float MouseY = Input.GetAxis("Mouse Y");

                playerCamera.transform.Rotate(Vector3.up, MouseX * cameraSensitivity);
                playerCamera.transform.Rotate(Vector3.left, MouseY * cameraSensitivity);

                Vector3 currentRotation = playerCamera.transform.eulerAngles;
                currentRotation.z = 0f;
                playerCamera.transform.eulerAngles = currentRotation;
            } 

            /// ---------- Camera Movement code given player input -------------------
            Transform cameraTransform = playerCamera.transform;
            float currX = cameraTransform.position.x;
            float currY = cameraTransform.position.y;
            float currZ = cameraTransform.position.z; 

            float Horizontal = Input.GetAxis("Horizontal");
            float Vertical = Input.GetAxis("Vertical");
            // Creates a Vector3 based on the moving direction of the camera (1/-1 or 0 for no movement)
            Vector3 MoveDirection = new Vector3(Horizontal, 0f, Vertical);
            // Converts the relative local facing to a world translation
            MoveDirection = cameraTransform.TransformDirection(MoveDirection);
            // Uses MoveDirection to apply the transformation. Space.world specifies the modifications are
            // being performed on the object's world coordinates not a local coordinate system.
            cameraTransform.Translate(0.5f * new Vector3(MoveDirection.x, 0f, MoveDirection.z), Space.World);

            float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
            if (scrollWheel > 0f) // Scrolling Up
            {
                if ((playerCamera.transform.position.y > maxZoomY) & (playerCamera.fieldOfView > maxZoom))
                {
                    playerCamera.transform.position = new Vector3(currX, currY - zoomRateY, currZ);
                    playerCamera.fieldOfView -= zoomRateFOV;
                }
            }
            else if (scrollWheel < 0f) // Scrolling Down
            {
                if ((playerCamera.transform.position.y < minZoomY) & (playerCamera.fieldOfView < minZoom))
                {
                   playerCamera.transform.position = new Vector3(currX, currY + zoomRateY, currZ);
                   playerCamera.fieldOfView += zoomRateFOV; 
                }
            }

            // ---------------------------------------------------------------------------

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                
                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log(hit.collider.gameObject.layer);
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Selectable"))
                    {
                        
                    }

                    /*SquadMember squadMember = hit.collider.gameObject.GetComponent<SquadMember>();

                    if (squadMember == null && hit.collider.transform.parent != null)
                    {
                        // Check the parent for SquadMember component
                        squadMember = hit.collider.transform.parent.parent.gameObject.GetComponent<SquadMember>();
                    }

                    if (squadMember != null)
                    {
                        Debug.Log("Squad Member Clicked!");
                    }*/
                }
            }
        }

        float normalizeAngles(float angle) {
            while (angle < 0f)
            {
                angle += 360f;
            } 
            while (angle >= 360f) 
            {
                angle -= 360f;
            }
            return angle;
        }
    }
}
