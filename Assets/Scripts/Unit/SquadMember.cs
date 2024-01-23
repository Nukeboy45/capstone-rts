using UnityEngine;

namespace Capstone
{
    public class SquadMember : MonoBehaviour
    {
        // Object functionality Variables
        public Squad parent;
        public GameObject modelObj; // Reference to GameObject this instance is attached to
        private SquadMember target; // Reference to the enemy SquadMember to be firing at
        private float currHealth;
        public WeaponData weaponData;
        public float maxHealth;
        public float attackSpeed;
        public float range;
        public float defense;
        public float moveSpeed;
        public int modelPriority;


        // Start is called before the first frame update
        void Start()
        {
            
        }

        void InstantiateSquadMember() {

        }

        // Update is called once per frame
        void Update()
        {
            if (currHealth <= 0) {
                //parent.killModel(modelObj);
            }
        } 


    }
}
