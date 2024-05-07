using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capstone {
    public class PlayerMouse : MonoBehaviour
    {
        // Start is called before the first frame update
        
        // -- Private, Editor-Accessible Variables -- 
        [SerializeField] private LayerMask ground;
        [SerializeField] private LayerMask clickable;
        
        // -- Private Runtime Variables --
        private GameActor player;
        private SquadMoveMarkerPool moveMarkerPool;
        private PlayerUI playerUI;
        private Camera playerCamera;
        private Camera rayCamera;
        private Vector3 prevMousePosition = Vector3.zero;
        private Vector3 prevCameraPosition = Vector3.zero;
        



        private Squad mouseSquad;    
        private OwnedStructure mouseOwnedStructure;
        private RaycastHit maskHit;
        public void mouseUpdate(ref List<GameObject> selected)
        {
            if (Input.mousePosition != prevMousePosition || playerCamera.transform.position != prevCameraPosition || mouseSquad != null || mouseOwnedStructure != null) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                prevMousePosition = Input.mousePosition;
                prevCameraPosition = playerCamera.transform.position;
            

                RaycastHit[] hits = Physics.RaycastAll(ray);

                mouseSquad = null;

                mouseOwnedStructure = null;

                foreach (RaycastHit hit in hits)
                {
                    if ((hit.collider.gameObject.CompareTag("SquadMember") && hit.collider.gameObject.layer == LayerMask.NameToLayer("Visible")) || playerUI.checkMouseOverWorldIcon(Input.mousePosition, "SquadIcon") != null)
                    {
                        SquadMember squadMember = hit.collider.gameObject.GetComponent<SquadMember>(); 

                        if (squadMember == null) {
                            mouseSquad = playerUI.checkMouseOverWorldIcon(Input.mousePosition, "SquadIcon").GetComponent<UnitIconUIWorld>().getReferenceUnitGameObject().GetComponent<Squad>();
                        } else
                            mouseSquad = squadMember.parent;
                        break;
                    }
                    if (hit.collider.gameObject.CompareTag("OwnedStructure") && hit.collider.gameObject.layer == LayerMask.NameToLayer("Visible"))
                    {
                        mouseOwnedStructure = hit.collider.gameObject.GetComponent<OwnedStructure>();
                    }
                }

                if (Physics.Raycast(ray, out maskHit, Mathf.Infinity, ground))
                {
                    // Selection Radiuses - Squad
                    if (mouseSquad != null) 
                    {
                        if (selected.Count == 0 || Input.GetKey(KeyCode.LeftShift)) { mouseSquad.showSelect = true; /*mouseSquad.showSelectTime = Time.time;*/ }
                        else if (selected.Count > 0 && !Input.GetKey(KeyCode.LeftShift)) { mouseSquad.showSelect = true; /*mouseSquad.showSelectTime = Time.time;*/ }
                    } 

                    // Selection radiuses - Building 
                    if (mouseOwnedStructure != null)
                    {
                        if (selected.Count == 0 || Input.GetKey(KeyCode.LeftShift)) { mouseOwnedStructure.showSelect = true; }
                        else if (selected.Count > 0 && !Input.GetKey(KeyCode.LeftShift)) { mouseOwnedStructure.showSelect = true; }
                    }

                    if (selected.Count == 1 && /*Selection.getSelectionComponent<Unit>(selected[0]) is Squad*/selected[0].GetComponent<Unit>() != null)
                    {
                        Unit unitComponent = selected[0].GetComponent<Unit>();
                        if (unitComponent is Squad)
                        {
                            //Squad squad = (Squad)Selection.getSelectionComponent<Unit>(selected[0]);
                            Squad squad = (Squad)unitComponent;
                            List<RaycastHit> raycastHits;
                            Transform squadLeadTransform = squad.getCurrentTransform();
                            if (squadLeadTransform != null)
                            {
                                raycastHits = Selection.getAdditionalCasts(maskHit, rayCamera, squad.getCurrentTransform(), squad.getAliveMembers(), ground);
                                moveMarkerPool.showMoveMarkers(raycastHits);
                            }
                        }
                    } else if (selected.Count == 1 && selected[0].GetComponent<OwnedStructure>() != null) {

                    } else if (selected.Count == 0 && moveMarkerPool.checkActive() == true) {
                        moveMarkerPool.hideMoveMarkers();
                    }
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                leftClick(ref selected);
            }

            if (Input.GetMouseButtonDown(1))
            {
                rightClick(ref selected, maskHit);
            }
        }

        private void leftClick(ref List<GameObject> selected)
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
                    if (Selection.checkBuildingInSelected(selected))
                        Selection.removeBuildingInSelected(ref selected);
                    Selection.squadSelect(mouseSquad, selected, player, SelectMode.click);
                    if (selected.Count > 1)
                        moveMarkerPool.hideMoveMarkers();
                    return;
                }
            }

            else if (mouseOwnedStructure != null)
            {
                Selection.buildingSelect(mouseOwnedStructure, selected, player);
                if (moveMarkerPool.checkActive() == true)
                    moveMarkerPool.hideMoveMarkers();
            }
                // Assuming player is not in multi-select mode, deselect all units and hide any move markers
            else if (!Input.GetKey(KeyCode.LeftShift) && selected.Count > 0) 
            { 
                Selection.deselectAll(player); 
                if (moveMarkerPool.markersActive == true)
                {
                    moveMarkerPool.hideMoveMarkers();
                }
            } 
        }

        private void rightClick(ref List<GameObject> selected, RaycastHit maskHit)
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

        // -- Getter / Setter Methods --
        public void setPlayer(GameActor newPlayer) { player = newPlayer;}
        public void setPlayerCamera(Camera newCamera) { playerCamera = newCamera; }
        public void setMovePool(SquadMoveMarkerPool newPool) { moveMarkerPool = newPool;}
        public void setPlayerUI(PlayerUI newUI) { playerUI = newUI;}
        //public void setPlayerCamera(Camera newCamera) { playerCamera = newCamera;}
        public void setRayCamera(Camera newCamera) { rayCamera = newCamera;}
    }
}
