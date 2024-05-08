using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Capstone {
    public class WinConditionCanvas : MonoBehaviour
    {
            public bool playerTeamVictory;
            public FactionType playerFaction = FactionType.none;

            [Header("Canvas Elements")]
            [SerializeField] Image[] postcardImages;
            [SerializeField] TextMeshProUGUI winConditionText;

            [Header("Postcard Library")]
            [SerializeField] Sprite[] cenVictorySprites;
            [SerializeField] Sprite[] cenDefeatSprites;
            [SerializeField] Sprite[] entVictorySprites;
            [SerializeField] Sprite[] entDefeatSprites;
            void Start()
            {
                GetComponent<Animator>().enabled = false;
                StartCoroutine(playAnimationWithConditions());
            }

            private IEnumerator playAnimationWithConditions()
            {
                while (playerFaction == FactionType.none)
                    yield return null;
                getRandomPostcards();
                GetComponent<Animator>().enabled = true;
                StartCoroutine(typeWriterWinText());
                yield break;
            }

            private IEnumerator typeWriterWinText()
            {
                while (winConditionText.color.a == 0)
                    yield return null;
                if (playerTeamVictory)
                {
                    int i = 0;
                    char[] victoryChars = new char[7]{'v','i','c','t','o','r','y'};
                    while (winConditionText.text.Length < victoryChars.Length)
                    {
                        winConditionText.text += victoryChars[i];
                        i++;
                        yield return new WaitForSecondsRealtime(0.25f);
                    }
                } else {
                    int i = 0;
                    char[] defeatChars = new char[6]{'d','e','f','e','a','t'};
                    while (winConditionText.text.Length < defeatChars.Length)
                    {
                        winConditionText.text += defeatChars[i];
                        i++;
                        yield return new WaitForSecondsRealtime(0.25f);
                    }
                }
                yield break;
            }

            private void getRandomPostcards()
            {
                List<int> postcardIndexes = getRandomIntegersInRange(4);
                switch (playerFaction)
                {
                    case FactionType.centralPowers:
                        if (playerTeamVictory)
                        {
                            for (int i=0; i<postcardImages.Length;i++)
                            {
                                postcardImages[i].sprite = cenVictorySprites[postcardIndexes[i]];
                            }
                        }
                        else
                        {
                            for (int i=0; i<postcardImages.Length;i++)
                            {
                                Debug.Log("i: " + i);
                                postcardImages[i].sprite = cenDefeatSprites[postcardIndexes[i]];
                            }
                        }
                        break;
                    case FactionType.ententeForces:
                        if (playerTeamVictory)
                        {
                            for (int i=0; i<postcardImages.Length;i++)
                            {
                                postcardImages[i].sprite = entVictorySprites[postcardIndexes[i]];
                            }
                        }
                        else
                        {
                            for (int i=0; i<postcardImages.Length;i++)
                            {
                                postcardImages[i].sprite = entDefeatSprites[postcardIndexes[i]];
                            }
                        }
                        break;
                }
            }
            public void quitMatch()
            {
                SceneManager.LoadScene("MainMenu");
            }
            private List<int> getRandomIntegersInRange(int range)
            {
                List<int> newRandList = new List<int>();
                while (newRandList.Count < range)
                {
                    int newInt = Random.Range(0, range);
                    if (!newRandList.Contains(newInt))
                    {   
                        Debug.Log(newInt);
                        newRandList.Add(newInt);
                    }
                }
                return newRandList;
            }
    }
}
