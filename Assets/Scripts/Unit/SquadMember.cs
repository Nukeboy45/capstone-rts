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
        [SerializeField] private float range;
        [SerializeField] private float defense;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float modelAccuracyModifier = 1.0f;// 1.0 is base accuracy modifier, 1.1 10% more accurate, etc.
        [SerializeField] private float attackSpeedModifier = 1.0f; // 1.0 is base attack speed, 1.1 10% faster, etc
        [SerializeField] private int modelPriority;
        [SerializeField] private NavMeshAgent myAgent;
        [SerializeField] private GameObject sightRadius;
        [SerializeField] private SphereCollider rangeCollider;
        [SerializeField] private WeaponAudio weaponAudio;
        [SerializeField] private ParticleSystem muzzleFlash;
        [SerializeField] private ParticleSystem muzzleSmoke;



        // Private Runtime Variables
        [SerializeField] private GameObject target = null;
        private TargetType targetType;

        // Shooting Variables
        private Coroutine firingLoop = null;
        private float weaponDamage;
        private float weaponAccuracy;
        private float weaponAccuracyFalloffFactor;
        private float weaponAccuracyFalloffDistance;
        private float highestWeaponAccuracyRange;
        private DateTime lastFiringTick = DateTime.MinValue;
        private float firingCooldown;
        private DateTime lastReloadTick;
        private DateTime lastAimTick = DateTime.MinValue;
        private float minAimTime;
        private float maxAimTime;
        private float currentAimTime;
        private bool reloading = false;
        private float reloadTime;
        [SerializeField] private int ammo;
        private int ammoCapacity;

        // Debug Variables
        //[SerializeField] private Collider[] targetCollisions;

        // -------- Utility Variables -----------

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
                StopCoroutine(firingLoop);
                parent.killModel(gameObject);
            }

            checkForTarget();

            if (target != null && firingLoop == null)
            {
                firingLoop = StartCoroutine(attackLoop());
            }
        } 

        private void checkForTarget()
        {
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
                        targetType = TargetType.infantry;
                        target = obj;
                    }
                }
                //GameObject obj = collider.GetComponent<Collider>().gameObject;
                //SquadMember component = obj.GetComponent<SquadMember>();
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
                    if (collider.gameObject.CompareTag("SquadMember") && collider.gameObject == target)
                    {
                        targetInRange = true;
                        break;
                    }
                    targetInRange = false;
                }
                
                if (!targetInRange)
                {
                    target = null;
                    firingLoop = null;
                    yield break;
                }

                float targetAngleDif = unitFunctions.get2DAngleDifference(transform, target.transform);
                if (myAgent.remainingDistance <= 0.1f)
                {
                    if (Mathf.Abs(targetAngleDif) >= 1.5f)
                    {
                        if (targetAngleDif >= 1.0f)
                        {
                            transform.Rotate(0f, -180f * Time.deltaTime , 0f, Space.Self);
                        } else if (targetAngleDif < -1.0) {
                            
                            transform.Rotate(0f, 180f * Time.deltaTime, 0f, Space.Self);
                        }
                    }
                }

                if (reloading)
                {
                    TimeSpan timeSinceReload = DateTime.Now - lastReloadTick;
                    if (timeSinceReload.TotalSeconds >= reloadTime)
                    {
                        reloading = false;
                        ammo = ammoCapacity;
                    }
                } else {
                    if (Mathf.Abs(targetAngleDif) <= 45.0f)
                    {
                        if (lastFiringTick == DateTime.MinValue)
                        {
                            if (lastAimTick == DateTime.MinValue)
                                lastAimTick = DateTime.Now;
                            else 
                            {
                                TimeSpan timeSinceAim = DateTime.Now - lastAimTick;
                                if (timeSinceAim.TotalSeconds >= currentAimTime)
                                {
                                    shootAtTarget();
                                    lastFiringTick = DateTime.Now;
                                }
                            }
                        } else {
                            TimeSpan timeSinceLastShot = DateTime.Now - lastFiringTick;
                            if (timeSinceLastShot.TotalSeconds >= firingCooldown && !reloading)
                            {
                                if (lastAimTick == DateTime.MinValue)
                                    lastAimTick = DateTime.Now;
                                else 
                                {                        
                                    TimeSpan timeSinceAim = DateTime.Now - lastAimTick;
                                    if (timeSinceAim.TotalSeconds >= currentAimTime)
                                    {
                                        shootAtTarget();
                                        lastFiringTick = DateTime.Now;
                                    }
                                }
                            }
                        }
                    }   
                }
                yield return null;
            }
            checkForTarget();
            firingLoop = null;
            yield break;
        }

        private void shootAtTarget()
        {
            currentAimTime = UnityEngine.Random.Range(minAimTime, maxAimTime);
            lastAimTick = DateTime.MinValue;
            weaponAudio.PlaySound(0);
            muzzleFlash.Play();
            muzzleSmoke.Play();
            switch (targetType)
            {
                case TargetType.infantry:
                    float distance = Vector3.Distance(transform.position, target.transform.position);
                    SquadMember component = target.GetComponent<SquadMember>();
                    float accuracyRoll = UnityEngine.Random.Range(0f, 1f);
                    float scaledAccuracy = accuracyWithDistance(distance);
                    if (scaledAccuracy >= accuracyRoll)
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

        private float accuracyWithDistance(float distance)
        {
            //Debug.Log("distance " + distance);
            //Debug.Log("optimal dist " + highestWeaponAccuracyRange);
            if (distance <  highestWeaponAccuracyRange && distance > 0)
            {
                float scaledAccuracy = weaponAccuracy * modelAccuracyModifier;
                return scaledAccuracy;
            }
            else 
            {
                float fallOffDistance = distance - highestWeaponAccuracyRange;
                float scaledAccuracy = weaponAccuracy * Mathf.Pow(weaponAccuracyFalloffFactor, fallOffDistance / weaponAccuracyFalloffDistance) * modelAccuracyModifier;

                //Debug.Log("Scaled accuracy: " + scaledAccuracy);
                return scaledAccuracy;
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
            weaponAccuracy = weaponData.MaxAccuracy;
            weaponAccuracyFalloffFactor = weaponData.AccuracyFalloffFactor;
            weaponAccuracyFalloffDistance = weaponData.AccuracyFalloffDistance;
            highestWeaponAccuracyRange = weaponData.MaxAccuracyRange;
            weaponAudio.setWeaponSounds(weaponData.weaponSounds);
            minAimTime = weaponData.MinAimTime;
            maxAimTime = weaponData.MaxAimTime;
        }

        // ------ Getters / Setters
        public float getRemainingDistance() { return myAgent.remainingDistance; }
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
