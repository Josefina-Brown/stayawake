using UnityEngine;

[RequireComponent(typeof(Light))]
public class GlitchyLight : MonoBehaviour
{
    private Light lightComponent;

    [Header("Intensity Settings")]
    public float minIntensity = 0f;
    public float maxIntensity = 3f;

    [Header("Timing Settings")]
    public float minFlickerInterval = 0.05f;
    public float maxFlickerInterval = 0.5f;

    [Header("Chance to Turn Off (0–1)")]
    [Range(0f, 1f)]
    public float flickerOffChance = 0.3f;

    [Header("Emission Settings")]
    public string emissionObjectName = "Plane"; // Nombre del objeto hermano con emisión
    public Color emissionColor = Color.white;
    public float emissionIntensity = 1.5f;

    private Material emissionMaterial;
    private static readonly string emissionKeyword = "_EMISSION";

    private void Start()
    {
        lightComponent = GetComponent<Light>();

        // Buscar el objeto hermano "Plane" y obtener su material
        Transform parent = transform.parent;
        if (parent != null)
        {
            Transform plane = parent.Find(emissionObjectName);
            if (plane != null)
            {
                Renderer renderer = plane.GetComponent<Renderer>();
                if (renderer != null)
                {
                    // Instanciar el material para no afectar el original
                    emissionMaterial = renderer.material;
                    EnableEmission(true);
                }
            }
        }

        StartCoroutine(FlickerRoutine());
    }

    private System.Collections.IEnumerator FlickerRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minFlickerInterval, maxFlickerInterval);
            yield return new WaitForSeconds(waitTime);

            bool lightShouldBeOn = Random.value >= flickerOffChance;

            // Cambiar luz
            lightComponent.enabled = lightShouldBeOn;
            if (lightShouldBeOn)
            {
                lightComponent.intensity = Random.Range(minIntensity, maxIntensity);
            }

            // Cambiar emisión
            EnableEmission(lightShouldBeOn);
        }
    }

    private void EnableEmission(bool enable)
    {
        if (emissionMaterial == null) return;

        if (enable)
        {
            emissionMaterial.EnableKeyword(emissionKeyword);
            emissionMaterial.SetColor("_EmissionColor", emissionColor * emissionIntensity);
        }
        else
        {
            emissionMaterial.SetColor("_EmissionColor", Color.black);
            emissionMaterial.DisableKeyword(emissionKeyword);
        }
    }
}
