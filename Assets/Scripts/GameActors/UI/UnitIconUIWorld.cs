using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Capstone {
    public class UnitIconUIWorld : MonoBehaviour
    {
        // Private, Editor-Accessible Variables
        [SerializeField] private Canvas removeEditorCanvas;
        [SerializeField] private CanvasScaler removeEditorScaler;
        [SerializeField] private Slider healtBar;
        [SerializeField] private Image healthBarColor;
        [SerializeField] private Image unitIcon;
        [SerializeField] private TextMeshProUGUI aliveModels;

        // Private Runtime Variable
        private GameObject unitReference;
        private GameObject unitIconPosition;
        private Unit unitComponent;
        [SerializeField] private Camera playerCamera;
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;

        // Icon Movement Code
        private Vector3 currentUIPosition;
        private Vector3 prevCameraPosition;
        private Vector3 prevCameraRotation;
        private Vector3 prevIconPosition;
        private float prevCameraFOV;
        private Coroutine updatePositionCoroutine;
        // Start is called before the first frame update
        void Awake()
        {
            Destroy(removeEditorScaler);
            Destroy(removeEditorCanvas);
        }

        void Start()
        {
            rectTransform = gameObject.GetComponent<RectTransform>();
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            updatePositionCoroutine = StartCoroutine(updateIconPositionLoop());
        }

        private IEnumerator updateIconPositionLoop()
        {
            while (true)
            {
                if (unitIconPosition != null && playerCamera != null)
                {
                    if (prevCameraPosition != playerCamera.transform.position || prevCameraRotation != playerCamera.transform.eulerAngles || prevIconPosition != unitIconPosition.transform.position || prevCameraFOV != playerCamera.fieldOfView)
                    {
                        prevCameraPosition = playerCamera.transform.position;
                        prevCameraRotation = playerCamera.transform.eulerAngles;
                        prevIconPosition = unitIconPosition.transform.position;
                        prevCameraFOV = playerCamera.fieldOfView;
                        float angle = unitFunctions.get2DAngleDifference(playerCamera.transform, unitIconPosition.transform);
                        float distance = Vector3.Distance(playerCamera.transform.position, unitIconPosition.transform.position);
                        float checkingFactor = playerCamera.fieldOfView / Mathf.Exp(distance*0.001f) + 5;

                        if (Mathf.Abs(angle) < checkingFactor && getReferenceUnitComponent().getRevealedIcon() == true)
                        {
                            canvasGroup.alpha = 1;
                            canvasGroup.interactable = true;
                            canvasGroup.blocksRaycasts = true;
                            currentUIPosition = playerCamera.WorldToScreenPoint(unitIconPosition.transform.position);   
                            gameObject.transform.position = currentUIPosition;
                            float scaleFactor = Mathf.Clamp(550f / distance, 5f, 150f);
                            rectTransform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
                        } else {
                            canvasGroup.alpha = 0;
                            canvasGroup.interactable = false;
                            canvasGroup.blocksRaycasts = false;
                        }
                    }
                }
                yield return null;
            }
        }

        public void destroySelf(List<UnitIconUIWorld> iconList)
        {
            iconList.Remove(this);
            StopCoroutine(updatePositionCoroutine);
            Destroy(gameObject);
        }

        // Getters / Setters

        public Slider getHealthBar()
        {
            return healtBar;
        }

        public Image getHealthBarColor()
        {
            return healthBarColor;
        }
        public Image getIcon()
        {
            return unitIcon;
        }
        public Unit getReferenceUnitComponent()
        {
            return unitComponent;
        }

        public GameObject getReferenceUnitGameObject()
        {
            return unitReference;
        }

        public void setReferencePosition(GameObject position)
        {
            unitIconPosition = position;
        }
        
        public void setUnitReference(GameObject unit)
        {
            unitReference = unit;
        }

        public void setMaxHealth(float maxHealth)
        {
            healtBar.maxValue = maxHealth;
        }
        public void setCurrentHealth(float health)
        {
            //Debug.Log("Health set:" + health);
            healtBar.value = health;
        }

        public void setIconTag(string tag)
        {
            this.tag = tag;
        }

        public void setHealthBarColor(UnitIconRender renderType)
        {
            switch (renderType)
            {
                case UnitIconRender.player:
                    healthBarColor.color = new Color(0.22f, 0.58f, 0.77f, 1.0f);
                    break;
                case UnitIconRender.playerTeam:
                    healthBarColor.color = new Color(0.0f, 0.71f, 0.59f, 1.0f);
                    break;
                case UnitIconRender.enemy:
                    healthBarColor.color = new Color(0.66f, 0.0f, 0.0f, 1.0f);
                    break;
            }
        }

        public void setAliveModels(int aliveMembers)
        {
            aliveModels.text = aliveMembers.ToString();
        }
        public void setUnitIcon(Sprite icon)
        {
            unitIcon.sprite = icon;
        }
        public void setReferenceUnitComponent(Unit unit)
        {
            unitComponent = unit;
        }

        public void setPlayerCamera(Camera playerCam)
        {
            playerCamera = playerCam;
        }

        public bool checkFullyInitialized()
        {
            if (healtBar == null || healthBarColor == null || unitIcon == null || aliveModels == null)
            {
                return false;
            }
            else {
                return true;
            }
        }
    }
}
