using UnityEngine;
using UnityEngine.AI;

namespace Capstone
{
    public class SquadMember : MonoBehaviour
    {
        // Object functionality Variables
        public Squad parent;
        private SquadMember target; // Reference to the enemy SquadMember to be firing at
        private float currHealth;
        public WeaponData weaponData;
        public float maxHealth;
        public float attackSpeed;
        public float range;
        public float defense;
        public float moveSpeed;
        public int modelPriority;
        public LayerMask ground;
        NavMeshAgent myAgent;



        // Start is called before the first frame update
        void Start()
        {
            currHealth = maxHealth;
            myAgent = GetComponent<NavMeshAgent>();
        }

        public void moveToPosition(Vector3 point, float speed)
        {
            myAgent.speed = speed;
            myAgent.SetDestination(point);
        }

        // Update is called once per frame
        void Update()
        {
            if (currHealth <= 0) {
                parent.killModel(this.gameObject);
            }
        } 

        // ------ Getters / Setters
        public float getCurrentHealth()
        {
            return currHealth;
        }

        public void takeDamage(float damage)
        {
            currHealth -= damage;
            parent.updateUIHealth();
            Debug.Log("Current health: " + currHealth.ToString());
        }
    }
}
