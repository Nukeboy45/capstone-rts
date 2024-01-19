/* using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capstone
{
    public class GameManager : MonoBehaviour
    {
        public GameObject playerPrefab;
        public GameObject aiPrefab;
        public GameActorType[] team1;
        public GameActorType[] team2;
        private GameObject[] players; // 0-3 is team 1, 4-7 is team 2;
        public HQSpawnPoint[] team1BaseSpawns;
        public HQSpawnPoint[] team2BaseSpawns;
        private int team1Tickets;
        private int team2Tickets;

        // Start is called before the first frame update
        void Start()
        {
            InitializeTeams();
            InitalizeBases();
        }

        /// <summary>
        /// Goes through the GameManager lists for each team and initalizes their
        /// member GameObjects with relevant names (either human controlled instances
        /// or AI controlled)
        /// </summary>
        void InitializeTeams()
        {
            int playerCount = 1;
            int aiCount = 1;
            int ownerTag = 0;
            foreach (GameObject i in team1)
            {
                GameObject newTeamMember = Instantiate(i, new Vector3(0,0,0), Quaternion.identity);
                GameActor component = newTeamMember.GetComponent<GameActor>();
                
                if (component != null)
                {
                    switch (component)
                    {
                        case Player playerComponent:
                            playerComponent.ownerTag = ownerTag;
                            playerComponent.team = 0;
                            playerComponent.faction = "aus";
                            newTeamMember.name = "Player" + playerCount + " Team 1";
                            playerCount++;
                            break;
                        case ComputerPlayer computerPlayerComponent:
                            computerPlayerComponent.ownerTag = ownerTag;
                            computerPlayerComponent.team = 0;
                            computerPlayerComponent.faction = "aus";
                            newTeamMember.name = "AI" + aiCount + " Team 1";
                            aiCount++;
                            break;
                        default:
                            Debug.Log("Added actor is neither a player nor an AI");
                            break;
                    }
                }
                team1Obj.Add(newTeamMember);
                ownerTag++;
            }
            ownerTag = 5;
            foreach (GameObject i in team2)
            {
                GameObject newTeamMember = Instantiate(i, new Vector3(0,0,0), Quaternion.identity);
                GameActor component = newTeamMember.GetComponent<GameActor>();

                if (component != null)
                {
                    switch (component)
                    {
                        case Player playerComponent:
                            playerComponent.ownerTag = ownerTag;
                            playerComponent.team = 1;
                            playerComponent.faction = "ita";
                            newTeamMember.name = "Player" + playerCount + " Team 2";
                            playerCount++;
                            break;
                        case ComputerPlayer computerPlayerComponent:
                            computerPlayerComponent.ownerTag = ownerTag;
                            computerPlayerComponent.team = 1;
                            computerPlayerComponent.faction = "ita";
                            newTeamMember.name = "AI" + aiCount + " Team 2";
                            aiCount++;
                            break;
                        default:
                            Debug.Log("Added actor is neither a player nor an AI");
                            break;
                    }
                }
                team2Obj.Add(newTeamMember);
                ownerTag++;
            }
        }

        /// <summary>
        /// For each existing Entity, spawns an HQ structure at map-defined spawn points
        /// </summary>
        void InitalizeBases()
        {
            /// Initializes Team 1 First
            for (int i = 0; i < team1.Length; i++)
            {
                /// The Player / ComputerPlayer inheret from a generic class GameActor
                /// so that a switch statement can be used to handle their component rather
                /// than writing an if-else check for each possible component.
                GameActor component = team1[i].GetComponent<GameActor>();
                Debug.Log(component.faction);
                if (component != null)
                {
                    switch (component.faction)
                    {
                        case "aus":
                            /// Load Austrian HQ building data from resources and spawn
                            PassiveBuildingData ausHQ = Resources.Load<PassiveBuildingData>("Assets/Resources/Buildings/Austrian/PassiveBuildings/ausHQ");
                            EntitySpawner.SpawnPassiveBuilding(ausHQ, team1BaseSpawns[i].transform.position, 0, component.ownerTag);
                            break;
                        case "ita":
                            /// Load Italian HQ building data from resources and spawn
                            PassiveBuildingData itaHQ = Resources.Load<PassiveBuildingData>("Assets/Resources/Buildings/Italian/PassiveBuildings/itaHQ");
                            EntitySpawner.SpawnPassiveBuilding(itaHQ, team1BaseSpawns[i].transform.position, 0, component.ownerTag);
                            break;
                        default:
                            Debug.Log("GameActor does not have a correct faction tag");
                            break;
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}

/// Legacy Code
/// *** TO BE REMOVED ***
                /*if (playerComponent != null)
                {

                }
                ComputerPlayer computerPlayerComponent = newTeamMember.GetComponent<ComputerPlayer>();
                if (computerPlayerComponent != null)
                {
                    computerPlayerComponent.ownerTag = ownerTag;
                    computerPlayerComponent.team = 0;
                    newTeamMember.name = "AI" + aiCount + " Team 1";
                    aiCount++;
                }
                ownerTag++;*/
                /*if (playerComponent != null)
                {
                    playerComponent.ownerTag = ownerTag;
                    playerComponent.team = 1;
                    newTeamMember.name = "Player" + playerCount + " Team 2";
                    playerCount++;
                }
                ComputerPlayer computerPlayerComponent = newTeamMember.GetComponent<ComputerPlayer>();
                if (computerPlayerComponent != null)
                {
                    computerPlayerComponent.ownerTag = ownerTag;
                    computerPlayerComponent.team = 1;
                    newTeamMember.name = "AI" + aiCount + " Team 2";
                    aiCount++;
                }*/