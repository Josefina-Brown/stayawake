using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    public float maxEnergy = 100f;
    public float currentEnergy;
    public float energyDrainPerSecond = 1f;

    public delegate void EnergyChanged(float current, float max);
    public event EnergyChanged OnEnergyChanged;

    void Start()
    {
        currentEnergy = maxEnergy;
    }

    void Update()
    {
        currentEnergy -= energyDrainPerSecond * Time.deltaTime;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);

        OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);

        if (currentEnergy <= 0)
        {
            FallAsleep();
        }
    }

    public void RestoreEnergy(float amount)
    {
        currentEnergy += amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
    }

    private void FallAsleep()
    {
        Debug.Log("Te estás quedando dormido...");
        // Acá luego activaremos al SleepHunter
    }
}
