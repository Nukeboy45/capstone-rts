using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

namespace Capstone {
    public class CapturePoint : MonoBehaviour
    {
        public SphereCollider pointCollider;
        public GameObject flag;
        public LayerMask layerMask;
        public Material[] flagTextures = new Material[3]; // 0 is neutral, 1 is aus, 2 is ita
        public List<GameObject> insidePoint = new List<GameObject>();
        public float captureValue = 0.0f;
        public int owner = 0;
        private bool checking = false;

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            checkOwnershipChange();

            Collider[] collisions = Physics.OverlapSphere(pointCollider.transform.position + pointCollider.center, pointCollider.radius, layerMask);

            foreach (Collider collider in collisions)
            {
                GameObject obj = collider.GetComponent<Collider>().gameObject;
                SquadMember component = obj.GetComponent<SquadMember>();

                if (component != null)
                {
                    if (!insidePoint.Contains(obj))
                    {
                        insidePoint.Add(obj);
                    }
                }
            }

            if (!checking)
            {
                checking = true;
                removeInvalidObjects(collisions);
            }

            if (insidePoint.Count > 0)
            {
                capturePoint();
            } else {
                neutralizePoint();
            }
        }

        private void removeInvalidObjects(Collider[] colliders)
        {
            List<GameObject> removeObjs = new List<GameObject>();
            foreach (GameObject captureObj in insidePoint)
            {
                bool inCollisions = false;
                foreach (Collider collider in colliders)
                {
                    GameObject obj = collider.GetComponent<Collider>().gameObject;
                    SquadMember component = obj.GetComponent<SquadMember>();

                    if (component != null)
                    {
                        if (obj == captureObj)
                        {
                            inCollisions = true;
                        }
                    }
                }
                if (!inCollisions) 
                {
                    removeObjs.Add(captureObj);
                }
            }
            
            foreach (GameObject removeObj in removeObjs)
            {
                insidePoint.Remove(removeObj);
            }
            checking = false;
        }

        private void capturePoint()
        {
            List<int> captureTeams = new List<int>();

            foreach (GameObject obj in insidePoint)
            {
                SquadMember component = obj.GetComponent<SquadMember>();
                int team = component.parent.team + 1;
                if (!captureTeams.Contains(team))
                {
                    captureTeams.Add(team);
                }
            }

            if (captureTeams.Count == 1)
            {
                switch (captureTeams[0])
                {
                    case 1:
                        if (captureValue <= (100.0f + 0.2f))
                        {
                            captureValue += 0.2f;
                            updateFlagPosition();
                        }
                        break;
                    
                    case 2:
                        if (captureValue >= (-100.0f - 0.2f))
                        {
                            captureValue -= 0.2f;
                            updateFlagPosition();
                        }
                        break;
                }
            } 
        }

        private void neutralizePoint()
        {
            switch (owner) {
                case 0:
                    if (captureValue >= 1.0f)
                    {
                        captureValue -= 0.2f;
                        updateFlagPosition(); 
                    } else if (captureValue <= -1.0f)
                    {
                        captureValue += 0.2f;
                        updateFlagPosition();
                    }
                    break;
                case 1:
                    if (captureValue <= (100f + 0.2f))
                    {
                        captureValue += 0.2f;
                        updateFlagPosition();
                    }
                    break;
                case 2:
                    if (captureValue >= (-100.0f - 0.2f))
                    {
                        captureValue -= 0.2f;
                        updateFlagPosition();
                    }
                    break;
                }
        }

        /// <summary>
        /// 
        /// </summary>
        private void updateFlagPosition() {
            Vector3 flagPosition = flag.transform.position;
            if (flagPosition.y < 5.4)
            {
                flagPosition.y = 4.2f * (Mathf.Abs(captureValue) / 100.0f) + 1.2f; 
            }
            flag.transform.position = flagPosition;
        }

        /// <summary>
        /// 
        /// </summary>
        private void checkOwnershipChange()
        {
            if (captureValue >= 100.0f && owner != 1)
            {
                updateOwner(1);
            } else if (captureValue <= -100.0f && owner != 2) {
                updateOwner(2);
            } else if ((captureValue >= -1.0f && captureValue <= 1.0f) && owner != 0) {
                updateOwner(0);
            }
        }
        private void updateOwner(int newOwner) {
            Renderer flagRender = flag.GetComponent<Renderer>();
            owner = newOwner;
            if (flagRender != null) 
            {
                switch (owner) {
                    case 0:
                        flagRender.material = flagTextures[0];
                        break;
                    case 1:
                        flagRender.material = flagTextures[1];
                        break;
                    case 2:
                        flagRender.material = flagTextures[2];
                        break;
                }
            }
        }
    }
}
