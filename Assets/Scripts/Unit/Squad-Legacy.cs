/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capstone 
{
    public class Squad : Unit
    {
        public SquadData squadData;
        private SquadMember[] squadMembers;
        private int veterancy;
        private GameObject squadLead; // Reference to the 'primary' member of the squad - determines where the unit icon is rendered, upon death another member
                                       // is assigned to this

        private int squadSize;
        private int aliveMembers; // Tracks current size of the squad to allow for reinforcement and generate spacings
        private bool selected; 
        
        void Start()
        {
            //InstantiateSquad();
        }

        void Update()
        {
            if (aliveMembers <= 0) {
                Destroy(unitObj);
            }
        }

        public void killModel(GameObject modelObject) {
            Destroy(modelObject);
            aliveMembers--;
        }

        /// <summary>
        /// Used when spawning a new squad. Generates a new squad at target coordinates with relevant variables
        /// </summary>
        void InstantiateSquad() 
        {
            // Initializing the SquadList, the intended SquadSize, and "aliveMembers" which is used to track the current alive members of the squad
            squadSize = squadData.models.Length;

            squadMembers = new SquadMember[squadSize];

            aliveMembers = squadSize;

            Dictionary<int, Vector3> spacing = generateSpacing(transform.position);

            // Spawn loop for initializing each model of squad according to the associated modelData in the squad list
            for (int i = 0; i < squadSize; i++)
            {
                GameObject squadMemberObject = new GameObject("SquadMember" + i);
                SquadMember squadMember = squadMemberObject.AddComponent<SquadMember>();

                // Initializing a model with its associated model data
                ModelData data = squadData.models[i];

                squadMember.weaponData = data.weaponData;
                squadMember.maxHealth = data.modelHealth;
                squadMember.currHealth = data.modelHealth;
                squadMember.attackSpeed = data.attackSpeed;
                squadMember.range = data.range;
                squadMember.defense = data.defense;
                squadMember.moveSpeed = data.moveSpeed;
                squadMember.modelPriority = data.modelPriority;
                squadMember.team = squadData.team;
                squadMember.parent = this;

                squadMemberObject = Instantiate(data.model, squadMemberObject.transform.position, Quaternion.identity);

                squadMember.transform.parent = squadMemberObject.transform;

                squadMember.modelObject = squadMemberObject;

                // First model to spawn becomes the default "squadLead" - 
                // squadLead models do not get any bonuses, their coordinates are used to 
                // render the squadIcon. Upon the death of a squadLead, 
                // the reference will automatically transfer to another squadMember object
                if (i == 0 & squadLead == null) {
                    squadLead = squadMemberObject;
                }

                // Sets the 3d model to have its parent position be the coordinates of the "empty" squadMemberObject 
                //model.transform.parent = squadMemberObject.transform;

                // Position is set to the spacing coordinates defined by squad size
                squadMemberObject.transform.position = spacing[i];

                squadMemberObject.transform.SetParent(this.transform, false);

                // Squad Member list is updated with the new object
                squadMembers[i] = squadMember;
            }

            // Initializes the squad Icon and renders it above the Squad Lead. Adds the "IconBehavior" script as a component which forces the icon to always render towards
            // the player camera
            // GameObject iconObject = new GameObject(squadData.Name + " Icon");
            GameObject iconObject = Instantiate(squadData.icon, (squadLead.transform.position + new Vector3(0,3,0)), Quaternion.identity);
            iconObject.AddComponent<IconBehavior>();

            iconObject.transform.SetParent(squadLead.transform, false);
            iconObject.layer = LayerMask.NameToLayer("Selectable");
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
}*/

