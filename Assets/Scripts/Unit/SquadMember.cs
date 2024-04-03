using System;
using System.Collections;
using System.Linq;
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
        [SerializeField] private SphereCollider rangeCollider;


        // Private Runtime Variables
        [SerializeField] private GameObject target = null;
        private TargetType targetType;

        // Shooting Variables
        [SerializeField] private Coroutine firingLoop = null;
        [SerializeField] private float weaponDamage;
        [SerializeField] private float weaponAccuracy;
        [SerializeField]private DateTime lastFiringTick = DateTime.MinValue;
        [SerializeField]private float firingCooldown;
        [SerializeField]private DateTime lastReloadTick;
        [SerializeField]private bool reloading = false;
        [SerializeField]private float reloadTime;
        [SerializeField]private int ammo;
        [SerializeField] private int ammoCapacity;

        // Debug Variables
        //[SerializeField] private Collider[] targetCollisions;

        // Start is called before the first frame update
        void Start()
        {
            currHealth = maxHealth;
            myAgent = GetComponent<NavMeshAgent>();
            UpdateWeaponVariables();

        }
        // Update is called once per frame
        void Update()
        {
            if (currHealth <= 0) {
                parent.killModel(this.gameObject);
            }

            Collider[] collisions = Physics.OverlapSphere(rangeCollider.transform.position + rangeCollider.center, rangeCollider.radius);
            //targetCollisions = collisions;

            foreach (Collider collider in collisions)
            {
                if (collider.gameObject.CompareTag("SquadMember"))
                {
                    GameObject obj = collider.GetComponent<Collider>().gameObject;
                    SquadMember component = obj.GetComponent<SquadMember>();
                    if (obj != null && target == null && component.parent.team != parent.team)
                    {
                        Debug.Log("squad member target found");
                        targetType = TargetType.infantry;
                        target = obj;
                    }
                }
                //GameObject obj = collider.GetComponent<Collider>().gameObject;
                //SquadMember component = obj.GetComponent<SquadMember>();
            }

            if (target != null && firingLoop == null)
            {
                firingLoop = StartCoroutine(attackLoop());
            }
        } 

        private IEnumerator attackLoop()
        {
            bool targetInRange = false;
            while (target != null)
            {
                Collider[] collisions = Physics.OverlapSphere(rangeCollider.transform.position + rangeCollider.center, rangeCollider.radius);
                foreach (Collider collider in collisions)
                {
                    if (collider.gameObject == target)
                    {
                        targetInRange = true;
                        break;
                    }
                }

                if (!targetInRange)
                {
                    target = null;
                    firingLoop = null;
                    yield break;
                }

                if (reloading)
                {
                    TimeSpan timeSinceReload = DateTime.Now - lastReloadTick;
                    if (timeSinceReload.Seconds >= reloadTime)
                    {
                        reloading = false;
                        ammo = ammoCapacity;
                    }
                }

                if (!reloading)
                {
                    if (lastFiringTick == DateTime.MinValue)
                    {
                        shootAtTarget();
                        lastFiringTick = DateTime.Now;
                    } else {
                        TimeSpan timeSinceLastShot = DateTime.Now - lastFiringTick;
                        if (timeSinceLastShot.Seconds >= firingCooldown && !reloading)
                        {
                            shootAtTarget();
                            lastFiringTick = DateTime.Now;
                        }
                    }   
                }
                yield return new WaitForSecondsRealtime(0.15f);
            }
        }

        private void shootAtTarget()
        {
            switch (targetType)
            {
                case TargetType.infantry:
                    SquadMember component = target.GetComponent<SquadMember>();
                    float accuracyRoll = UnityEngine.Random.Range(0f, 1f);
                    if (accuracyRoll >= weaponAccuracy)
                        component.takeHit(weaponDamage);
                    break;
            }
            ammo-=1;
            if (ammo == 0)
            {
                lastFiringTick = DateTime.MinValue;
                reloading = true;
                lastReloadTick = DateTime.Now;
            }
        }

        public void takeHit(float weaponDamage)
        {
            currHealth -= weaponDamage;
            parent.updateUIHealth();
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

        private void UpdateWeaponVariables()
        {
            ammoCapacity = weaponData.MagazineCapacity;
            ammo = weaponData.MagazineCapacity;
            reloadTime = weaponData.ReloadTime;
            firingCooldown = weaponData.FiringCooldown;
            weaponDamage = weaponData.Damage;
            weaponAccuracy = weaponData.Accuracy;
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
