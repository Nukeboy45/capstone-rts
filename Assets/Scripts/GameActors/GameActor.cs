using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capstone
{
    public class GameActor : MonoBehaviour
    {
        // Identity Variables
        public FactionType faction;
        public int ownerTag;
        public int team;
        public List<GameObject> unitList = new List<GameObject>();

        // Utility Variables for interaction with world space
        public LayerMask clickable;
        public LayerMask ground;
        public Camera rayCamera;
        public ProductionStructure headquarters;
        public GameObject retreatPoint;

        // Resource and Gameplay variables
        public int firewood;
        public int techTier = 0;
    }
}
