using System.Collections;
using System.Collections.Generic;
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

            Vector3 targetDirection = targetPosition - currentTransform.position;
            float angleDif = Vector3.Angle(targetDirection, currentTransform.forward);

            Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, targetPosition - currentTransform.position);
            Debug.Log(rotation);
            switch (aliveMembers)
            {
                case 1:
                    moveCoordinates.Add(rotation * targetPosition);
                    break;
                
                case 2:
                    moveCoordinates.Add(rotation * targetPosition);
                    moveCoordinates.Add(targetPosition - (rotation * (new Vector3(2,0,2))));
                    break;

                case 3:
                    moveCoordinates.Add(rotation * targetPosition);
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
                    moveCoordinates.Add(rotation * targetPosition);
                    moveCoordinates.Add(targetPosition - (rotation * (new Vector3(2,0,2))));
                    moveCoordinates.Add(targetPosition + (rotation * (new Vector3(4,0,-2))));
                    moveCoordinates.Add(targetPosition + (rotation * (new Vector3(2,0,0))));
                    moveCoordinates.Add(targetPosition + (rotation * (new Vector3(1,0,-2))));
                    break;

                case 6: 
                    moveCoordinates.Add(rotation * targetPosition);
                    moveCoordinates.Add(targetPosition - (rotation * (new Vector3(2,0,2))));
                    moveCoordinates.Add(targetPosition + (rotation * (new Vector3(4,0,-2))));
                    moveCoordinates.Add(targetPosition + (rotation * (new Vector3(2,0,0))));
                    moveCoordinates.Add(targetPosition + (rotation * (new Vector3(1,0,-2))));
                    moveCoordinates.Add(targetPosition + (rotation * (new Vector3(1,0,-4))));
                    break;
            }
            return moveCoordinates;
        }

    }
}