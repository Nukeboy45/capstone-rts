using UnityEngine;
using System;
using UnityEngine.UI;

namespace Capstone 
{

    public class FogLayerManager : MonoBehaviour
    {
        // 
        public static FogLayerManager instance;

        // Private, Editor-Accessible Variables
        [SerializeField] private RenderTexture fogTexture;
        [SerializeField] private Camera fogCamera;

        // Private Runtime Variables
        private Texture2D tex;
        private int playerTeam;

        void Awake()
        {
            createFogLayerManagerSingleton();
        }

        private void Update()
        {

        }

        private void createFogLayerManagerSingleton()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else 
                Destroy(gameObject);
        }

        public bool isInFog(Vector3 position)
        {
            Vector3 cameraTranslatedCoords = fogCamera.WorldToViewportPoint(position);
            Vector2 objectCoord = new Vector2(cameraTranslatedCoords.x, cameraTranslatedCoords.y);

            RenderTexture.active = fogTexture;
            Texture2D tex = new Texture2D(fogTexture.width, fogTexture.height);
            tex.ReadPixels(new Rect(0,0, fogTexture.width, fogTexture.height), 0, 0);
            tex.Apply();
            RenderTexture.active = null;
            if (tex != null)
            {
                Color sampledColor = tex.GetPixelBilinear(objectCoord.x, objectCoord.y);
                // Returns True if it is not fully transparent (i.e - in fog)*/
                // Destroy texture to prevent buildup of unused, outdated textures for processing
                Destroy(tex);
                return sampledColor.a != 0f;
            }
            

            // Destroy texture to prevent buildup of unused, outdated textures for processing
            Destroy(tex);
            return true;
        }

        // --- Getters / Setter Methods ---
        
        public void setPlayerTeam(int newTeam)
        {
            playerTeam = newTeam;  
        }

        public int getPlayerTeam()
        {
            return playerTeam;
        }

    }
}