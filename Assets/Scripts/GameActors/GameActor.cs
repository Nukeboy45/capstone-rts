using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capstone
{
    public class GameActor : MonoBehaviour
    {
        public string faction;
        public int ownerTag;
        public int team;
        public List<GameObject> unitList = new List<GameObject>();
        public LayerMask ground;
        public Camera rayCamera;
    }
}
