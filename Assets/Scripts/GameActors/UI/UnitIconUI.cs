using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Capstone
{
    public class UnitIconUI : MonoBehaviour
    {
        private Slider healtBar;
        private Image healthBarColor;
        private Image unitPortrait;
        private Image unitIcon;
        private TextMeshProUGUI aliveModels;
        private IconStatus state = IconStatus.queued;
        private GameObject selfReference;
        private Unit referenceUnit;
        // Start is called before the first frame update
        void Start()
        {
            healtBar = this.GetComponentInChildren<Slider>();
            if (healtBar != null)
            {

            }
            aliveModels = this.GetComponentInChildren<TextMeshProUGUI>();
            if (aliveModels != null)
            {

            }
            Transform healthBarTransform = this.transform.Find("SquadHealth/Health");
            healthBarColor = healthBarTransform.GetComponent<Image>();
            if (healthBarColor != null)
            {

            }

            Transform unitPortraitTransform = this.transform.Find("PortraitImage");
            unitPortrait = unitPortraitTransform.GetComponent<Image>();
            if (unitPortrait != null)
            {

            }

            Transform unitIconTransform = this.transform.Find("IconImage");
            unitIcon = unitIconTransform.GetComponent<Image>();
            if (unitIcon != null)
            {

            }
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

        public Image getPortrait()
        {
            return unitPortrait;
        }

        public Image getIcon()
        {
            return unitIcon;
        }

        public void setMaxHealth(float maxHealth)
        {
            healtBar.maxValue = maxHealth;
        }
        public void setCurrentHealth(float health)
        {
            healtBar.value = health;
        }

        public void setHealtBarColor(Color newColor)
        {
            healthBarColor.color = newColor;
        }

        public void setAliveModels(int aliveMembers)
        {
            aliveModels.text = aliveMembers.ToString();
        }

        public void setIconStatus(IconStatus status)
        {
            state = status;
        }

        public void setUnitPortrait(Sprite portrait)
        {
            unitPortrait.sprite = portrait;
        }
        public void setUnitIcon(Sprite icon)
        {
            unitIcon.sprite = icon;
        }

        public void setReferenceUnit(Unit unit)
        {
            referenceUnit = unit;
        }

        public bool checkFullyInitialized()
        {
            if (healtBar == null || healthBarColor == null || unitPortrait == null || unitIcon == null || aliveModels == null)
            {
                return false;
            }
            else {
                return true;
            }
        }
    }
}
