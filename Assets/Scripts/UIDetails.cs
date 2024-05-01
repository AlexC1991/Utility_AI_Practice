using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDetails : MonoBehaviour
{
    [SerializeField] private Slider healthInfo;
    [SerializeField] private Slider hungerInfo;
    [SerializeField] private Slider thirstInfo;
    [SerializeField] private PercentagesOfWants needsOfSlime;
    [SerializeField] private TMP_Text trainOfThoughtText;
    private float healthDeath = 0.0f;
    private float zeroHunger = 0.0f;
    private float zeroThirst = 0.0f;
    private float fullHealth = 1.0f;
    private float fullHunger = 1.0f;
    private float fullThirst = 1.0f;
    public float currentHealth;
    public float currentHunger;
    public float currentThirst;
    private IEnumerator thirstyCoDecrease;
    private IEnumerator hungryCoDecrease;
    private IEnumerator healthCoDecrease;

    private void Awake()
    {
        thirstyCoDecrease = DecreaseThirst();
        hungryCoDecrease = DecreaseHunger();
        healthCoDecrease = DecreaseHealth();
        
        healthInfo.value = 1;
        hungerInfo.value = 1;
        thirstInfo.value = 1;

        currentHealth = healthInfo.value;
        currentHunger = hungerInfo.value;
        currentThirst = thirstInfo.value;
    }
    private void Start()
    {
        StartCoroutine(thirstyCoDecrease);
        StartCoroutine(hungryCoDecrease);
        StartCoroutine(DisplayingCharacterThoughts());
    }
    private void Stop()
    {
        if (currentHunger == zeroHunger)
        {
            StopCoroutine(hungryCoDecrease);
        }
        
        if (currentThirst == zeroThirst)
        {
            StopCoroutine(thirstyCoDecrease);
        }

        if (currentThirst > zeroThirst && currentHunger > zeroThirst)
        {
            StopCoroutine(healthCoDecrease);
        }
    }
    
    IEnumerator DecreaseHealth()
    {
        while (true)
        {
            if (currentThirst == zeroThirst)
            {
                yield return new WaitForSeconds(0.2f);
                currentHealth = HealthSliderDecrease(currentHealth);
                healthInfo.value = currentHealth;
                needsOfSlime.needsWaterValue += 10f * Time.deltaTime;
                needsOfSlime.needsToGetHealth += 20f *  Time.deltaTime;
                needsOfSlime.needsToWalkValue -= 80f *  Time.deltaTime;
                yield return new WaitForSeconds(0.2f);
            }

            if (currentHunger == zeroHunger)
            {
                yield return new WaitForSeconds(0.1f);
                currentHealth = HealthSliderDecrease(currentHealth);
                healthInfo.value = currentHealth;
                needsOfSlime.needsToEatValue += 10f * Time.deltaTime;
                needsOfSlime.needsToGetHealth += 20f * Time.deltaTime;
                needsOfSlime.needsToWalkValue -= 50f * Time.deltaTime;
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
    IEnumerator DecreaseHunger()
    {
        while (true)
        {
            if (currentHunger > 0)
            {
                yield return new WaitForSeconds(0.2f);
                currentHunger= HungerSliderDecrease(currentHunger);
                hungerInfo.value = currentHunger;
                needsOfSlime.needsToEatValue += 20f *  Time.deltaTime;
                needsOfSlime.needsToWalkValue -= 20f *   Time.deltaTime;
                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                StartCoroutine(healthCoDecrease);
                Stop();
            }
            yield return null;
        }
    }
    IEnumerator DecreaseThirst()
    {
        while (true)
        {
            if (currentThirst > 0)
            {
                yield return new WaitForSeconds(0.1f);
                currentThirst = ThirstSliderDecrease(currentThirst);
                thirstInfo.value = currentThirst;
                needsOfSlime.needsWaterValue += 10f *  Time.deltaTime;
                needsOfSlime.needsToWalkValue -= 20f *  Time.deltaTime;
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                StartCoroutine(healthCoDecrease);
                Stop();
            }
            yield return null;
        }
    }


    private float HealthSliderDecrease(float currentValue, float decreaseAmount = 0.1f) {
        
        
        currentValue -= decreaseAmount * Time.deltaTime;
        return Mathf.Clamp(currentValue, healthDeath, fullHealth);
    }
    private float HungerSliderDecrease(float currentValue, float decreaseAmount = 0.1f)
    {
        currentValue -= decreaseAmount * Time.deltaTime;
        return Mathf.Clamp(currentValue, zeroHunger, fullHunger);
    } 
    private float ThirstSliderDecrease(float currentValue, float decreaseAmount = 0.1f)
    {
        currentValue -= decreaseAmount * Time.deltaTime;
        return Mathf.Clamp(currentValue, zeroThirst, fullThirst);
    }

    
    public float HealthSliderIncrease(float currentValue, float increaseAmount = 0.1f)
    {
        return Mathf.Clamp(currentValue + increaseAmount, healthDeath, fullHealth);
    }
    public float HungerSliderIncrease(float currentValue, float increaseAmount = 0.2f)
    {
        return Mathf.Clamp(currentValue + increaseAmount, zeroHunger, fullHunger);
    }
    public float ThirstSliderIncrease(float currentValue, float increaseAmount = 0.2f)
    {
        return Mathf.Clamp(currentValue + increaseAmount, zeroThirst, fullThirst);
    }


    private IEnumerator DisplayingCharacterThoughts()
    {
        while (true)
        {
            if (UtilityAIBrain.WalkingRightNow)
            {
                trainOfThoughtText.text = "Walking Around Exploring";
            }
            if (UtilityAIBrain.EatingRightNow)
            {
                trainOfThoughtText.text = "Feeling Hungry Now";
            }
            if (UtilityAIBrain.DrinkingRightNow)
            {
                trainOfThoughtText.text = "Wanting Something To Drink";
            }
            if (UtilityAIBrain.HealthIsGettingLow)
            {
                trainOfThoughtText.text = "Feel like I need Something Quick";
            }
            else if (!UtilityAIBrain.HealthIsGettingLow && !UtilityAIBrain.DrinkingRightNow && !UtilityAIBrain.EatingRightNow && !UtilityAIBrain.WalkingRightNow)
            {
                trainOfThoughtText.text = "I Don't Know Anymore..";
            }

            yield return null;
        }
    }
}
