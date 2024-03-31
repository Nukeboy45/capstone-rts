using System;
using TMPro;
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

        // Private Runtime Variables
        private GameObject selfReference;
        private GameObject unitIconReference;
        private Unit unitComponent;
        // Start is called before the first frame update
        void Awake()
        {
            Destroy(removeEditorScaler);
            Destroy(removeEditorCanvas);
        }

        // Update is called once per frame
        void Update()
        {
            
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

        public GameObject getReferenceUnit()
        {
            return unitIconReference;
        }

        public Unit getReferenceUnitComponent()
        {
            return unitComponent;
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
                    healthBarColor.color = new Color(0.0f, 0.04f, 0.42f, 1.0f);
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

        public void setReferenceObj(GameObject reference)
        {
            unitIconReference = reference;
        }

        public void setReferenceUnitComponent(Unit unit)
        {
            unitComponent = unit;
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
