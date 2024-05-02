using System;
using System.Collections.Generic;
using System.Collections;

//using System.Numerics;
using UnityEngine;

// Credit to 'Multiple Unit Selection in Unity...' by 'SpawnCampGames' on YouTube for ideas on this script and others : https://www.youtube.com/watch?v=vAVi04mzeKk&t=746s

namespace Capstone {
    public class DragSelect : MonoBehaviour
    {
        // Start is called before the first frame update
        private Camera playerCam;
        [SerializeField] private RectTransform boxGraphic;
        [SerializeField] LayerMask ground;
        [SerializeField] private LayerMask selectLayers;
        [SerializeField] GameObject debugPrefab;
        public List<GameObject> collisions;
        private GameObject selection;
        private GameActor player;
        private List<GameObject> selected;
        private List<String> unitTags = new List<String>{"SquadMember"};
        private float mouseDownTime;

        // Mesh Testing
        private SelectionBox selectBox;
        private MeshFilter selectionFilter;
        private MeshCollider selectionCollider;
        private Vector3 startMousePosition;
        private Vector3 endMousePosition;
        private LayerMask includeLayer;
        private LayerMask excludeLayer;
        public bool looping = false;

        void Start()
        {
            includeLayer = 1 << LayerMask.NameToLayer("Visible");
            excludeLayer = ~(1 << LayerMask.NameToLayer("Visible"));
        }

        // Update is called once per frame
        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
                {
                    startMousePosition = Input.mousePosition;
                }
                mouseDownTime = Time.time;
            }

            if (Input.GetMouseButton(0))
            {
                Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
                {
                    endMousePosition = Input.mousePosition;
                }
                if (Mathf.Abs(Mathf.Abs(startMousePosition.x) - Mathf.Abs(endMousePosition.x)) > 1f && Mathf.Abs(Mathf.Abs(startMousePosition.y) - Mathf.Abs(endMousePosition.y)) > 1f)
                {
                    drawBox();
                    if (selection == null)
                    {
                        selection = new GameObject("Selection");
                        selectBox = selection.AddComponent<SelectionBox>();
                        selectBox.Initialize(this, player, playerCam, ground, includeLayer, excludeLayer, debugPrefab, startMousePosition, endMousePosition);
                    } else {
                        selectBox.updateMousePositions(startMousePosition, endMousePosition);
                    }
                }
            }

            if (Time.time - mouseDownTime > 0.1f)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    StartCoroutine(selectInsideBox());
                }
            } else {
                if (Input.GetMouseButtonUp(0))
                {
                    if (selectBox != null)
                    selectBox.destroySelectionBox();
                    selection = null;
                    startMousePosition = Vector3.zero;
                    endMousePosition = Vector3.zero;
                }    
            }
            
        }

        void drawBox()
        {
            Vector2 boxStart = startMousePosition;
            Vector2 boxEnd = endMousePosition;
            Vector2 boxCenter = (boxStart + boxEnd) / 2;
            boxGraphic.position = boxCenter;
            Vector2 boxSize = new Vector2(Mathf.Abs(boxStart.x - boxEnd.x), Mathf.Abs(boxStart.y - boxEnd.y));
            boxGraphic.sizeDelta = boxSize;
        }
        private IEnumerator selectInsideBox()
        {
            startMousePosition = Vector3.zero;
            endMousePosition = Vector3.zero;
            drawBox();
            yield return new WaitForSeconds(0.01f);
            if (selectBox != null)
                collisions = selectBox.cachedCollisions;
            Debug.Log(collisions.Count);
            if (collisions != null)
            {
                looping = true;
                //collisions = selectBox.GetColliders();
                foreach (GameObject collision in collisions)
                {
                    if (collision != null)
                    {
                        if (unitTags.Contains(collision.tag))
                        {
                            switch (collision.tag)
                            {
                                case "SquadMember":
                                    SquadMember squadMemberComponent = collision.GetComponent<SquadMember>();
                                    Squad squadComponent = squadMemberComponent.parent;
                                    Selection.squadSelect(squadComponent, selected, player, SelectMode.drag);
                                    break;
                            }
                        }
                    }
                }
            }
            looping = false;
            if (collisions != null)
                collisions.Clear();
            if (selection != null)
                selectBox.destroySelectionBox();
            selection = null;
        }
        // ---- Getters / Setter Methods ----
        public void setPlayer(GameActor newPlayer) { player = newPlayer;}
        public void setPlayerCamera(Camera newCamera) { playerCam = newCamera; }
        public void setSelectionBoxGraphic (RectTransform newGraphic) { boxGraphic = newGraphic; }
        public void setSelectedList(List<GameObject> newSelectedList) { selected = newSelectedList; }
    }
}
