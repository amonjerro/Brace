using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundsHolder : MonoBehaviour
{
    [SerializeField]
    GameObject EmptyRoundPipPrefab;

    [SerializeField]
    Sprite RoundWonPip;

    [SerializeField]
    HorizontalLayoutGroup layoutGroup;

    List<Image> pips;
    private int roundsWon;

    private void Start()
    {
        pips = new List<Image>();
        roundsWon = 0;
    }

    public void Initialize(int pipsToMake)
    {
        for (int i = 0; i < pipsToMake; i++) {
            GameObject go = Instantiate(EmptyRoundPipPrefab, layoutGroup.transform);
            pips.Add(go.GetComponent<Image>());
        }
    }

    public void AddVictory()
    {
        pips[roundsWon].sprite = RoundWonPip; 
        roundsWon++;
    }
}