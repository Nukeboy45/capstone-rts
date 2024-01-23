using System.Collections;
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
        private GameObject[] players; // 0-3 is team 1, 4-7 is team 2
        public HQSpawnPoint[] baseSpawns; // 0-3 is team 1, 4-7 is team 2
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
            players = new GameObject[8];
            int playerCount = 1;
            int aiCount = 1;
            int ownerTag = 0;

            foreach (GameActorType type in team1)
            {
                switch (type)
                {
                    case (GameActorType.player): {
                        GameObject newTeamMember = Instantiate(playerPrefab, new Vector3(0,0,0), Quaternion.identity);
                        Player playerComponent = newTeamMember.GetComponent<Player>();
                        if (playerComponent != null) 
                        {
                            playerComponent.ownerTag = ownerTag;
                            playerComponent.team = 0;
                            playerComponent.faction = "aus";
                            newTeamMember.name = "Player" + playerCount + " Team 1";
                            playerCount++;
                        }
                        players[ownerTag] = newTeamMember;
                        ownerTag++;
                        break;
                    }
                    case (GameActorType.aiEasy): {
                        GameObject newTeamMember = Instantiate(aiPrefab, new Vector3(0,0,0), Quaternion.identity);
                        ComputerPlayer computerPlayerComponent = newTeamMember.GetComponent<ComputerPlayer>();
                        if (computerPlayerComponent != null)
                        {
                            computerPlayerComponent.ownerTag = ownerTag;
                            computerPlayerComponent.team = 0;
                            computerPlayerComponent.faction = "aus";
                            computerPlayerComponent.difficulty = "easy";
                            newTeamMember.name = "AI" + aiCount + computerPlayerComponent.difficulty + " Team 1";
                            aiCount++;
                        }
                        players[ownerTag] = newTeamMember;
                        ownerTag++;
                        break;
                    }
                    case (GameActorType.aiHard): {
                        GameObject newTeamMember = Instantiate(aiPrefab, new Vector3(0,0,0), Quaternion.identity);
                        ComputerPlayer computerPlayerComponent = newTeamMember.GetComponent<ComputerPlayer>();
                        if (computerPlayerComponent != null)
                        {
                            computerPlayerComponent.ownerTag = ownerTag;
                            computerPlayerComponent.team = 0;
                            computerPlayerComponent.faction = "aus";
                            computerPlayerComponent.difficulty = "hard";
                            newTeamMember.name = "AI" + aiCount + computerPlayerComponent.difficulty + " Team 1";
                            aiCount++;
                        }
                        players[ownerTag] = newTeamMember;
                        ownerTag++;
                        break;
                    }
                    default: 
                        Debug.Log("Incorrect Player Object defined, could not instantiate");
                        break;
                }
            }
            ownerTag = 4;
            foreach (GameActorType type in team2)
            {
                switch (type)
                {
                    case (GameActorType.player): {
                        GameObject newTeamMember = Instantiate(playerPrefab, new Vector3(0,0,0), Quaternion.identity);
                        Player playerComponent = newTeamMember.GetComponent<Player>();
                        if (playerComponent != null) 
                        {
                            playerComponent.ownerTag = ownerTag;
                            playerComponent.team = 1;
                            playerComponent.faction = "ita";
                            newTeamMember.name = "Player" + playerCount + " Team 1";
                            playerCount++;
                        }
                        players[ownerTag] = newTeamMember;
                        ownerTag++;
                        break;
                    }
                    case (GameActorType.aiEasy): {
                        GameObject newTeamMember = Instantiate(aiPrefab, new Vector3(0,0,0), Quaternion.identity);
                        ComputerPlayer computerPlayerComponent = newTeamMember.GetComponent<ComputerPlayer>();
                        if (computerPlayerComponent != null)
                        {
                            computerPlayerComponent.ownerTag = ownerTag;
                            computerPlayerComponent.team = 1;
                            computerPlayerComponent.faction = "ita";
                            computerPlayerComponent.difficulty = "easy";
                            newTeamMember.name = "AI" + aiCount + computerPlayerComponent.difficulty + " Team 2";
                            aiCount++;
                        }
                        players[ownerTag] = newTeamMember;
                        ownerTag++;
                        break;
                    }
                    case (GameActorType.aiHard): {
                        GameObject newTeamMember = Instantiate(aiPrefab, new Vector3(0,0,0), Quaternion.identity);
                        ComputerPlayer computerPlayerComponent = newTeamMember.GetComponent<ComputerPlayer>();
                        if (computerPlayerComponent != null)
                        {
                            computerPlayerComponent.ownerTag = ownerTag;
                            computerPlayerComponent.team = 1;
                            computerPlayerComponent.faction = "ita";
                            computerPlayerComponent.difficulty = "hard";
                            newTeamMember.name = "AI" + aiCount + computerPlayerComponent.difficulty + " Team 2";
                            aiCount++;
                        }
                        players[ownerTag] = newTeamMember;
                        ownerTag++;
                        break;
                    }
                    default: 
                        Debug.Log("Incorrect Player Object defined, could not instantiate");
                        break;
                }
            }
        }

        /// <summary>
        /// For each existing Entity, spawns an HQ structure at map-defined spawn points
        /// </summary>
        void InitalizeBases()
        {
            /// Initalizes bases for existing gameActors in the player list
            foreach (GameObject gameActor in players)
            {
                /// The Player / ComputerPlayer inheret from a generic class GameActor
                /// so that a switch statement can be used to handle their component rather
                /// than writing an if-else check for each possible component.
                if (gameActor != null)
                {
                    /// Getting the component of the gameActor object
                    GameActor component = gameActor.GetComponent<GameActor>();
                    Debug.Log(component.faction);
                    if (component != null)
                    {
                        switch (component.faction)
                        {
                            case "aus":
                                /// Load Austrian HQ building data from resources and spawn
                                PassiveBuildingData ausHQ = Resources.Load<PassiveBuildingData>("Buildings/Austrian/PassiveBuildings/ausHQ");
                                if (ausHQ != null)
                                {
                                    EntitySpawner.SpawnPassiveBuilding(ausHQ, baseSpawns[component.ownerTag].transform.position, component.team, component.ownerTag);
                                } else {
                                    Debug.Log("Austrian HQ object is", ausHQ);
                                }
                                break;
                            case "ita":
                                /// Load Italian HQ building data from resources and spawn
                                PassiveBuildingData itaHQ = Resources.Load<PassiveBuildingData>("Buildings/Italian/PassiveBuildings/itaHQ");
                                if (itaHQ != null)
                                {
                                    EntitySpawner.SpawnPassiveBuilding(itaHQ, baseSpawns[component.ownerTag].transform.position, component.team, component.ownerTag);
                                } else {
                                    Debug.Log("Italian HQ object is ", itaHQ);
                                }
                                break;
                            default:
                                Debug.Log("GameActor does not have a correct faction tag");
                                break;
                        }
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