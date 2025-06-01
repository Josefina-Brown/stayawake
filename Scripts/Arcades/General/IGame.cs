using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
public interface IGame
{
    void StartGame();
    void StopGame();
    void WinGame();
    void LoseGame();

    bool isGameStarted { get; set; }
    int ticketReward { get; set; }
    GameObject noGameScreen { get; set; }
    GameObject winScreen { get; set; }
    GameObject loseScreen { get; set; }

    AudioSource audioSource { get; set; }
    List<AudioClip> gameSounds { get; set; }

    void PlaySound(int index);

    TextMeshPro  ticketRewardText { get; set; }
}
