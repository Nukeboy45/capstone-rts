using UnityEngine;

namespace Capstone
{
    public class UnitManager : MonoBehaviour
    {
        
        void Start()
        {
            SquadData data = Resources.Load<SquadData>("Units/Austrian/Infantry/ausRifle");
            EntitySpawner.SquadSpawn(data, new Vector3(0,0,0), 0, 0);
            EntitySpawner.SquadSpawn(data, new Vector3(5,0,5), 0, 0);
            EntitySpawner.SquadSpawn(data, new Vector3(-5,0,-5), 0, 0);
            EntitySpawner.SquadSpawn(data, new Vector3(5,0,-5), 1, 4);
            EntitySpawner.SquadSpawn(data, new Vector3(0,0,-15), 0, 0);
            
        }
        
    }
}
