using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capstone 
{
    public class Squad : Unit
    {
        public SquadData squadData;
        public GameObject[] squadMembers;
        private int veterancy;
        private GameObject squadLead; // Reference to the 'primary' member of the squad - determines where the unit icon is rendered, upon death another member
                                       // is assigned to this

        private int squadSize;
        private int aliveMembers; // Tracks current size of the squad to allow for reinforcement and generate spacings 
        public bool? showSelect = null;
        private bool selected = false;
        
        // --------------------- Class Methods ---------------------------------

        public override void Start()
        {
            base.Start();
            InstantiateSquad();
        }

        void Update()
        {
            if (aliveMembers <= 0) {
                Destroy(this.gameObject);
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

        public override void select()
        {
            base.select();
            selected = true;
            showSelectionRadius();
        }

        public override void deselect()
        {
            base.deselect();
            selected = false;
            hideSelectionRadius();
        }

        private void showSelectionRadius()
        {
            foreach (GameObject member in squadMembers)
            {
                SpriteRenderer render = member.GetComponentInChildren<SpriteRenderer>();
                if (render != null)
                {   
                    render.enabled = true;
                }
            }
        }

        private void hideSelectionRadius()
        {
            foreach (GameObject member in squadMembers)
            {
                SpriteRenderer render = member.GetComponentInChildren<SpriteRenderer>();
                if (render != null)
                {   
                    render.enabled = false;
                }
            }
        }

        /// <summary>
        /// Function called by a squadMember object belonging to this squad when its
        /// health reaches 0. Allows the squad to always determine proper number of 
        /// alive members.
        /// </summary>
        /// <param name="modelObject"></param>
        public void killModel(GameObject modelObject) 
        {
            Destroy(modelObject);
            aliveMembers--;
        }

        /// <summary>
        /// Used when spawning a new squad. Generates a new squad at target coordinates with relevant variables
        /// </summary>
        void InstantiateSquad() 
        {
            // Initializing the SquadList, the intended SquadSize, and 
            //"aliveMembers" which is used to track the current alive members of the squad
            squadSize = squadData.models.Length;

            aliveMembers = squadSize;

            squadMembers = new GameObject[squadSize];
            
            Dictionary<int, Vector3> spacing = generateSpacing(transform.position);

            // Setting the squad's icon based on whether or not it belongs to player's team
            Player playerObj = FindObjectOfType<Player>();
            if (owner.team != playerObj.team) 
            {
                squadData.icon = squadData.icons[1];
            } else {
                squadData.icon = squadData.icons[0];
            }

            // Spawn loop for initializing each model of squad according to 
            // the associated model prefabs
            for (int i = 0; i < squadSize; i++)
            {   
                // Spawns the prefab and ties the reference to the squadMembers list
                squadMembers[i] = Instantiate(squadData.models[i], spacing[i], Quaternion.identity);

                // Retrieves the squadMember script reference from the newly instantiated
                // infantry model
                SquadMember squadMemberComponent = squadMembers[i].GetComponent<SquadMember>();

                // Sets the parent reference on the model to the current squad object
                squadMemberComponent.parent = this;

                // First model to spawn becomes the default "squadLead" - 
                // squadLead models do not get any bonuses, their coordinates are used to 
                // render the squadIcon. Upon the death of a squadLead, 
                // the reference will automatically transfer to another squadMember object
                if (i == 0 & squadLead == null) {
                    squadLead = squadMembers[i];
                }

                // Sets the squad's transformation as parent - not really relevant since
                // movement code is always separate, but makes it look cleaner in the
                // editor
                squadMembers[i].transform.SetParent(this.transform, false);
            }

            // Initializes the squad Icon and attaches it to the icon position empty object on the model.
            GameObject iconPosition = squadLead.transform.Find("iconPos").gameObject;
            if (iconPosition != null) {
                iconPosition.SetActive(true);
                
                GameObject iconObject = Instantiate(squadData.iconObj, new Vector3(0,0,0), Quaternion.identity);
                iconObject.transform.SetParent(iconPosition.transform, false);
                SpriteRenderer renderer = iconObject.GetComponent<SpriteRenderer>();
                if (renderer != null) 
                {
                    renderer.sprite = squadData.icon;
                }
                iconObject.layer = LayerMask.NameToLayer("Selectable");
            } else {
                Debug.Log("Icon Position Object not found!");
            }

            // Setting the render color of the selection circles
            if (owner.team != playerObj.team)
            {
                foreach(GameObject member in squadMembers)
                {
                    SpriteRenderer render = member.GetComponentInChildren<SpriteRenderer>();
                    if (render != null)
                    {
                    render.color = Color.red;
                    render.enabled = false;
                    }
                }
            } else {
                foreach(GameObject member in squadMembers)
                {
                    SpriteRenderer render = member.GetComponentInChildren<SpriteRenderer>();
                    if (render != null)
                    {
                        render.color = Color.blue;
                        render.enabled = false;
                    }
                }    
            }
    
        }

        /// <summary>
        /// Assuming a squad has not taken cover, generates a spacing based on number of alive squad members upon spawning and every subsequent move command.
        /// Takes a Vector3 value, target position to generate the new values
        /// </summary>
        /// <returns> Spacing dictionary to be used in squad positioning </returns>
        private Dictionary<int, Vector3> generateSpacing(Vector3 targetPosition)
        {
            Dictionary<int, Vector3> spacingDictionary = new Dictionary<int, Vector3>();
            switch (aliveMembers)
            {
                case 1:
                    spacingDictionary[0] = targetPosition;
                    break;
                
                case 2:
                    spacingDictionary[0] = targetPosition;
                    spacingDictionary[1] = targetPosition - new Vector3(2,0,0);
                    break;

                case 3:
                    spacingDictionary[0] = targetPosition;
                    spacingDictionary[1] = targetPosition - new Vector3(2,0,2);
                    spacingDictionary[2] = targetPosition + new Vector3(2,0,2);
                    break;

                case 4:
                    spacingDictionary[0] = targetPosition;
                    spacingDictionary[1] = targetPosition - new Vector3(2,0,2);
                    spacingDictionary[2] = targetPosition + new Vector3(4,0,-2);
                    spacingDictionary[3] = targetPosition + new Vector3(2,0,0);
                    break;

                case 5:
                    spacingDictionary[0] = targetPosition;
                    spacingDictionary[1] = targetPosition - new Vector3(2,0,2);
                    spacingDictionary[2] = targetPosition + new Vector3(4,0,-2);
                    spacingDictionary[3] = targetPosition + new Vector3(2,0,0);
                    spacingDictionary[4] = targetPosition + new Vector3(1,0,-2);
                    break;

                case 6: 
                    spacingDictionary[0] = targetPosition;
                    spacingDictionary[1] = targetPosition - new Vector3(2,0,2);
                    spacingDictionary[2] = targetPosition + new Vector3(4,0,-2);
                    spacingDictionary[3] = targetPosition + new Vector3(2,0,0);
                    spacingDictionary[4] = targetPosition + new Vector3(1,0,-2);
                    spacingDictionary[5] = targetPosition + new Vector3(1,0,-4);
                    break;
            }
            return spacingDictionary;
        }

    }
}

