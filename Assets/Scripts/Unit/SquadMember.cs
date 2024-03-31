using UnityEngine;
using UnityEngine.AI;

namespace Capstone
{
    public class SquadMember : MonoBehaviour
    {
        // Object functionality Variables
        public Squad parent;

        // Private, Editor-Accessible Variables
        [SerializeField] private WeaponData weaponData;
        [SerializeField] private float maxHealth;
        [SerializeField] private float currHealth;    
        [SerializeField] private float attackSpeed;
        [SerializeField] private float range;
        [SerializeField] private float defense;
        [SerializeField] private float moveSpeed;
        [SerializeField] private int modelPriority;
        [SerializeField] private NavMeshAgent myAgent;
        [SerializeField] private GameObject sightRadius;


        // Private Runtime Variables
        private GameObject target; 
        private TargetType targetType;



        // Start is called before the first frame update
        void Start()
        {
            currHealth = maxHealth;
            myAgent = GetComponent<NavMeshAgent>();
        }

        // Update is called once per frame
        void Update()
        {
            if (currHealth <= 0) {
                parent.killModel(this.gameObject);
            }
        } 

        public void moveToPosition(Vector3 point, float speed)
        {
            myAgent.speed = speed;
            myAgent.SetDestination(point);
        }

        public void setSightRadiusStatus(bool status)
        {
            if (status)
            {
                sightRadius.SetActive(true);
            } else {
                sightRadius.SetActive(false);
            }
        }

        // ------ Getters / Setters
        public float getCurrentHealth()
        {
            return currHealth;
        }

        public float getMoveSpeed()
        {
            return moveSpeed;
        }

        public float getMaxHealth()
        {
            return maxHealth;
        }

        public void takeDamage(float damage)
        {
            currHealth -= damage;
            parent.updateUIHealth();
        }
    }
}
