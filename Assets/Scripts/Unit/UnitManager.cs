using UnityEngine;
using System.Collections;

namespace Capstone
{
    public class UnitManager : MonoBehaviour
    {
        
        void Awake()
        {
            StartCoroutine(waitForMatchManager());
        }

        private IEnumerator waitForMatchManager()
        {
            while (MatchManager.instance == null)
            {
                Debug.Log("Waiting, no MatchManager for initialization");
                yield return null;
            }

            MatchManager.instance.resetMatchVariables();
            MatchManager.instance.setMapPlayerSlots(2);
            MatchManager.instance.addMatchMember(0, GameActorType.player, FactionType.centralPowers);
            MatchManager.instance.addMatchMember(1, GameActorType.aiEasy, FactionType.ententeForces);
        }
    }
}
