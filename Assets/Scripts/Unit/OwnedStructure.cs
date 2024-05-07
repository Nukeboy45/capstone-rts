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
        public bool currentlyRevealed = false;
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

        /// <summary>
        /// ** For players only ** converts the base GameActor component to a player component
        /// to add the desired unit into the selected list.
        /// </summary>
        public virtual void select()
        {
            if (owner is Player) 
            {
                Player playerComp = (Player)owner;
                playerComp.getSelected().Add(gameObject);
            }
            selected = true;
        }

        /// <summary>
        /// ** For players only ** converts the base GameActor component to a player component
        /// to remove the desired unit from the selected list.
        /// </summary>
        public virtual void deselect()
        {
            if (owner is Player)
            {
                Player playerComp = (Player)owner;
                playerComp.getSelected().Remove(gameObject);              
            }
            selected = false;
        }

        protected virtual void Update()
        {
            bool inFog = fogCheck();
            if (previousRevealed != true)
            {
                if (inFog) 
                {
                    foreach(HideObject script in hideObjects) { script.showObj(); }
                    previousRevealed = true;
                }
            } 

            if (showSelect != null && selected == false)
            {
                if (showSelect == true)
                {
                    showSelectionRadius();
                    showSelect = false;
                } else {
                    hideSelectionRadius();
                    showSelect = false;
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
            {
                setSightRadius(false);
                SpriteRenderer renderer = gameObject.GetComponentInChildren<SpriteRenderer>();
                renderer.color = Color.red;
                renderer.enabled = false;
            }
            else 
            {
                setSightRadius(true);
                SpriteRenderer renderer = gameObject.GetComponentInChildren<SpriteRenderer>();
                renderer.color = Color.blue;
                renderer.enabled = false;
            }
        }
        private void setSightRadius(bool enableSight)
        {
            sightRadius.SetActive(enableSight);
        }

        protected void showSelectionRadius()
        {
            SpriteRenderer renderer = gameObject.GetComponentInChildren<SpriteRenderer>();
            renderer.enabled = true;
        }

        protected void hideSelectionRadius()
        {
            SpriteRenderer renderer = gameObject.GetComponentInChildren<SpriteRenderer>();
            renderer.enabled = false;
        }
    }
}