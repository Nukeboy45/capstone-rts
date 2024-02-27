using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Capstone {
    public class PlayerUI : MonoBehaviour
    {
        public Player playerObj; 
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;

        public void Start()
        {
            
        }
        public void Update()
        {
            if (playerObj.selected.Count == 1)
            {
                
            }
        }
        public void Button1()
        {
            Debug.Log("Button1 Pressed!");
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

        public void setPlayerObj(Player player)
        {
            playerObj = player;
        }
    }

}
