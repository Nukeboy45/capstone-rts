using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capstone
{
    public class Player : GameActor
    {
        // ---Private, Editor-Accessible Variables ---
        
        // Class References 
        [SerializeField] private SquadMoveMarkerPool moveMarkerPool;
        [SerializeField] private DragSelect dragSelect;
        [SerializeField] private PlayerCamMove camMove;
        [SerializeField] private PlayerMouse mouse;
        [SerializeField] private PlayerKeyboard keyboard;

        // --- Private Runtime Variables ---

        // Class References
        private PlayerUI playerUI;
        // Variables
        private Camera playerCamera;
        [SerializeField] private List<GameObject> selected = new List<GameObject>();

        // -------- Utility Variables -----------
        float dTime;

        // Start is called before the first frame update
        void Start()
        {
            Cursor.visible = false;
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
            
            dragSelect.setPlayer(this);
            //dragSelect.setSelectedList(selected);

            mouse.setPlayer(this);
            mouse.setPlayerCamera(playerCamera);
            mouse.setMovePool(moveMarkerPool);
            mouse.setRayCamera(rayCamera);

            keyboard.setPlayer(this);
            keyboard.setRayCamera(rayCamera);
            // keyboard.setSpawnPoint(spawnPoint);
            

            if (ui != null)
            {
                playerUI = Instantiate(ui).GetComponentInChildren<PlayerUI>();

                playerUI.setPlayerObj(this);
                playerUI.setPlayerCamera(playerCamera);

                playerUI.setFaction(faction);

                dragSelect.setSelectionBoxGraphic(playerUI.getBoxSelectionGraphic());
                dragSelect.setPlayerCamera(playerCamera);

                mouse.setPlayerUI(playerUI);
            }
            if (Camera.main != null) {
                Camera.main.tag = "Untagged";
            }
            
            playerCamera.tag = "MainCamera";
            playerCamera.gameObject.AddComponent<AudioListener>();
            playerUI.name = name + "UI";
            //StartCoroutine(cameraMove());
            //StartCoroutine(keyboardUpdateCoroutine());
            //StartCoroutine(cameraUpdateCoroutine());
        }

        void Update()
        {
            dTime = Time.deltaTime;
            camMove.checkCameraMove(playerCamera, dTime);
            mouse.mouseUpdate(ref selected);
            dragSelect.dragSelectUpdate(ref selected);
            keyboard.keyboardUpdate(ref selected);
        }

        // ------------------- Getter / Setter / Accessibility Functions ----------------
        public List<GameObject> getSelected() { return selected; }

        public Camera getPlayerCamera() { return playerCamera; }

        public PlayerUI GetPlayerUI() { return playerUI; }

        public void setCameraPosition(Vector3 position) { playerCamera.transform.position = position; }

    }
}
