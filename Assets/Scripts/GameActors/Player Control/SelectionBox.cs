using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace Capstone {
    public class SelectionBox : MonoBehaviour
    {
        public List<GameObject> collisions = new List<GameObject>();
        public List<GameObject> cachedCollisions = new List<GameObject>();
        private Camera playerCam;
        private GameActor player;
        private DragSelect parentController;
        private Mesh selectionMesh;    
        private MeshFilter selectionFilter;
        private MeshCollider selectionCollider;
        private LayerMask ground;
        private LayerMask includeLayer;
        private LayerMask excludeLayer;
        private GameObject debugPrefab;
        private GameObject[] debugMarkers = new GameObject[4];
        private Vector3 startMousePosition;
        private Vector3 endMousePosition;
        private Vector3 prevEndMousePosition;
        private Coroutine visualLoop;
        private List<String> unitTags = new List<String>{"SquadMember"};
        public void Initialize(DragSelect parent, GameActor owner, Camera newCamera, LayerMask newGround, LayerMask newIncludeLayer, LayerMask newExcludeLayer, GameObject newDebugPrefab, Vector3 startPosition, Vector3 endPosition)
        {
            parentController = parent;
            player = owner;
            playerCam = newCamera;
            ground = newGround;
            includeLayer = newIncludeLayer;
            excludeLayer = newExcludeLayer;
            debugPrefab = newDebugPrefab;
            startMousePosition = startPosition;
            endMousePosition = endPosition;
            Rigidbody body = gameObject.AddComponent<Rigidbody>();
            body.isKinematic = true;
            body.useGravity = false;
            selectionFilter = gameObject.AddComponent<MeshFilter>();
            selectionCollider = gameObject.AddComponent<MeshCollider>();
            selectionCollider.includeLayers = includeLayer;
            selectionCollider.excludeLayers = excludeLayer;
            selectionCollider.convex = true;
            selectionCollider.isTrigger = true;
            selectionMesh = new Mesh();
            drawSelectionMesh();
        }

        public void updateMousePositions(Vector3 newStartPosition, Vector3 newEndPosition)
        {
            startMousePosition = newStartPosition;
            endMousePosition = newEndPosition;
        }
        void OnTriggerStay(Collider other)
        {
            if (!collisions.Contains(other.gameObject))
            {
                collisions.Add(other.gameObject);
                setCacheCollisions();
            }
            if (unitTags.Contains(other.gameObject.tag))
            {
                switch (other.gameObject.tag)
                {
                    case "SquadMember":
                        SquadMember squadMemberComponent = other.gameObject.GetComponent<SquadMember>();
                        Squad squadComponent = squadMemberComponent.parent;
                        if (squadComponent.owner == player)
                        {
                            squadComponent.multiSelect = true;
                            squadComponent.multiSelectTime = Time.time;
                        }
                        break;
                }
            }
        }

        private void Update()
        {
            if (parentController != null)
            {
                if (!drawing && endMousePosition != prevEndMousePosition && Vector3.Distance(prevEndMousePosition, endMousePosition) > 10f)
                {
                    prevEndMousePosition = endMousePosition;
                    drawSelectionMesh();
                }
            }
        }

        private Vector3[] prevVertices;
        private bool drawing;
        private bool copying = false;
        private float lastClear;
        public void drawSelectionMesh()
        {
            setCacheCollisions();
            while (copying)
            {}
            collisions.Clear();
            lastClear = Time.time;
            drawing = true;
            Vector3 p1 = Vector3.zero;
            Vector3 p2 = Vector3.zero;
            Vector3 p3 = Vector3.zero;
            Vector3 p4 = Vector3.zero;
            if (startMousePosition != endMousePosition)
            {

                if (startMousePosition.x < endMousePosition.x)
                {
                    if (startMousePosition.y > endMousePosition.y)
                    {
                        p1 = startMousePosition;
                        p2 = new Vector3(startMousePosition.x, endMousePosition.y, 0f);
                        p3 = new Vector3(endMousePosition.x, startMousePosition.y, 0f);
                        p4 = endMousePosition;
                    } else {
                        p1 = new Vector3(startMousePosition.x, endMousePosition.y, 0f);
                        p2 = startMousePosition;
                        p3 = endMousePosition;
                        p4 = new Vector3(endMousePosition.x, startMousePosition.y, 0f);
                    }
                } else {
                    if (endMousePosition.y > startMousePosition.y)
                    {
                        p1 = new Vector3(endMousePosition.x, startMousePosition.y, 0f);
                        p2 = startMousePosition;
                        p3 = endMousePosition;
                        p4 = new Vector3(startMousePosition.x, endMousePosition.y, 0f); 
                    } else {
                        p1 = endMousePosition;
                        p2 = new Vector3(startMousePosition.x, endMousePosition.y, 0f);
                        p3 = new Vector3(endMousePosition.x, startMousePosition.y, 0f);
                        p4 = startMousePosition;
                    }
                }
                Vector3[] customCorners = new Vector3[8];
                customCorners[0] = p1;
                customCorners[1] = p2;
                customCorners[2] = p3;
                customCorners[3] = p4;
                for (int i=0; i<4; i++)
                {
                    // if (debugMarkers[i] != null)
                    //     Destroy(debugMarkers[i]);
                    Ray ray = playerCam.ScreenPointToRay(customCorners[i]);

                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
                    {
                        customCorners[i] = hit.point;
                        // debugMarkers[i] = Instantiate(debugPrefab, hit.point, Quaternion.identity);
                        customCorners[i+4] = new Vector3(hit.point.x, hit.point.y + 10f, hit.point.z);
                    }
                }

                int[] meshTriangles = {
                    0, 1, 2,
                    2, 1, 3,
                    4, 6, 0,
                    0, 6, 2,
                    6, 7, 2,
                    2, 7, 3,
                    7, 5, 3,
                    3, 5, 1,
                    4, 5, 0,
                    0, 5, 1,
                    4, 5, 6,
                    6, 5, 7
                };

                selectionMesh.Clear();
                if (customCorners.Length == 8)
                {
                    selectionFilter.mesh = selectionMesh;
                    selectionMesh.vertices = customCorners;
                    selectionMesh.triangles = meshTriangles;
                    prevVertices = customCorners;
                    selectionFilter.mesh.RecalculateNormals();
                    selectionFilter.sharedMesh = selectionFilter.mesh;
                    selectionCollider.sharedMesh = selectionFilter.sharedMesh;
                } else {
                    selectionFilter.mesh = selectionMesh;
                    selectionMesh.vertices = prevVertices;
                    selectionMesh.triangles = meshTriangles;
                    selectionFilter.mesh.RecalculateNormals();
                    selectionFilter.sharedMesh = selectionFilter.mesh;
                    selectionCollider.sharedMesh = selectionFilter.sharedMesh;
                }
            }
            drawing = false;
        }
        public void setCacheCollisions() { 
            copying=true;
            cachedCollisions = new List<GameObject>(collisions);
            copying=false;
        }
        public void clearColliders() { collisions = new List<GameObject>(); }
        public void destroySelectionBox()
        {
            // foreach (GameObject marker in debugMarkers)
            // {
            //     if (marker != null)
            //         Destroy(marker);
            // }
            Destroy(gameObject);
        }
    }
}
