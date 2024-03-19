using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capstone
{
    public class GameManager : MonoBehaviour
    {
        // ------- Public Variables -----------
        private List<(int, GameActorType, FactionType)> matchMembers = new List<(int, GameActorType, FactionType)>();
        public GameObject[] players; // 0-3 is team 1, 4-7 is team 2
        public SpawnPoint[] spawns; // 0-3 is team 1, 4-7 is team 2
        public Camera rayCamera;
        
        // ---- Private Variables ------
        private int team1Tickets;
        private int team2Tickets;
        //private static GameManager _instance;
        //public static GameManager Instance { get {return _instance; } }

        // Start is called before the first frame update
        void Awake()
        {
            /*if (_instance != null && _instance != this)
            {
                Destroy(this);
            } else {
                _instance = this;
            }*/
            matchMembers = MatchManager.instance.getMatchMembers();
            InitializeTeams();
            InitalizeSpawnPoints();
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
            
            GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/GameActors/PlayerPrefab");
            GameObject aiPrefab = Resources.Load<GameObject>("Prefabs/GameActors/AIPrefab");

            foreach ((int, GameActorType, FactionType) gameActor in matchMembers)
            {
                switch (gameActor.Item2)
                {
                    case (GameActorType.player): {
                        GameObject newTeamMember = Instantiate(playerPrefab, new Vector3(0,0,0), Quaternion.identity);
                        Player playerComponent = newTeamMember.GetComponent<Player>();
                        if (playerComponent != null) 
                        {
                            playerComponent.ownerTag = ownerTag;
                            playerComponent.team = gameActor.Item1;
                            playerComponent.faction = gameActor.Item3;
                            playerComponent.rayCamera = rayCamera;
                            newTeamMember.name = "Player" + playerCount + "Team" + gameActor.Item1.ToString();
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
                            computerPlayerComponent.team = gameActor.Item1;
                            computerPlayerComponent.faction = gameActor.Item3;
                            computerPlayerComponent.difficulty = "easy";
                            computerPlayerComponent.rayCamera = rayCamera;
                            newTeamMember.name = "AI" + aiCount + computerPlayerComponent.difficulty + gameActor.Item1.ToString();
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
                            computerPlayerComponent.team = gameActor.Item1;
                            computerPlayerComponent.faction = gameActor.Item3;
                            computerPlayerComponent.difficulty = "hard";
                            computerPlayerComponent.rayCamera = rayCamera;
                            newTeamMember.name = "AI" + aiCount + computerPlayerComponent.difficulty + gameActor.Item1.ToString();
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
            /*foreach (GameActorType type in team2)
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
                            playerComponent.rayCamera = rayCamera;
                            newTeamMember.name = "Player" + playerCount + "Team2";
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
                            computerPlayerComponent.rayCamera = rayCamera;
                            computerPlayerComponent.difficulty = "easy";
                            newTeamMember.name = "AI" + aiCount + computerPlayerComponent.difficulty + "Team2";
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
                            computerPlayerComponent.rayCamera = rayCamera;
                            computerPlayerComponent.difficulty = "hard";
                            newTeamMember.name = "AI" + aiCount + computerPlayerComponent.difficulty + "Team2";
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
            */
        }

        private void InitalizeSpawnPoints()
        {
            foreach (GameObject gameActor in players)
            {
                if (gameActor != null)
                {
                    GameActor component = gameActor.GetComponent<GameActor>();
                    component.spawnPoint = spawns[component.ownerTag];
                    component.spawnPoint.faction = component.faction;
                    component.spawnPoint.team = component.team;
                    component.spawnPoint.ownerTag = component.ownerTag;
                    component.spawnPoint.owner = component;
                    component.spawnPoint.setGameManager(this);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }


    }
}
