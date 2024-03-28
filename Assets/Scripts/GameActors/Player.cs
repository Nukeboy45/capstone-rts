using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capstone
{
    public class Player : GameActor
    {
        // --------- Player Specific Variables -------
        private Camera playerCamera;
        public PlayerUI playerUI;
        public List<GameObject> selected = new List<GameObject>();
        public squadMoveMarkerPool moveMarkerPool;

        // --------- Camera Variables ----------
        private float minZoom = 110;
        private int minZoomY = 8;
        private float maxZoom = 50;
        private int maxZoomY = 4;
        private float zoomRateFOV = 5;
        private float zoomRateY = 15.0f;
        private float cameraSensitivityHorizontal = 60.0f;
        private float cameraSensitivityRotate = 5.0f;

        // -------- Utility Variables -----------
        private float dTime;

        // Start is called before the first frame update
        void Start()
        {
            playerCamera = new GameObject("PlayerCamera").AddComponent<Camera>();
            GameObject ui = Resources.Load("Prefabs/UI/PlayerUI") as GameObject;
            playerCamera.transform.position = new Vector3(0,6,0);
            playerCamera.transform.rotation = Quaternion.Euler(45.0f, 0f, 0f);
            
            int fogCullLayer = LayerMask.NameToLayer("FOV");
            if (fogCullLayer >= 0)
                playerCamera.cullingMask &= ~(1 << fogCullLayer);
                
            if (ui != null)
            {
                playerUI = Instantiate(ui).GetComponentInChildren<PlayerUI>();

                playerUI.setPlayerObj(this);

                playerUI.setFaction(this.faction);

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
            dTime = Time.deltaTime;
            cameraUpdate();
            mouseUpdate();
            keyboardUpdate();
        }

        /// <summary>
        /// Contains all logic for camera movement, called at the very start of Update()
        /// </summary>
        private void cameraUpdate() 
        {
            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetMouseButton(2))
            {
                float MouseX = Input.GetAxis("Mouse X"); 
                float MouseY = Input.GetAxis("Mouse Y");

                playerCamera.transform.Rotate(Vector3.up, MouseX * cameraSensitivityRotate);
                playerCamera.transform.Rotate(Vector3.left, MouseY * cameraSensitivityRotate);

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
            cameraTransform.Translate(cameraSensitivityHorizontal * dTime * new Vector3(MoveDirection.x, 0f, MoveDirection.z), Space.World);

            float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
            if (scrollWheel > 0f) // Scrolling Up
            {
                if ((playerCamera.transform.position.y > maxZoomY) & (playerCamera.fieldOfView > maxZoom))
                {
                    playerCamera.transform.position = new Vector3(currX, currY - zoomRateY * dTime, currZ);
                    playerCamera.fieldOfView -= zoomRateFOV;
                }
            }
            else if (scrollWheel < 0f) // Scrolling Down
            {
                if ((playerCamera.transform.position.y < minZoomY) & (playerCamera.fieldOfView < minZoom))
                {
                    playerCamera.transform.position = new Vector3(currX, currY + zoomRateY * dTime, currZ);
                    playerCamera.fieldOfView += zoomRateFOV; 
                }
            }
        }

        /// <summary>
        /// Contains all logic for selection and clicking commands, called in the Update() function
        /// </summary>
        private void mouseUpdate() 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            SquadMember squadMember;
            PassiveBuilding passiveBuilding;
            //DefenseBuilding defenseBuilding;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickable))
            {   
                Transform hitTransform = hit.collider.transform;

                squadMember = Selection.getSelectionComponent<SquadMember>(hitTransform);

                passiveBuilding = Selection.getSelectionComponent<PassiveBuilding>(hitTransform);

                if (squadMember != null) 
                {
                    if (selected.Count == 0 || Input.GetKey(KeyCode.LeftShift)) { squadMember.parent.showSelect = true; }
                    else if (selected.Count > 0 && !Input.GetKey(KeyCode.LeftShift)) { squadMember.parent.showSelect = true; }
                }

                if (selected.Count == 1 && Selection.getSelectionComponent<Unit>(selected[0]) is Squad)
                {
                    Squad squad = (Squad)Selection.getSelectionComponent<Unit>(selected[0]);
                    List<RaycastHit> hits = new List<RaycastHit>();
                    hits = Selection.getAdditionalCasts(hit, rayCamera, squad.getCurrentTransform(), squad.getAliveMembers(), ground);
                    moveMarkerPool.showMoveMarkers(hits);
                }

                // Check if LMB has been clicked
                if (Input.GetMouseButtonDown(0))
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Selectable"))
                    {
                        if (squadMember != null && squadMember.parent.squadState != SquadState.retreating)
                        {
                            Selection.squadSelect(squadMember, selected, this);
                        }

                        if (passiveBuilding != null)
                        {
                            Selection.buildingSelect(passiveBuilding, selected, this);
                        }
                        
                    } else {
                        // Assuming player is not in multi-select mode, deselect all units and hide any move markers
                        if (!Input.GetKey(KeyCode.LeftShift)) 
                        { 
                            Selection.deselectAll(this); 
                            if (moveMarkerPool.markersActive == true)
                            {
                                moveMarkerPool.hideMoveMarkers();
                            }
                        } 
                    }
                }
                // Check if RMB has been clicked
                if (Input.GetMouseButton(1))
                {
                    if (selected.Count > 0)
                    {
                        if (selected.Count == 1)
                        {
                            if (Selection.getSelectionComponent<Unit>(selected[0]) is Squad)
                            {
                                Squad squad = (Squad)Selection.getSelectionComponent<Unit>(selected[0]);
                                List<RaycastHit> hits = new List<RaycastHit>();
                                hits = Selection.getAdditionalCasts(hit, rayCamera, squad.getCurrentTransform(), squad.getAliveMembers(), ground);
                                squad.moveTo(hits);
                            } else if (Selection.getSelectionComponent<Building>(selected[0]) is Building) {
                                Building building = Selection.getSelectionComponent<Building>(selected[0]);
                                if (building is PassiveBuilding)
                                {
                                    PassiveBuilding passiveBuildComp = (PassiveBuilding)building;
                                    passiveBuildComp.setRallyPoint(hit);
                                }
                            }
                        } else {
                            foreach (GameObject gameObject in selected)
                            {
                                Unit unit = Selection.getSelectionComponent<Unit>(gameObject);
                            }
                        }
                    }
                }
            }
        }
        
        private void keyboardUpdate()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (selected.Count > 0)
                {
                    foreach (GameObject gameObject in selected)
                    {
                        if (Selection.getSelectionComponent<Unit>(gameObject) is Unit)
                        {
                            Unit unitComp = Selection.getSelectionComponent<Unit>(gameObject);
                            if (unitComp is Squad)
                            {
                                Ray ray = rayCamera.ScreenPointToRay(rayCamera.WorldToScreenPoint(spawnPoint.transform.position));
                                RaycastHit hit;

                                if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
                                {
                                    Squad squad = (Squad)unitComp;
                                    List<RaycastHit> hits = new List<RaycastHit>();
                                    hits = Selection.getAdditionalCasts(hit, rayCamera, squad.getCurrentTransform(), squad.getAliveMembers(), ground);
                                    squad.retreat(hits);
                                }
                            }
                        }
                    }
                    Selection.deselectAll(this);
                }
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                if (selected.Count > 0)
                {
                    foreach (GameObject gameObject in selected)
                    {
                        Unit unitComp = Selection.getSelectionComponent<Unit>(gameObject);
                        if (unitComp is Squad)
                        {
                            Squad squadComp = (Squad)unitComp;
                            squadComp.dealDebugDamage(10f);
                        }
                    }
                }
            }
        }

        // ------------------- Getter / Setter / Accessibility Functions ----------------
        public List<GameObject> getSelected() {
            return selected;
        }

        public Camera getPlayerCamera()
        {
            return playerCamera;
        }

        public void setCameraPosition(Vector3 position)
        {
            playerCamera.transform.position = position;
        }

    }
}
