using System.Collections.Generic;
using UnityEngine;

namespace Capstone
{
    public class MatchManager : MonoBehaviour
    {
        public static MatchManager instance;

        public List<(int, GameActorType, FactionType)> matchMembers = new List<(int, GameActorType, FactionType)>();
        public int mapSlots;

        private void Awake()
        {
            createMatchManager();
        }

        void createMatchManager()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }

        public void resetMatchVariables()
        {
            matchMembers.Clear();
            mapSlots = 0;
        }
        
        // ------------------- Getters / Setters ---------------------------------------------
        public void setMapPlayerSlots(int slots)
        {
            mapSlots = slots;
        }

        public void addMatchMember(int team, GameActorType gameActorType, FactionType factionType)
        {
            matchMembers.Add((team, gameActorType, factionType));
            Debug.Log("added!");
        }

        public int getMapPlayerSlots()
        {
            return mapSlots;
        }
        public List<(int, GameActorType, FactionType)> getMatchMembers()
        {
            return matchMembers;
        }


    }
}