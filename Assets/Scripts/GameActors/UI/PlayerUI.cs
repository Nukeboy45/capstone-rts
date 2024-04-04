using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Capstone {
    public class PlayerUI : MonoBehaviour
    {
        // public / editor variables
        public Player playerObj; 

        // Private, Editor-Accessible Variables
        [SerializeField] private Button[] buttons;
        [SerializeField] private List<ScoreBarList> scoreBars = new List<ScoreBarList>();
        [SerializeField] private List<GameObject> unitIconPositions = new List<GameObject>();
        [SerializeField] private GameObject worldSpaceIconsParent;

        // Private Runtime Variables
        private UIState uiState = UIState.defaultMenu;
        [SerializeField] private Camera playerCamera;
        private Dictionary<String, Sprite> buttonImages = new Dictionary<string, Sprite>();
        [SerializeField] private List<GameObject> unitIconBarList = new List<GameObject>();
        [SerializeField] private List<UnitIconUIWorld> unitIconWorldList = new List<UnitIconUIWorld>();
        private FactionType faction;
        
        // Prefabs
        private GameObject emptyUnitIconUI;
        public void Start()
        {
            switch (faction)
            {
                case (FactionType.centralPowers):
                    // Menu and Unit Construction buttons - Central Powers
                    if (!buttonImages.ContainsKey("ausRifle"))
                        buttonImages.Add("ausRifle", Resources.Load<Sprite>("Art/ui/uiButtonAusRifle"));
                    break;
            }
            // Generic Button Images
            buttonImages.Add("retreat", Resources.Load<Sprite>("Art/ui/uiButtonRetreat"));

            // Initializing the Empty UnitIconUI Prefab
            emptyUnitIconUI = Resources.Load<GameObject>("Prefabs/UI/UnitIconUIBar");

            updateMenu();

            StartCoroutine(updateWorldSpaceIconPositions());
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
                        playerObj.spawnPoint.addToBuildQueue(Resources.Load<GameObject>("Prefabs/Units/Infantry Squads/ausRifleSquad"));
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

        private IEnumerator updateWorldSpaceIconPositions()
        {
            while (true)
            {
                if (unitIconWorldList.Count == 0)
                {
                    yield return null;
                } else {
                    if (playerCamera != null)
                    {
                        foreach (UnitIconUIWorld icon in unitIconWorldList)
                        {
                            icon.updateUIPosition(playerCamera);
                        }
                    }
                }
                yield return null;
            }
        }

        public UnitIconUIWorld spawnWorldSpaceUnitIcon(GameObject iconPrefab, GameObject iconPosition)
        {
            float distance = Vector3.Distance(playerCamera.transform.position, iconPosition.transform.position);
            Vector3 screenPosition = playerCamera.WorldToScreenPoint(iconPosition.transform.position);
            GameObject newIcon = Instantiate(iconPrefab);
            newIcon.transform.SetParent(worldSpaceIconsParent.transform);
            newIcon.transform.position = screenPosition;
            float scaleFactor = Mathf.Clamp(500f / distance, 1f, 130f);
            newIcon.GetComponent<RectTransform>().localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

            UnitIconUIWorld worldIconComponent = newIcon.GetComponent<UnitIconUIWorld>();
            worldIconComponent.setIconObject(newIcon);
            worldIconComponent.setReferencePosition(iconPosition);
            unitIconWorldList.Add(worldIconComponent);
            
            return worldIconComponent;
        }

        public void removeWorldSpaceUnitIcon(UnitIconUIWorld unitIconUIWorld)
        {
            unitIconUIWorld.destroySelf(unitIconWorldList);
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
            for (int i = 0; i < unitIconBarList.Count; i++)
            {
                unitIconBarList[i].transform.SetParent(unitIconPositions[i].transform, false);
                //Debug.Log("Updated Parent!");
                unitIconBarList[i].transform.localPosition = Vector3.zero;
            }
        }

        public GameObject iconCheck;
        public GameObject checkMouseOverWorldIcon(Vector3 mousePosition, string checkTag)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = mousePosition;
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResults);

            if (raycastResults.Count > 0)
            {
                foreach (RaycastResult result in raycastResults)
                    {
                    GameObject hitObject = result.gameObject;
                    iconCheck = hitObject;
                    List<GameObject> iconsList = unitIconWorldList.Select(iconComp => iconComp.gameObject).ToList();
                    if (hitObject.GetComponent<GetParentIcon>() != null)
                        hitObject = hitObject.GetComponent<GetParentIcon>().getParentIcon();
                        iconCheck = hitObject;
                    if (iconsList.Contains(hitObject) && hitObject.CompareTag(checkTag))
                    {
                        return hitObject;
                    }
                }
            }
            return null;
        }

        public GameObject checkMouseOverUIIcon(Vector3 mousePosition, string checkTag)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = mousePosition;
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResults);

            if (raycastResults.Count > 0)
            {
                foreach (RaycastResult result in raycastResults)
                    {
                    GameObject hitObject = result.gameObject;
                    iconCheck = hitObject;
                    List<GameObject> iconsList = unitIconBarList.Select(iconComp => iconComp.gameObject).ToList();
                    if (hitObject.GetComponent<GetParentIcon>() != null)
                        if (hitObject.GetComponent<GetParentIcon>() != null)
                            hitObject = hitObject.GetComponent<GetParentIcon>().getParentIcon();

                        iconCheck = hitObject;
                    if (iconsList.Contains(hitObject) && hitObject.CompareTag(checkTag))
                    {
                        return hitObject;
                    }
                }
            }
            return null;
        }
        
        // ---------- Getters / Setters / Modification Functions --------------
        public void setPlayerObj(Player player) { playerObj = player; }

        public void setPlayerCamera(Camera playerCam) { playerCamera = playerCam; }

        public void setFaction(FactionType factionType) { faction = factionType; }

        public UnitIconUI addNewUnitIcon(Sprite portraitImage, Sprite iconImage)
        {
            GameObject newIcon = Instantiate(emptyUnitIconUI, Vector3.zero, Quaternion.identity);
            unitIconBarList.Add(newIcon);
            UnitIconUI returnIconUI = newIcon.GetComponent<UnitIconUI>();
            StartCoroutine(waitForIconInitialization(returnIconUI, portraitImage, iconImage));
            updateUnitIconBarPositions();
            return returnIconUI;
        }

        public void removeUnitIcon(GameObject removeIcon)
        {
            unitIconBarList.Remove(removeIcon);
            Destroy(removeIcon);
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
