using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEngine.Timeline;

namespace Capstone 
{
    public class squadMoveMarkerPool : MonoBehaviour
    {
        public static squadMoveMarkerPool sharedInstance;
        public List<GameObject> moveMarkers;
        public GameObject moveMarkerPrefab;
        public int maxMarkers;
        public bool markersActive = false;

        void Awake()
        {
            sharedInstance = this;
        }

        void Start()
        {
            moveMarkers = new List<GameObject>();
            GameObject temp;
            for (int i = 0; i < maxMarkers; i++)
            {
                temp = Instantiate(moveMarkerPrefab);
                temp.SetActive(false);
                moveMarkers.Add(temp);
            }
        }

        public void showMoveMarkers(List<RaycastHit> hits)
        {
            for (int i = 0; i < hits.Count; i++)
            {
                moveMarkers[i].transform.position = hits[i].point + new Vector3(0.0f, 0.1f, 0.0f);
                moveMarkers[i].SetActive(true);
            }
            markersActive = true;
        }

        public void hideMoveMarkers()
        {
            foreach (GameObject marker in moveMarkers)
            {
                if (marker.activeSelf == true)
                {
                    marker.SetActive(false);
                }
            }
            markersActive = false;
        }
    }

}