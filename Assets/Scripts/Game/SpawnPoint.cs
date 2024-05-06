using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;


namespace Capstone {
    public class SpawnPoint : MonoBehaviour
    {
        public GameObject cameraSpawnPos;

        // -------- Getters / Setters ---------------

        public Vector3 getCameraSpawnPosition() { return cameraSpawnPos.transform.position; }
    }
}
