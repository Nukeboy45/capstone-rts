using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Capstone 
{
    public class UnitIconWorld : MonoBehaviour
    {
        private Slider healtBar;
        private Image healthBarColor;
        private Image unitIcon;
        private TextMeshProUGUI aliveModels;

        void Start()
        {
            healtBar = this.GetComponentInChildren<Slider>();
            if (healtBar != null)
            {
                Debug.Log("Not null!");
            }
            aliveModels = this.GetComponentInChildren<TextMeshProUGUI>();
            if (aliveModels != null)
            {
                Debug.Log("Model Text Found!");
            }
            Transform healthBarTransform = this.transform.Find("Canvas/SquadHealth/Health");
            healthBarColor = healthBarTransform.GetComponent<Image>();
            if (healthBarColor != null)
            {
                Debug.Log("Healthbar Found!");
            }

            Transform unitIconTransform = this.transform.Find("Canvas/IconImage");
            unitIcon = unitIconTransform.GetComponent<Image>();
            if (unitIcon != null)
            {
                Debug.Log("Unit icon found!");
            }
        }
        void Update()
        {
            transform.LookAt(Camera.main.transform);
        }

        // ------ Getters / Setters --------------------
        public void setUnitIcon(Sprite icon)
        {
            unitIcon.sprite = icon;
        }

        public void setAliveModels(int modelCount)
        {
            aliveModels.text = modelCount.ToString();
        }

        public void setMaxHealth(float maxHealth)
        {
            healtBar.maxValue = maxHealth;
        }

        public void setCurrentHealth(float health)
        {
            healtBar.value = health;
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