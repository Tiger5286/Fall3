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
        switch (winner)
        {
            case WinnerType.None:
                break;
            case WinnerType.Player1:
                _winCountPlayer1++;
                break;
            case WinnerType.Player2:
                _winCountPlayer2++;
                break;
            case WinnerType.Draw:
                break;
            default:
                break;
        }

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
