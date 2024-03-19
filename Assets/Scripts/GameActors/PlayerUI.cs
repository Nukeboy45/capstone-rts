using System;
using System.Collections.Generic;
using capstone;
using UnityEngine;
using UnityEngine.UI;

namespace Capstone {
    public class PlayerUI : MonoBehaviour
    {
        public Player playerObj; 
        public Button[] buttons;
        public Button buttonMenu1;
        public Button buttonMenu2;
        public FactionType faction;
        private UIState uiState = UIState.menu1;
        private Dictionary<String, Sprite> buttonImages = new Dictionary<string, Sprite>();
        public void Start()
        {
            // Menu and Unit Construction buttons
            buttonImages.Add("ausRifle", Resources.Load<Sprite>("Art/ui/uiButtonAusRifle"));

            // Unit control buttons
            buttonImages.Add("retreat", Resources.Load<Sprite>("Art/ui/uiButtonRetreat"));

            updateMenu();
        }
        public void Update()
        {   
            if (playerObj.selected.Count == 0)
            {
                if (buttonMenu1.interactable == false)
                {
                    if (uiState != UIState.menu1)
                    {
                        uiState = UIState.menu1;
                        updateMenu();
                    }
                } else if (buttonMenu2.interactable == false)
                {
                    if (uiState != UIState.menu2)
                    {
                        uiState = UIState.menu2;
                        updateMenu();
                    }
                }
            }
            else if (playerObj.selected.Count == 1)
            {
                if (Selection.getSelectionComponent<Unit>(playerObj.selected[0]) is Unit)
                {
                    if (Selection.getSelectionComponent<Unit>(playerObj.selected[0]) is Squad)
                    {
                        uiState = UIState.squad;
                        updateMenu();
                    }
                }
            } 
            else if (playerObj.selected.Count > 1)
            {

            }

        }
        public void Button1()
        {
            switch (uiState)
            {
                case UIState.menu1:
                    if (faction == FactionType.centralPowers)
                    {
                        playerObj.spawnPoint.addToBuildQueue(Resources.Load<SquadData>("Units/Austrian/Infantry/ausRifle"));
                    }
                    break;
            }
        }
        public void Button2()
        {
            Debug.Log("Button2 Pressed!");
        }
        public void Button3()
        {
            Debug.Log("Button3 Pressed!");
        }
        public void Button4()
        {
            Debug.Log("Button4 Pressed!");
        }

        public void Button5()
        {

        }

        public void Button6()
        {

        }

        public void MenuButton1()
        {

        }

        public void MenuButton2()
        {

        }

        public void updateMenu()
        {
            switch (uiState)
            {
                case UIState.menu1:
                    if (faction == FactionType.centralPowers)
                    {
                        buttons[0].GetComponent<Image>().sprite = buttonImages["ausRifle"];
                        buttons[0].interactable = true;
                    }
                    else 
                    {

                    }
                    break;

                case UIState.menu2:

                    break;

                case UIState.squad:

                    break;

                case UIState.group:

                    break;
            }
        }

        public void setPlayerObj(Player player)
        {
            playerObj = player;
        }
    }

}
