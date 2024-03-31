using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Capstone
{
    public static class unitFunctions 
    {
        /// <summary>
        /// Generates a Dictionary of Vector3 values for use in spawning and movement commands for infantry
        /// squads. 
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="aliveMembers"></param>
        /// <returns></returns>
        public static List<Vector3> generateSpacing(Vector3 targetPosition, int aliveMembers)
        {
            List<Vector3> spacingList = new List<Vector3>();
            switch (aliveMembers)
            {
                case 1:
                    spacingList.Add(targetPosition);
                    break;
                
                case 2:
                    spacingList.Add(targetPosition);
                    spacingList.Add(targetPosition - new Vector3(2,0,0));
                    break;

                case 3:
                    spacingList[0] = targetPosition;
                    spacingList[1] = targetPosition - new Vector3(2,0,2);
                    spacingList[2] = targetPosition + new Vector3(2,0,2);
                    break;

                case 4:
                    spacingList.Add(targetPosition);
                    spacingList.Add(targetPosition - new Vector3(2,0,2));
                    spacingList.Add(targetPosition + new Vector3(4,0,-2));
                    spacingList.Add(targetPosition + new Vector3(2,0,0));
                    break;

                case 5:
                    spacingList.Add(targetPosition);
                    spacingList.Add(targetPosition - new Vector3(2,0,2));
                    spacingList.Add(targetPosition + new Vector3(4,0,-2));
                    spacingList.Add(targetPosition + new Vector3(2,0,0));
                    spacingList.Add(targetPosition + new Vector3(1,0,-2));
                    break;

                case 6: 
                    spacingList.Add(targetPosition);
                    spacingList.Add(targetPosition - new Vector3(2,0,2));
                    spacingList.Add(targetPosition + new Vector3(4,0,-2));
                    spacingList.Add(targetPosition + new Vector3(2,0,0));
                    spacingList.Add(targetPosition + new Vector3(1,0,-2));
                    spacingList.Add(targetPosition + new Vector3(1,0,-4));
                    break;
            }
            return spacingList;
        }

        public static List<Vector3> getMoveCoordinates(Vector3 targetPosition, Transform currentTransform, int aliveMembers) 
        {
            List<Vector3> moveCoordinates = new List<Vector3>();

            Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, targetPosition - currentTransform.position);
            switch (aliveMembers)
            {
                case 1:
                    moveCoordinates.Add(targetPosition);
                    break;
                
                case 2:
                    moveCoordinates.Add(targetPosition);
                    moveCoordinates.Add(targetPosition - (rotation * (new Vector3(2,0,2))));
                    break;

                case 3:
                    moveCoordinates.Add(targetPosition);
                    moveCoordinates.Add(targetPosition - (rotation * (new Vector3(2,0,2))));
                    moveCoordinates.Add(targetPosition + (rotation * (new Vector3(4,0,-2))));
                    break;

                case 4:
                    moveCoordinates.Add(targetPosition);
                    moveCoordinates.Add(targetPosition - (rotation * (new Vector3(2,0,2))));
                    moveCoordinates.Add(targetPosition + (rotation * (new Vector3(4,0,-2))));
                    moveCoordinates.Add(targetPosition + (rotation * (new Vector3(2,0,0))));
                    break;

                case 5:
                    moveCoordinates.Add(targetPosition);
                    moveCoordinates.Add(targetPosition - (rotation * (new Vector3(2,0,2))));
                    moveCoordinates.Add(targetPosition + (rotation * (new Vector3(4,0,-2))));
                    moveCoordinates.Add(targetPosition + (rotation * (new Vector3(2,0,0))));
                    moveCoordinates.Add(targetPosition + (rotation * (new Vector3(1,0,-2))));
                    break;

                case 6: 
                    moveCoordinates.Add(targetPosition);
                    moveCoordinates.Add(targetPosition - (rotation * (new Vector3(2,0,2))));
                    moveCoordinates.Add(targetPosition + (rotation * (new Vector3(4,0,-2))));
                    moveCoordinates.Add(targetPosition + (rotation * (new Vector3(2,0,0))));
                    moveCoordinates.Add(targetPosition + (rotation * (new Vector3(1,0,-2))));
                    moveCoordinates.Add(targetPosition + (rotation * (new Vector3(1,0,-4))));
                    break;
            }
            return moveCoordinates;
        }

        // ----------- Squad Movement ------------------

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hits"></param>
        /// <returns></returns>
        public static Dictionary<GameObject, RaycastHit> getOptimalMovePositions(List<RaycastHit> hits, GameObject[] squadMembers, GameObject squadLead) 
        {
            List<Dictionary<GameObject, RaycastHit>> permutations = getPermutations(squadMembers, hits);

            Dictionary<GameObject, RaycastHit> returnDict = new Dictionary<GameObject, RaycastHit>();
            
            float minH = 99999f;

            foreach (Dictionary<GameObject, RaycastHit> permutation in permutations)
            {   
                float cost = 0f;
                foreach (var pair in permutation)
                {
                    cost += Vector3.Distance(pair.Key.transform.position, pair.Value.point);
                }
                if (cost < minH && permutation[squadLead].point == hits[0].point)
                {
                    minH = cost;
                    returnDict = permutation;
                }
            }
            return returnDict;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="GameObject"></typeparam>
        /// <typeparam name="RaycastHit"></typeparam>
        /// <param name="members"></param>
        /// <param name="hits"></param>
        /// <returns></returns>
        private static List<Dictionary<GameObject, RaycastHit>> getPermutations<GameObject, RaycastHit>(GameObject[] members, List<RaycastHit> hits)
        {
            List<Dictionary<GameObject, RaycastHit>> result = new List<Dictionary<GameObject, RaycastHit>>();
            int len = members.Length;
            RaycastHit[] hitsArray = hits.ToArray();

            RaycastHit squadLeadPosition = hits[0];

            void heaps(int l)
            {
                if (l == 1)
                {
                    Dictionary<GameObject, RaycastHit> temp = new Dictionary<GameObject, RaycastHit>();
                    for (int i=0; i < len; i++)
                    {
                        if (i == 0) {
                            temp[members[i]] = squadLeadPosition;
                        } else {
                            temp[members[i]] = hitsArray[i];
                        }
                    }
                    result.Add(temp);
                } 
                else 
                {
                    for (int i=0; i < len; i++)
                    {
                        heaps(l-1);

                        if (l % 2 == 1)
                        {
                            (members[i], members[l - 1]) = (members[l - 1], members[i]);
                        } else {
                            (members[0], members[l - 1]) = (members[l - 1], members[0]);
                        }
                    }
                }
            }
            heaps(len);
            return result;
        }

        public static float getCameraRotationDifference(Transform cameraTransform, Transform targetTransform)
        {
            Vector3 NormalizedVector = new Vector3(targetTransform.position.x - cameraTransform.position.x, 0f, targetTransform.position.z - cameraTransform.position.z);

            float angle = Vector3.Angle(NormalizedVector, cameraTransform.forward);
            
            return angle;
        }

    }
}