using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Capstone {
    public class PlayerUI : MonoBehaviour
    {
        // public / editor variables
        public Player playerObj; 

        // Private, Editor-Accessible Variables
        [SerializeField] private Button[] buttons;
        [SerializeField] private List<scoreBarList> scoreBars = new List<scoreBarList>();
        [SerializeField] private List<GameObject> unitIconPositions = new List<GameObject>();

        // Private Runtime Variables
        private UIState uiState = UIState.defaultMenu;
        private Dictionary<String, Sprite> buttonImages = new Dictionary<string, Sprite>();
        private List<GameObject> unitIconList = new List<GameObject>();
        private FactionType faction;
        
        // Prefabs
        private GameObject emptyUnitIconUI;
        public void Start()
        {
            switch (faction)
            {
                case (FactionType.centralPowers):
                    // Menu and Unit Construction buttons - Central Powers
                    buttonImages.Add("ausRifle", Resources.Load<Sprite>("Art/ui/uiButtonAusRifle"));
                    break;
            }
            // Generic Button Images
            buttonImages.Add("retreat", Resources.Load<Sprite>("Art/ui/uiButtonRetreat"));

            // Initializing the Empty UnitIconUI Prefab
            emptyUnitIconUI = Resources.Load<GameObject>("Prefabs/UI/UnitIconUI");

            updateMenu();
        }
        public void Update()
        {   
            if (playerObj.selected.Count == 0)
            {
                uiState = UIState.defaultMenu;
                updateMenu();
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
                case UIState.defaultMenu:
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
        
        // Update Functions
        public void updateMenu()
        {
            switch (uiState)
            {
                case UIState.defaultMenu:
                    if (faction == FactionType.centralPowers)
                    {
                        buttons[0].GetComponent<Image>().sprite = buttonImages["ausRifle"];
                        buttons[0].interactable = true;
                    }
                    else 
                    {

                    }
                    break;

                case UIState.squad:

                    break;

                case UIState.group:

                    break;
            }
        }

        public void updateScoreBars(int team1Tickets, int team2Tickets)
        {
            if (playerObj.team == 0)
            {
                scoreBars[0].sliderComponent.value = team1Tickets;
                scoreBars[0].textComponent.text = team1Tickets.ToString();
                scoreBars[1].sliderComponent.value = team2Tickets;
                scoreBars[1].textComponent.text = team2Tickets.ToString();
            }
            else 
            {
                scoreBars[1].sliderComponent.value = team1Tickets;
                scoreBars[1].textComponent.text = team1Tickets.ToString();
                scoreBars[0].sliderComponent.value = team2Tickets;
                scoreBars[0].textComponent.text = team2Tickets.ToString();
            }
        }

        public void updateUnitIconBarPositions()
        {
            for (int i = 0; i < unitIconList.Count; i++)
            {
                unitIconList[i].transform.parent = unitIconPositions[i].transform;
                Debug.Log("Updated Parent!");
                unitIconList[i].transform.localPosition = Vector3.zero;
            }
        }
        
        // ---------- Getters / Setters / Modification Functions --------------
        public void setPlayerObj(Player player)
        {
            playerObj = player;
        }

        public void setFaction(FactionType factionType)
        {
            faction = factionType;
        }

        public UnitIconUI addNewUnitIcon(Sprite portraitImage, Sprite iconImage)
        {
            GameObject newIcon = Instantiate(emptyUnitIconUI, Vector3.zero, Quaternion.identity);
            unitIconList.Add(newIcon);
            UnitIconUI returnIconUI = newIcon.GetComponent<UnitIconUI>();
            StartCoroutine(waitForIconInitialization(returnIconUI, portraitImage, iconImage));
            updateUnitIconBarPositions();
            return returnIconUI;
        }

        public void removeUnitIcon(GameObject removeIcon)
        {
            unitIconList.Remove(removeIcon);
            updateUnitIconBarPositions();
        }

        // INumerators
        private IEnumerator waitForIconInitialization(UnitIconUI newIcon, Sprite portraitImage, Sprite iconImage)
        {
            while (newIcon.checkFullyInitialized() == false)
            {
                yield return null;
            }
            newIcon.setUnitPortrait(portraitImage);
            newIcon.setUnitIcon(iconImage);
        }
    }

}
