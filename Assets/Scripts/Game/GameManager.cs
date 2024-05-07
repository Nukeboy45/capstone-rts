using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Capstone
{
    public class GameManager : MonoBehaviour
    {
        // ------- Public Variables -----------
        private List<(int, GameActorType, FactionType)> matchMembers = new List<(int, GameActorType, FactionType)>();
        public Player player;
        public PlayerUI playerUI;
        [SerializeField] private CapturePoint[] objectives;
        [SerializeField] private CapturePoint[] rationObjectives;
        [SerializeField] private CapturePoint[] coalObjectives;
        public GameObject[] players; // 0-3 is team 1, 4-7 is team 2
        public SpawnPoint[] spawns; // 0-3 is team 1, 4-7 is team 2
        public Camera rayCamera;

        // ---- Private Variables ------
        [SerializeField] private int team1Tickets = 50;
        [SerializeField] private int team2Tickets = 50;

        // Timing Variables
        private DateTime lastObjectiveTick;
        private float objectivePointTickTime = 5f;

        // Singleton
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
                        player = playerComponent;
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

        private IEnumerator InitalizeHeadquarters()
        {
            foreach (GameObject gameActor in players)
            {
                if (gameActor != null)
                {
                    GameActor component = gameActor.GetComponent<GameActor>();
                    if (component is Player)
                    {
                        Player playerComp = (Player)component;
                        StartCoroutine(waitForPlayerCamera(playerComp, spawns[component.ownerTag].getCameraSpawnPosition()));
                    }
                    switch (component.faction)
                    {
                        case FactionType.centralPowers:
                            AsyncOperationHandle cenHeadquartersHandle = Addressables.LoadAssetAsync<GameObject>("cenHeadquarters");
                            yield return StartCoroutine(SpawnBuilding(cenHeadquartersHandle, spawns[component.ownerTag].gameObject, component));
                            break;
                        case FactionType.ententeForces:
                            AsyncOperationHandle entHeadquartersHandle = Addressables.LoadAssetAsync<GameObject>("cenHeadquarters");
                            yield return StartCoroutine(SpawnBuilding(entHeadquartersHandle, spawns[component.ownerTag].gameObject, component));
                            break;
                    }
                }
            }
            yield break;
        }

        private IEnumerator SpawnBuilding(AsyncOperationHandle buildingAsyncOperation, GameObject spawnPosition, GameActor owner)
        {
            GameObject buildingPrefab;
            yield return buildingAsyncOperation;
            if (buildingAsyncOperation.Status == AsyncOperationStatus.Succeeded)
            {
                buildingPrefab = buildingAsyncOperation.Result as GameObject;
            } 
            else 
            {
                yield break;
            }
            GameObject newHeadquarters = Instantiate(buildingPrefab, spawnPosition.transform.position, spawnPosition.transform.rotation);
            ProductionStructure newHeadquartersComponent = newHeadquarters.GetComponent<ProductionStructure>();
            owner.headquarters = newHeadquartersComponent;
            newHeadquartersComponent.owner = owner;
            newHeadquartersComponent.team = owner.team;
            owner.retreatPoint = newHeadquartersComponent.getSpawnPoint();
            yield break;
        }

        // Update is called once per frame
        void Update()
        {
            TimeSpan elapsedObjectiveTime = DateTime.Now - lastObjectiveTick;
            if (elapsedObjectiveTime.Seconds >= objectivePointTickTime)
            {
                capturePointTicketUpdate();
                lastObjectiveTick = DateTime.Now;
            }
        }

        private bool paused = false;
        public void pause()
        {
            if (paused == false)
            {
                paused = true;
                playerUI.showPauseMenu();
            }
            else 
            {
                resume();
                return;
            }
            Time.timeScale = 0f;
        }
        public void resume()
        {
            paused = false;
            playerUI.hidePauseMenu();
            Time.timeScale = 1f;
        }

        private void capturePointTicketUpdate()
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
            if (team1Points > team2Points) {
                if (team2Tickets - team1Points < 0)
                {
                    team2Tickets = 0;
                } else {
                    team2Tickets -= team1Points;
                }
            } else if (team2Points > team1Points) {
                if (team1Tickets - team2Points < 0) {
                    team1Tickets = 0;
                } else {
                    team1Tickets -= team2Points;
                }
            }

            foreach(GameObject gameActor in players)
            {
                Player playerComp = gameActor.GetComponent<Player>();
                if (playerComp != null)
                    playerComp.GetPlayerUI().updateScoreBars(team1Tickets, team2Tickets);
            }

            if (team1Tickets == 0 || team2Tickets == 0 && Time.timeScale != 0f)
            {
                bool playerVictory = false;
                FactionType playerFaction = FactionType.none;
                if (team1Tickets == 0)
                {
                    // Team 1 Victory
                    playerVictory = player.team == 1;
                    playerFaction = player.faction;
                }

                if (team2Tickets == 0)
                {
                    // Team 2 Victory
                    playerVictory = player.team == 0;
                    playerFaction = player.faction;
                }
                if (gameOver == false)
                {
                    gameOver = true;
                    AsyncOperationHandle winScreenHandle = Addressables.LoadAssetAsync<GameObject>("WinConditionCanvas");
                    StartCoroutine(spawnWinConditionScreen(winScreenHandle, playerVictory, playerFaction));
                }
            }
        }

        private bool gameOver = false;
        private IEnumerator spawnWinConditionScreen(AsyncOperationHandle winScreenHandle, bool playerTeamVictory, FactionType playerFaction)
        {
            GameObject winScreenPrefab = null;
            yield return winScreenHandle;
            if (winScreenHandle.Status == AsyncOperationStatus.Succeeded) 
            {
                winScreenPrefab = winScreenHandle.Result as GameObject;
            } 
            else 
            {
                yield break;
            }
            if (winScreenPrefab != null)
            {
                GameObject winScreenObject = Instantiate(winScreenPrefab, playerUI.transform);
                WinConditionCanvas winConditionComponent = winScreenObject.GetComponent<WinConditionCanvas>();
                winConditionComponent.playerTeamVictory = playerTeamVictory;
                winConditionComponent.playerFaction = playerFaction;
            }
            Time.timeScale = 0f;
            yield break;
        }

        private IEnumerator waitForMatchManager()
        {
            while (MatchManager.instance == null)
            {
                yield return null;
            }

            while (MatchManager.instance.getMatchMembers().Count < 2)
            {
                yield return null;
            }

            matchMembers = MatchManager.instance.getMatchMembers();
            Debug.Log("Initializing teams!");
            InitializeTeams();
            StartCoroutine(InitalizeHeadquarters());
        }

        private IEnumerator waitForPlayerUI(Player playerComponent)
        {
            while (playerComponent.GetPlayerUI() == null)
            {
                yield return null;
            }

            playerUI = playerComponent.GetPlayerUI();
            playerComponent.GetPlayerUI().updateScoreBars(team1Tickets, team2Tickets);
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
