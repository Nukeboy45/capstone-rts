using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Capstone {
    public class MapSlot : MonoBehaviour
    {
        public GameObject playerType;
        public GameObject team;
        public GameObject faction;
        
        private Dictionary<string, GameActorType> playerTypes = new Dictionary<string, GameActorType>() 
        {
            {"Player (Self)", GameActorType.player}, 
            {"AI (Easy)", GameActorType.aiEasy},
            {"AI (Hard)", GameActorType.aiHard}
        };

        private Dictionary<string, FactionType> factionTypes = new Dictionary<string, FactionType>()
        {
            {"Central Powers", FactionType.centralPowers},
            {"Allied Forces", FactionType.alliedForces}
        };

        private Dictionary<string, int> teamDict = new Dictionary<string, int>()
        {
            {"Team 1", 0},
            {"Team 2", 1}
        };

        public GameActorType getPlayerType()
        {
            TextMeshProUGUI playerName = playerType.transform.Find("Label").GetComponent<TextMeshProUGUI>();
            GameActorType returnType = playerTypes[playerName.text];
            return returnType;
        }
        
        public int getTeam()
        {
            TextMeshProUGUI teamField = team.transform.Find("Label").GetComponent<TextMeshProUGUI>();
            int teamNumber = teamDict[teamField.text];
            return teamNumber;
        }

        public FactionType getFaction()
        {
            TextMeshProUGUI playerFaction = faction.transform.Find("Label").GetComponent<TextMeshProUGUI>();
            FactionType returnType = factionTypes[playerFaction.text];
            return returnType;
        }
    }
}
