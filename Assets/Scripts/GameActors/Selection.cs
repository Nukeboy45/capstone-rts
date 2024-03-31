using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capstone
{
    public static class Selection
    {

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="startTransform"></param>
            /// <returns></returns>
            public static T getSelectionComponent<T>(Transform startTransform) where T : Component
            {
                Transform currentTransform = startTransform;

                while (currentTransform != null)
                {
                    T component = currentTransform.gameObject.GetComponent<T>();

                    if (component != null) { return component; }

                    currentTransform = currentTransform.parent;
                }

                return null;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="gameObject"></param>
            /// <returns></returns>
            public static T getSelectionComponent<T>(GameObject gameObject) where T: Component
            {
                T component = gameObject.GetComponent<T>();
                if (component is Unit) { return component; }
                if (component is Building) { return component; }
                return null;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="squadMember"></param>
            /// <param name="selected"></param>
            /// <param name="owner"></param>
            public static void squadSelect(Squad squad, List<GameObject> selected, GameActor owner)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    {
                        if (!selected.Contains(squad.gameObject))
                        {
                            if (squad.owner == owner) {
                                squad.select();
                            }
                        } else {
                            squad.deselect();
                        }
                    } else {
                        deselectAll(squad.owner);
                        if (!selected.Contains(squad.gameObject))
                        {
                            if (squad.owner == owner) {
                                squad.select();
                            }
                        }
                    }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="building"></param>
            /// <param name="selected"></param>
            /// <param name="owner"></param>
            public static void buildingSelect(Building building, List<GameObject> selected, GameActor owner)
            {
                if (building is PassiveBuilding passiveBuilding)
                {
                    if (!selected.Contains(passiveBuilding.gameObject))
                    {
                        if (passiveBuilding.owner == owner) 
                        {
                            deselectAll(owner);
                            passiveBuilding.select();
                        }
                    }
                } else if (building is DefenseBuilding defenseBuilding)
                {
                    if (!selected.Contains(defenseBuilding.gameObject))
                    {
                        if (defenseBuilding.owner == owner) 
                        {
                            deselectAll(owner);
                            defenseBuilding.select();
                        }
                    }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="owner"></param>
            public static void deselectAll(GameActor owner) 
            {
                if (owner is Player) {
                    Player playerComp = owner.GetComponent<Player>();
                    List<GameObject> selected = playerComp.getSelected();
                    if (selected.Count > 0)
                    {
                        for (int i = selected.Count - 1; i >= 0; i--) 
                        {
                            Unit unit = selected[i].GetComponent<Unit>();
                            Building building = selected[i].GetComponent<Building>();
                            if (unit != null) 
                            {
                                unit.deselect();
                            } else if (building != null) 
                            {
                                building.deselect();
                            }
                        }
                    }
                }
            }


        public static List<RaycastHit> getAdditionalCasts(RaycastHit parentHit, Camera castCamera, Transform currTransform, int squadSize, LayerMask ground)
        {
            List<Vector3> castTargets = unitFunctions.getMoveCoordinates(parentHit.point, currTransform, squadSize);
            List<RaycastHit> hits = new List<RaycastHit>();
            foreach(Vector3 target in castTargets)
            {
                Ray ray = castCamera.ScreenPointToRay(castCamera.WorldToScreenPoint(target));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
                {
                    hits.Add(hit);
                } else {
                    Debug.Log("not hitting!");
                }
            }
            return hits;
        }
    }
}