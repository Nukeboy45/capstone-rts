using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
namespace Capstone
{
    public class OwnedStructure : MonoBehaviour
    {
        [Header("Gameplay Structure Variables - Determined at Runtime")]
        public int team;
        public GameActor owner;
        public bool? showSelect = null;
        public bool selected = false;
        public BuildingType buildingType;

        [Header("Default Structure Variables - shared by all children")]
        [SerializeField] protected FogDetection fogDetection;
        [SerializeField] protected HideObject[] hideObjects;
        [SerializeField] private GameObject sightRadius;
        [SerializeField] protected GameObject iconObj;
        [SerializeField] protected Sprite[] iconSprites;
        [SerializeField] protected Sprite iconSprite;
        [SerializeField] protected float buildTime;
        [SerializeField] protected float health;
        [SerializeField] protected float smallArmsDamageResist;
        [SerializeField] protected float explosiveDamageResist;

        // Runtime Variables
        private bool previousRevealed = false;

        protected virtual void Start()
        {
            StartCoroutine(initializeOwnedStructure());
        }

        protected virtual void Update()
        {
            if (previousRevealed != true)
            {
                if (fogCheck()) 
                {
                    foreach(HideObject script in hideObjects) { script.showObj(); }
                    previousRevealed = true;
                }
            }
        }
        
        private bool fogCheck()
        {
            if (GameManager.Instance.player.team != team)
            {
                if (fogDetection.visible)
                    return true;
                else
                    return false;
            }
            else 
            {
                return true;
            }
        }
        protected IEnumerator initializeOwnedStructure()
        {
            while (owner == null)
                yield return null;

            // Deactivating / Activating FOV sight based on team
            if (owner.team != GameManager.Instance.player.team)
                setSightRadius(false);
            else 
                setSightRadius(true);
        }
        private void setSightRadius(bool enableSight)
        {
            sightRadius.SetActive(enableSight);
        }
    }
}