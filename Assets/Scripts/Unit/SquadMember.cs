using UnityEngine;

namespace Capstone
{
    public class SquadMember : MonoBehaviour
    {
        // Object functionality Variables
        public Squad parent;
        public GameObject modelObject;
        private SquadMember target; // Reference to the enemy SquadMember to be firing at
        public int team;
        public WeaponData weaponData;
        public float maxHealth;
        public float currHealth;
        public float attackSpeed;
        public float range;
        public float defense;
        public float moveSpeed;
        public int modelPriority;


        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if (currHealth <= 0) {
                parent.killModel(modelObject);
            }
        } 


    }
}
