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
        public SquadMoveMarkerPool moveMarkerPool;

        // --------- Camera Variables ----------
        private float minZoom = 90;
        private float maxZoom = 50;
        private float zoomRateFOV = 720f;
        private float cameraSensitivityHorizontal = 60.0f;
        private float cameraSensitivityRotate = 980.0f;

        // -------- Utility Variables -----------
        float dTime;

        // Start is called before the first frame update
        void Start()
        {
            playerCamera = new GameObject("PlayerCamera").AddComponent<Camera>();
            GameObject ui = Resources.Load("Prefabs/UI/PlayerUI") as GameObject;
            playerCamera.transform.rotation = Quaternion.Euler(60.0f, 0f, 0f);
            playerCamera.fieldOfView = 90f;
            
            int fogCullLayer = LayerMask.NameToLayer("FOV");
            if (fogCullLayer >= 0)
                playerCamera.cullingMask &= ~(1 << fogCullLayer);
                
            int hiddenLayer = LayerMask.NameToLayer("Hidden");
            if (hiddenLayer >= 0)
                playerCamera.cullingMask &= ~(1 << hiddenLayer);
            if (ui != null)
            {
                playerUI = Instantiate(ui).GetComponentInChildren<PlayerUI>();

                playerUI.setPlayerObj(this);
                playerUI.setPlayerCamera(playerCamera);

                playerUI.setFaction(faction);

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
            playerCamera.gameObject.AddComponent<AudioListener>();
            playerUI.name = name + "UI";
            //StartCoroutine(mouseUpdateCoroutine());
            //StartCoroutine(keyboardUpdateCoroutine());
            //StartCoroutine(cameraUpdateCoroutine());
        }

        // Update is called once per frame
        void Update()
        {
            dTime = Time.deltaTime;
            cameraUpdate();
            keyboardUpdate();
            mouseUpdate();
        }

        private void cameraUpdate()
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

        private void keyboardUpdate()
        {
            if (Input.anyKeyDown)
            {
                String keyPress = Input.inputString;

                if (!string.IsNullOrEmpty(keyPress))
                {
                    char keyPressed = keyPress[0];
                    KeyCode keyCode = (KeyCode)keyPressed;

                    keyboardUpdate(keyCode);
                }
            }
        }

        private void mouseUpdate()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit[] hits = Physics.RaycastAll(ray);

            Squad mouseSquad = null;

            foreach (RaycastHit hit in hits)
            {
                if ((hit.collider.gameObject.CompareTag("SquadMember") || playerUI.checkMouseOverWorldIcon(Input.mousePosition, "SquadIcon") != null) && hit.collider.gameObject.layer == LayerMask.NameToLayer("Visible"))
                {
                    SquadMember squadMember = hit.collider.gameObject.GetComponent<SquadMember>(); 

                    if (squadMember == null) {
                        mouseSquad = playerUI.checkMouseOverWorldIcon(Input.mousePosition, "SquadIcon").GetComponent<UnitIconUIWorld>().getReferenceUnitGameObject().GetComponent<Squad>();
                    } else
                        mouseSquad = squadMember.parent;
                    break;
                }
            }

            RaycastHit maskHit;

            if (Physics.Raycast(ray, out maskHit, Mathf.Infinity, ground))
            {
                if (mouseSquad != null) 
                {
                    if (selected.Count == 0 || Input.GetKey(KeyCode.LeftShift)) { mouseSquad.showSelect = true; }
                    else if (selected.Count > 0 && !Input.GetKey(KeyCode.LeftShift)) { mouseSquad.showSelect = true; }
                } 

                if (selected.Count == 1 && Selection.getSelectionComponent<Unit>(selected[0]) is Squad)
                {
                    Squad squad = (Squad)Selection.getSelectionComponent<Unit>(selected[0]);
                    List<RaycastHit> raycastHits;
                    Transform squadLeadTransform = squad.getCurrentTransform();
                    if (squadLeadTransform != null)
                    {
                        raycastHits = Selection.getAdditionalCasts(maskHit, rayCamera, squad.getCurrentTransform(), squad.getAliveMembers(), ground);
                        moveMarkerPool.showMoveMarkers(raycastHits);
                    }
                } else if (selected.Count == 0 && moveMarkerPool.checkActive() == true) {
                    moveMarkerPool.hideMoveMarkers();
                }

                if (Input.GetMouseButtonDown(0))
                {
                    leftClick(mouseSquad);
                }

                if (Input.GetMouseButtonDown(1))
                {
                    rightClick(maskHit);
                }
            }
        }
        private IEnumerator cameraUpdateCoroutine()
        {
            while (true)
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
                yield return null;
            }
        }

        private IEnumerator mouseUpdateCoroutine()
        {
            while (true)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit[] hits = Physics.RaycastAll(ray);

                Squad mouseSquad = null;

                foreach (RaycastHit hit in hits)
                {
                    if ((hit.collider.gameObject.CompareTag("SquadMember") || playerUI.checkMouseOverWorldIcon(Input.mousePosition, "SquadIcon") != null) && hit.collider.gameObject.layer == LayerMask.NameToLayer("Visible"))
                    {
                        SquadMember squadMember = hit.collider.gameObject.GetComponent<SquadMember>(); 

                        if (squadMember == null) {
                            mouseSquad = playerUI.checkMouseOverWorldIcon(Input.mousePosition, "SquadIcon").GetComponent<UnitIconUIWorld>().getReferenceUnitGameObject().GetComponent<Squad>();
                        } else
                            mouseSquad = squadMember.parent;
                        break;
                    }
                }

                RaycastHit maskHit;

                if (Physics.Raycast(ray, out maskHit, Mathf.Infinity, ground))
                {
                    if (mouseSquad != null) 
                    {
                        if (selected.Count == 0 || Input.GetKey(KeyCode.LeftShift)) { mouseSquad.showSelect = true; }
                        else if (selected.Count > 0 && !Input.GetKey(KeyCode.LeftShift)) { mouseSquad.showSelect = true; }
                    } 

                    if (selected.Count == 1 && Selection.getSelectionComponent<Unit>(selected[0]) is Squad)
                    {
                        Squad squad = (Squad)Selection.getSelectionComponent<Unit>(selected[0]);
                        List<RaycastHit> raycastHits;
                        Transform squadLeadTransform = squad.getCurrentTransform();
                        if (squadLeadTransform != null)
                        {
                            raycastHits = Selection.getAdditionalCasts(maskHit, rayCamera, squad.getCurrentTransform(), squad.getAliveMembers(), ground);
                            moveMarkerPool.showMoveMarkers(raycastHits);
                        }
                    } else if (selected.Count == 0 && moveMarkerPool.checkActive() == true) {
                        moveMarkerPool.hideMoveMarkers();
                    }

                    if (Input.GetMouseButtonDown(0))
                    {
                        leftClick(mouseSquad);
                    }

                    if (Input.GetMouseButtonDown(1))
                    {
                        rightClick(maskHit);
                    }
                }
                yield return null;
            }
        }
        
        private IEnumerator keyboardUpdateCoroutine()
        {
            while (true)
                {
                if (Input.anyKeyDown)
                {
                    String keyPress = Input.inputString;

                    if (!string.IsNullOrEmpty(keyPress))
                    {
                        char keyPressed = keyPress[0];
                        KeyCode keyCode = (KeyCode)keyPressed;

                        keyboardUpdate(keyCode);
                    }
                }
                yield return null;
            }
        }

        private void keyboardUpdate(KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.R:
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
                    break;
                case KeyCode.P:
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
                    break;
            }
        }

        private void leftClick(Squad mouseSquad = null)
        {
            if (playerUI.checkMouseOverUIIcon(Input.mousePosition, "UnitIconUI") != null)
            {
                GameObject iconObject = playerUI.checkMouseOverUIIcon(Input.mousePosition, "UnitIconUI");
                UnitIconUI unitIconUI = iconObject.GetComponent<UnitIconUI>();
                unitIconUI.playerClick();
                if (selected.Count > 1)
                    moveMarkerPool.hideMoveMarkers();
                return;
            }
            //if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Selectable"))
            if (mouseSquad != null)
            {
                if (mouseSquad != null && mouseSquad.squadState != SquadState.retreating)
                {
                    Selection.squadSelect(mouseSquad, selected, this);
                    if (selected.Count > 1)
                        moveMarkerPool.hideMoveMarkers();
                    return;
                }
            }
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

        private void rightClick(RaycastHit maskHit)
        {
            if (selected.Count > 0)
            {
                if (selected.Count == 1)
                {
                    if (Selection.getSelectionComponent<Unit>(selected[0]) is Squad)
                    {
                        Squad squad = (Squad)Selection.getSelectionComponent<Unit>(selected[0]);
                        List<RaycastHit> raycastHits;
                        raycastHits = Selection.getAdditionalCasts(maskHit, rayCamera, squad.getCurrentTransform(), squad.getAliveMembers(), ground);
                        squad.moveTo(raycastHits);
                    } 
                } else {
                    // needs work
                    List<RaycastHit> unitSpacingHits = Selection.getMultipleUnitMovePositions(maskHit, rayCamera, selected, ground);
                    for (int i=0; i < unitSpacingHits.Count; i++)
                    {
                        Unit unitComp = Selection.getSelectionComponent<Unit>(selected[i]);
                        if (unitComp is Squad)
                        {
                            Squad squad = (Squad)unitComp;
                            List<RaycastHit> raycastHits;
                            raycastHits = Selection.getAdditionalCasts(unitSpacingHits[i], rayCamera, squad.getCurrentTransform(), squad.getAliveMembers(), ground);
                            squad.moveTo(raycastHits);
                        } 
                    }
                }
            }
        }

        /// <summary>
        /// Contains all logic for selection and clicking commands, called in the Update() function
        /// </summary>
        // private void mouseUpdate() 
        // {
        //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //     RaycastHit[] hits = Physics.RaycastAll(ray);

        //     Squad mouseSquad = null;

        //     foreach (RaycastHit hit in hits)
        //     {
        //         if ((hit.collider.gameObject.CompareTag("SquadMember") || playerUI.checkMouseOverWorldIcon(Input.mousePosition, "SquadIcon") != null) && hit.collider.gameObject.layer == LayerMask.NameToLayer("Visible"))
        //         {
        //             SquadMember squadMember = hit.collider.gameObject.GetComponent<SquadMember>(); 

        //             if (squadMember == null) {
        //                 mouseSquad = playerUI.checkMouseOverWorldIcon(Input.mousePosition, "SquadIcon").GetComponent<UnitIconUIWorld>().getReferenceUnitGameObject().GetComponent<Squad>();
        //             } else
        //                 mouseSquad = squadMember.parent;
        //             break;
        //         }
        //     }

        //     RaycastHit maskHit;

        //     if (Physics.Raycast(ray, out maskHit, Mathf.Infinity, ground))
        //     {
                
        //         if (mouseSquad != null) 
        //         {
        //             if (selected.Count == 0 || Input.GetKey(KeyCode.LeftShift)) { mouseSquad.showSelect = true; }
        //             else if (selected.Count > 0 && !Input.GetKey(KeyCode.LeftShift)) { mouseSquad.showSelect = true; }
        //         } 

        //         if (selected.Count == 1 && Selection.getSelectionComponent<Unit>(selected[0]) is Squad)
        //         {
        //             Squad squad = (Squad)Selection.getSelectionComponent<Unit>(selected[0]);
        //             List<RaycastHit> raycastHits;
        //             Transform squadLeadTransform = squad.getCurrentTransform();
        //             if (squadLeadTransform != null)
        //             {
        //                 raycastHits = Selection.getAdditionalCasts(maskHit, rayCamera, squad.getCurrentTransform(), squad.getAliveMembers(), ground);
        //                 moveMarkerPool.showMoveMarkers(raycastHits);
        //             }
        //         } else if (selected.Count == 0 && moveMarkerPool.checkActive() == true) {
        //             moveMarkerPool.hideMoveMarkers();
        //         }

        //         // Check if LMB has been clicked
        //         if (Input.GetMouseButtonDown(0))
        //         {

        //         }
        //         // Check if RMB has been clicked
        //         if (Input.GetMouseButton(1))
        //         {

        //         }
        //     }
        // }
        

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
