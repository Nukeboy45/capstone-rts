using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Capstone
{
    public class GameManager : MonoBehaviour
    {
        // ------- Public Variables -----------
        private List<(int, GameActorType, FactionType)> matchMembers = new List<(int, GameActorType, FactionType)>();
        public Player playerReference;
        public CapturePoint[] objectives;
        public GameObject[] players; // 0-3 is team 1, 4-7 is team 2
        public SpawnPoint[] spawns; // 0-3 is team 1, 4-7 is team 2
        public Camera rayCamera;

        // ---- Private Variables ------
        private int team1Tickets = 100;
        private int team2Tickets = 100;

        // Timing Variables
        private DateTime lastObjectiveTick;
        private float objectivePointTickTime = 5f;

        private static GameManager instance;
        public static GameManager Instance { get {return instance; } }

        private void Awake()
        {
            createGameManager();
        }

        void createGameManager()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {
            lastObjectiveTick = DateTime.Now;
            StartCoroutine(waitForMatchManager());
        }

        /// <summary>
        /// Goes through the GameManager lists for each team and initalizes their
        /// member GameObjects with relevant names (either human controlled instances
        /// or AI controlled)
        /// </summary>
        void InitializeTeams()
        {
            players = new GameObject[MatchManager.instance.mapSlots];
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
                        playerReference = playerComponent;
                        StartCoroutine(waitForPlayerUI(playerComponent));
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
        }

        private void InitalizeSpawnPoints()
        {
            foreach (GameObject gameActor in players)
            {
                if (gameActor != null)
                {
                    GameActor component = gameActor.GetComponent<GameActor>();
                    component.spawnPoint = spawns[component.ownerTag];
                    if (component is Player)
                    {
                        Player playerComp = (Player)component;
                        StartCoroutine(waitForPlayerCamera(playerComp, playerComp.spawnPoint.getCameraSpawnPosition()));
                    }
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
            TimeSpan elapsedObjectiveTime = DateTime.Now - lastObjectiveTick;
            if (elapsedObjectiveTime.Seconds >= objectivePointTickTime)
            {
                checkCapturePoints();
                lastObjectiveTick = DateTime.Now;
            }
        }

        private void checkCapturePoints()
        {
            int team1Points = 0;
            int team2Points = 0;
            foreach (CapturePoint point in objectives)
            {
                if (point.getOwner() == 1)
                    team1Points++;
                else if (point.getOwner() == 2)
                    team2Points++;
            }
            if (team1Points > team2Points)
                team2Tickets -= team1Points;
            else if (team2Points > team1Points)
                team1Tickets -= team2Points;
            
            foreach(GameObject gameActor in players)
            {
                Player playerComp = gameActor.GetComponent<Player>();
                if (playerComp != null)
                    playerComp.playerUI.updateScoreBars(team1Tickets, team2Tickets);
            }
        }

        private IEnumerator waitForMatchManager()
        {
            while (MatchManager.instance == null)
            {
                Debug.Log("Waiting, no MatchManager for data");
                yield return null;
            }

            while (MatchManager.instance.getMatchMembers().Count < 2)
            {
                Debug.Log("Waiting, not enough players");
                yield return null;
            }

            matchMembers = MatchManager.instance.getMatchMembers();
            Debug.Log("Initializing teams!");
            InitializeTeams();
            InitalizeSpawnPoints();
        }

        private IEnumerator waitForPlayerUI(Player playerComponent)
        {
            while (playerComponent.playerUI == null)
            {
                yield return null;
            }

            playerComponent.playerUI.updateScoreBars(team1Tickets, team2Tickets);
        }

        private IEnumerator waitForPlayerCamera(Player playerComponent, Vector3 position)
        {
            while (playerComponent.getPlayerCamera() == null)
            {
                yield return null;
            }

            playerComponent.setCameraPosition(position);
        }

    }
}
