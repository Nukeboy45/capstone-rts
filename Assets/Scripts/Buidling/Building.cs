using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capstone {

    public class Building : MonoBehaviour
    {
        public GameObject buildingObj;
        public float maxBuildingHealth;
        public int team;
        public GameActor owner;
        private float currHealth;
        public bool? showSelect = null;
        private bool selected = false;

        public void setCurrentHealth(float health) {
            currHealth = health;
        }

        public float getCurrentHealth() {
            return currHealth;
        }

        /// <summary>
        /// ** For players only ** converts the base GameActor component to a player component
        /// to add the desired building into the selected list.
        /// </summary>
        public virtual void select()
        {
            if (owner is Player) 
            {
                Player playerComp = (Player)owner;
                playerComp.selected.Add(this.gameObject);
            }
        }

        public virtual void deselect()
        {
            if (owner is Player)
            {
                Player playerComp = (Player)owner;
                playerComp.selected.Remove(this.gameObject);              
            }
        }


    }
    
}