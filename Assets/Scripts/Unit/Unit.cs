using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Capstone 
{
    public class Unit : MonoBehaviour
    {
        // Public Variables
        public int team;
        public GameActor owner;
        public bool? showSelect = null;
        public bool selected = false;

        // Private / Protected, Editor-Accessible Variables
        [SerializeField] protected GameObject iconObj;
        [SerializeField] protected Sprite[] iconSprites;
        [SerializeField] protected Sprite portrait;
        [SerializeField] protected float buildTime;
        

        // Private / Protected Runtime Variables for Child Classes
        protected Sprite iconSprite;
        protected UnitIconUI uiIconComponent;
        protected UnitIconUIWorld worldIconComponent;
        protected GameObject worldIconObj;
        protected bool revealed = false;
        protected bool revealStatus = true;

        /// ---------- Generic Methods to Inherit ---------------
        
        public virtual void Start()
        {
            owner.unitList.Add(this.gameObject);
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
                playerComp.selected.Add(this.gameObject);
            }
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
                playerComp.selected.Remove(this.gameObject);              
            }
        }

        public virtual void moveTo(List<RaycastHit> hit) {}

        public virtual bool checkReveal() {return false;}

        // --- Getters / Setter Methods ---

        public float getBuildTime() { return buildTime; }

        public Sprite getIcon(int index) { return iconSprites[index]; }

        public Sprite getPortrait() { return portrait; }
        public bool getRevealedIcon() { return revealStatus; }

        public void setRevealedIcon(bool value) { revealStatus = value; }
    }
}