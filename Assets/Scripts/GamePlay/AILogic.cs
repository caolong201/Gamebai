using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILogic : MonoBehaviour
{
    private Player aiPlayer;
    private PhomGameManager cardManager;

    private void Start()
    {
        aiPlayer = GetComponent<Player>();
        cardManager = FindObjectOfType<PhomGameManager>();
    }

}