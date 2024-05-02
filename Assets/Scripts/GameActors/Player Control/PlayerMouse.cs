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
        //private Camera playerCamera;
        private Camera rayCamera;
        private Vector3 prevMousePosition = Vector3.zero;
        



        private Squad mouseSquad;    
        private RaycastHit maskHit;
        public void mouseUpdate(List<GameObject> selected = null)
        {
            if (Input.mousePosition != prevMousePosition) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                prevMousePosition = Input.mousePosition;
            

                RaycastHit[] hits = Physics.RaycastAll(ray);

                mouseSquad = null;

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
                }

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
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                leftClick(selected, mouseSquad);
            }

            if (Input.GetMouseButtonDown(1))
            {
                rightClick(selected, maskHit);
            }
        }

        private void leftClick(List<GameObject> selected, Squad mouseSquad = null)
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
                    Selection.squadSelect(mouseSquad, selected, player, SelectMode.click);
                    if (selected.Count > 1)
                        moveMarkerPool.hideMoveMarkers();
                    return;
                }
            }
                // Assuming player is not in multi-select mode, deselect all units and hide any move markers
            if (!Input.GetKey(KeyCode.LeftShift)) 
            { 
                Selection.deselectAll(player); 
                if (moveMarkerPool.markersActive == true)
                {
                    moveMarkerPool.hideMoveMarkers();
                }
            } 
        }

        private void rightClick(List<GameObject> selected, RaycastHit maskHit)
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
        public void setMovePool(SquadMoveMarkerPool newPool) { moveMarkerPool = newPool;}
        public void setPlayerUI(PlayerUI newUI) { playerUI = newUI;}
        //public void setPlayerCamera(Camera newCamera) { playerCamera = newCamera;}
        public void setRayCamera(Camera newCamera) { rayCamera = newCamera;}
    }
}
