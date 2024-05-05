using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Capstone {
    public class PlayerKeyboard : MonoBehaviour
    {
        // -- Private, Editor-Accessible Variables -- 
        [SerializeField] private LayerMask ground;
        [SerializeField] private LayerMask clickable;

        // -- Private, Runtime Variables --
        private SpawnPoint spawnPoint;
        private GameActor player;
        private Camera rayCamera;
        private bool keycheckRunning = false;

        public void keyboardUpdate(List<GameObject> selected)
        {
            if (Input.anyKeyDown)
            {
                String keyPress = Input.inputString;

                if (!string.IsNullOrEmpty(keyPress))
                {
                    char keyPressed = keyPress[0];
                    KeyCode keyCode = (KeyCode)keyPressed;

                    if (!keycheckRunning)
                        keycodeCheck(keyCode, selected);
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                    GameManager.Instance.pause();
                    
            }
        }
        private void keycodeCheck(KeyCode keyCode, List<GameObject> selected)
        {
            keycheckRunning = true;
            Debug.Log(keyCode);
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
                        Selection.deselectAll(player);
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
            keycheckRunning = false;
        }

        // -- Getters / Setter Methods --
        public void setPlayer(GameActor newPlayer) { player = newPlayer;}
        public void setRayCamera(Camera newCamera) { rayCamera = newCamera;}
        public void setSpawnPoint(SpawnPoint newSpawnPoint) { spawnPoint = newSpawnPoint;}
    }
}
