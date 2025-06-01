using UnityEngine;
using System.Collections;

public class PlayerEnergyManager : MonoBehaviour
{
    public Animator anim;
    public Animator eyesanim;
    public float maxEnergy = 100f;
    public float currentEnergy;
    public float energyDrainPerSecond = 1f;

    public GameObject loseScreen;

    public delegate void EnergyChanged(float current, float max);
    public event EnergyChanged OnEnergyChanged;

    private bool isSleeping = false;

 void Start ()
{
    Time.timeScale = 1;
}

    void Update()
    {
        if (isSleeping) return;

        currentEnergy -= energyDrainPerSecond * Time.deltaTime;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);

        OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);

        if (currentEnergy <= 0 && !isSleeping)
        {
            FallAsleep();
        }

        eyesanim.SetFloat("energyConsume", currentEnergy);

    }

    public void RestoreEnergy(float amount)
    {
        if (isSleeping) return;

        currentEnergy += amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
    }

    private void FallAsleep()
    {
        isSleeping = true;

        if (anim != null)
        {
            anim.SetBool("isSleep", true);
            eyesanim.SetBool("isSleep", true);
        }

        StartCoroutine(ShowLoseScreenAfterDelay(1f));
    }

    private IEnumerator ShowLoseScreenAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (loseScreen != null)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            loseScreen.SetActive(true);
        }
    }
}
