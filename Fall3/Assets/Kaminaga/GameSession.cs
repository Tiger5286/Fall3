using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WinnerType
{
    None,
    Player1,
    Player2,
    Draw
}

public class GameSession : MonoBehaviour
{
    public WinnerType _lastWinner {  get; private set; }

    public int _winCountPlayer1 { get; private set; }
    public int _winCountPlayer2 { get; private set; }

    public void SetResult(WinnerType winner)
    {
        _lastWinner = winner;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
