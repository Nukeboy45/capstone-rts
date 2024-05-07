using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AddressableAssets;
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
        [SerializeField] private RectTransform boxSelectionGraphic;
        [SerializeField] private Transform cursorPosition;
        [SerializeField] private GameObject pauseMenu;

        // Private Runtime Variables
        private UIState uiState = UIState.defaultMenu;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Sprite blankButtonImage;
        [SerializeField] private List<GameObject> unitIconBarList = new List<GameObject>();
        [SerializeField] private List<UnitIconUIWorld> unitIconWorldList = new List<UnitIconUIWorld>();
        private FactionType faction;

        // UI Update Variables
        private bool unitIconsUpToDate = true;
        
        // Prefabs
        private GameObject emptyUnitIconUI;
        public void Start()
        {
            // Initializing the Empty UnitIconUI Prefab
            emptyUnitIconUI = Resources.Load<GameObject>("Prefabs/UI/UnitIconUIBar");

            updateMenu();

            GameObject fadeInObject = transform.Find("fadeInCanvas").gameObject;

            StartCoroutine(fadeInCamera(fadeInObject));
            // StartCoroutine(updateWorldSpaceIconPositions());
        }

        private IEnumerator fadeInCamera(GameObject fadeInObj) {
            CanvasGroup fadeInCanvasGroup = fadeInObj.GetComponent<CanvasGroup>();
            while (fadeInCanvasGroup.alpha > 0) 
            {
                fadeInCanvasGroup.alpha -= 0.05f;
                yield return new WaitForSecondsRealtime(0.25f);
            }
            Destroy(fadeInObj);
            yield break;
        }
        public void Update()
        {   
            List<GameObject> selected = playerObj.getSelected();
            if (selected.Count == 0)
            {
                uiState = UIState.defaultMenu;
                updateMenu();
            }
            else if (selected.Count == 1)
            {
                if (selected[0].GetComponent<Unit>() != null)
                {
                    Unit baseComponent = selected[0].GetComponent<Unit>();
                    if (baseComponent is Squad)
                    {
                        uiState = UIState.squad;
                        updateMenu();
                    }
                }
                if (selected[0].GetComponent<OwnedStructure>() != null)
                {
                    OwnedStructure baseComponent = selected[0].GetComponent<OwnedStructure>();
                    if (baseComponent is ProductionStructure)
                    {
                        ProductionStructure productionStructureComponent = (ProductionStructure)baseComponent;
                        uiState = UIState.productionStructure;
                        for (int i=0; i <productionStructureComponent.constructableUnits.Count(); i++)
                        {
                            if (productionStructureComponent.constructableUnits[i] != null)
                            {
                                int index = i;
                                buttons[i].GetComponent<Image>().sprite = productionStructureComponent.constructableUnits[i].GetComponent<Unit>().buttonSprite;
                                buttons[i].onClick.RemoveAllListeners();
                                buttons[i].interactable = true;
                                buttons[i].onClick.AddListener(delegate {productionStructureComponent.addToBuildQueue(index);});
                            }
                        }
                        buttonsCleared = false;
                    }
                }
            } 
            else if (playerObj.getSelected().Count > 1)
            {

            }

            if (!unitIconsUpToDate)
            {
                updateUnitIconBarPositions();
            }
        }

        public void showPauseMenu()
        {
            pauseMenu.SetActive(true);
        }

        public void hidePauseMenu()
        {
            pauseMenu.SetActive(false);
        }

        // private IEnumerator updateWorldSpaceIconPositions()
        // {
        //     while (true)
        //     {
        //         if (unitIconWorldList.Count == 0)
        //         {
        //             yield return null;
        //         } else {
        //             if (playerCamera != null)
        //             {
        //                 foreach (UnitIconUIWorld icon in unitIconWorldList)
        //                 {
        //                     icon.updateUIPosition(playerCamera);
        //                 }
        //             }
        //         }
        //         yield return null;
        //     }
        // }

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
            worldIconComponent.setReferencePosition(iconPosition);
            worldIconComponent.setPlayerCamera(playerCamera);
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
                    if (!buttonsCleared)
                        clearButtons();
                    break;
                case UIState.squad:

                    break;

                case UIState.group:

                    break;
            }
        }

        private bool buttonsCleared = true;
        private void clearButtons()
        {
            foreach(Button button in buttons)
            {
                button.GetComponent<Image>().sprite = blankButtonImage;
                button.onClick.RemoveAllListeners();
            }
            buttonsCleared = true;
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
                unitIconBarList[i].transform.localPosition = Vector3.zero;
            }
            unitIconsUpToDate = true;
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

        public RectTransform getBoxSelectionGraphic() { return boxSelectionGraphic; }

        public UnitIconUI addNewUnitIcon(Sprite portraitImage, Sprite iconImage)
        {
            GameObject newIcon = Instantiate(emptyUnitIconUI, Vector3.zero, Quaternion.identity);
            unitIconBarList.Add(newIcon);
            UnitIconUI returnIconUI = newIcon.GetComponent<UnitIconUI>();
            unitIconsUpToDate = false;
            newIcon.SetActive(false);
            StartCoroutine(waitForIconInitialization(returnIconUI, portraitImage, iconImage));
            return returnIconUI;
        }

        public void removeUnitIcon(GameObject removeIcon)
        {
            unitIconBarList.Remove(removeIcon);
            Destroy(removeIcon);
            unitIconsUpToDate = false;
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
            newIcon.gameObject.SetActive(true);
        }
    }

}
