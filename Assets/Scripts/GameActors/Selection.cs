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
            public static void squadSelect(SquadMember squadMember, List<GameObject> selected, GameActor owner)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    {
                        if (!selected.Contains(squadMember.parent.gameObject))
                        {
                            if (squadMember.parent.owner == owner) {
                                squadMember.parent.select();
                            }
                        } else {
                            squadMember.parent.deselect();
                        }
                    } else {
                        deselectAll(squadMember.parent.owner);
                        if (!selected.Contains(squadMember.parent.gameObject))
                        {
                            if (squadMember.parent.owner == owner) {
                                squadMember.parent.select();
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
                            Unit component = selected[i].GetComponent<Unit>();
                            component.deselect();
                        }
                    }
                }
            }
    }
}