using UnityEngine;

public interface IGame
{
    void StartGame();
    void StopGame();
    void WinGame();
    void LoseGame();
    public bool isGameStarted { get; set; }
    public int ticketReward { get; set; }
    public GameObject noGameScreen { get; set; }
    public GameObject winScreen { get; set; }
    public GameObject loseScreen { get; set; }
}
