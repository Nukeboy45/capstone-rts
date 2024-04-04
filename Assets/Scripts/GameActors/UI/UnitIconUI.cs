using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Capstone
{
    public class UnitIconUI : MonoBehaviour
    {
        // Private, Editor-Accessible Variables
        [SerializeField] private Canvas removeEditorCanvas;
        [SerializeField] private CanvasScaler removeEditorScaler;
        [SerializeField] private Slider healtBar;
        [SerializeField] private Image healthBarColor;
        [SerializeField] private Image unitPortrait;
        [SerializeField] private Image unitIcon;
        [SerializeField] private TextMeshProUGUI aliveModels;

        // Private Runtime Variables
        [SerializeField] private IconStatus state = IconStatus.queued;
        [SerializeField] private GameObject selfReference;
        [SerializeField] private Unit referenceUnit;
        // Start is called before the first frame update

        // Temporary for Debug
        private SpawnPoint spawnPoint;

        public void setSpawnPoint(SpawnPoint buildQueue)
        {
            spawnPoint = buildQueue;
        }

        // Update is called once per frame
        void Awake()
        {
            selfReference = this.gameObject;
            Destroy(removeEditorScaler);
            Destroy(removeEditorCanvas);
            selfReference.GetComponent<RectTransform>().localScale = new Vector3(75, 75, 75);
        }

        public void playerClick()
        {
            switch (state)
            {
                case IconStatus.active:
                    if (referenceUnit is Squad)
                    {
                        Squad squadComponent = (Squad)referenceUnit;
                        Selection.squadSelect(squadComponent, GameManager.Instance.player.selected, squadComponent.owner);
                    }
                    break;
                default:
                    spawnPoint.removeFromBuildQueue(this);
                    break;
            }
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

        public void setHealtBarColor(int team)
        {
            // team 0 = same team as player, team 1 = enemy team
            Color color;
            if (team == 0)
                color = new Color(0.22f, 0.58f, 0.77f, 1.0f);
            else if (team == 1)
                color = new Color(0.66f, 0.0f, 0.0f, 1.0f);
            else
                color = new Color();

            healthBarColor.color = color;
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
