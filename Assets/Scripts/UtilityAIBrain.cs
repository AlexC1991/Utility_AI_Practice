using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class UtilityAIBrain : MonoBehaviour
{
    [Range(0,50)][SerializeField] private float visionRadius;
    [SerializeField] private Animator animator;
    [SerializeField] private PercentagesOfWants needsOfSlime;
    [SerializeField] private UIDetails slimeDetails;
    private NavMeshAgent _agent;
    private float _slimeWalkSpeed;
    private float _randomDistance;
    [SerializeField] private List<ResourceDetails> resourcesFound = new List<ResourceDetails>();
    private float _walkValue;
    private float _eatValue;
    private float _waterValue;
    private float _healthValue;
    public static bool WalkingRightNow;
    public static bool DrinkingRightNow;
    public static bool EatingRightNow;
    public static bool HealthIsGettingLow;
    private ResourceDetails _rDetails;
    private bool _isIdleCoroutineRunning;
    private Animator _slimeAnimator;

    private enum AIState
    {
        Idle,
        MovingToResource,
        AtResource,
        Eating,
        FixingHealth,
        Drinking,
        Walking
    }
    
    private void Start()
    {
        SphereCollider sphereCollider = gameObject.GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        sphereCollider.radius = visionRadius;
        _agent = gameObject.GetComponent<NavMeshAgent>();
        _slimeAnimator = GetComponent<Animator>();
        needsOfSlime.needsToWalkValue = Mathf.Clamp(needsOfSlime.needsToWalkValue, 0, 100);
        needsOfSlime.needsToEatValue = Mathf.Clamp(needsOfSlime.needsToEatValue, 0, 100);
        needsOfSlime.needsToGetHealth = Mathf.Clamp(needsOfSlime.needsToGetHealth, 0, 100);
        needsOfSlime.needsWaterValue = Mathf.Clamp(needsOfSlime.needsWaterValue, 0, 100);
        
        needsOfSlime.needsToWalkValue = 100;
        needsOfSlime.needsToEatValue = Random.Range(0, 5);
        needsOfSlime.needsToGetHealth = Random.Range(0, 5);
        needsOfSlime.needsWaterValue = Random.Range(0, 5);
        StartWalking();
    }

    private void Update()
    {
        UpdateNeedsValues();

        if (IsBusy()) return; 
        
        if (_currentState == AIState.Idle)
        {
            StartCoroutine(IdleCurrentState());
        }
        
        if (_eatValue > _walkValue)
        {
            List<ResourceDetails> foundResource = resourcesFound.FindAll(resource => resource.nameOfResource.Contains("Food_Meat"));
            
            if (foundResource.Count > 0)
            {
                StartEating();
                WalkingRightNow = false;
                EatingRightNow = true;
                DrinkingRightNow = false;
                HealthIsGettingLow = false;
            }
            else
            {
                StartEating();
            }
        }
        else if (_waterValue > _walkValue)
        {
            List<ResourceDetails> foundResource = resourcesFound.FindAll(resource => resource.nameOfResource.Contains("WaterSource"));

            if (foundResource.Count > 0)
            {
                StartDrinking();
                WalkingRightNow = false;
                EatingRightNow = false;
                DrinkingRightNow = true;
                HealthIsGettingLow = false;
            }
            else
            {
                StartDrinking();
            }
        }
        else if (_waterValue > _eatValue && _waterValue > _walkValue)
        {
            List<ResourceDetails> foundResource = resourcesFound.FindAll(resource => resource.nameOfResource.Contains("WaterSource"));
               
            if (foundResource.Count > 0)
            {
                StartDrinking();
                WalkingRightNow = false;
                EatingRightNow = false;
                DrinkingRightNow = true;
                HealthIsGettingLow = false;
            }
            else
            {
                StartDrinking();
            }
        }
        else if (_eatValue > _walkValue && _eatValue > _waterValue)
        {
            List<ResourceDetails> foundResource = resourcesFound.FindAll(resource => resource.nameOfResource.Contains("Food_Meat"));
            
            if (foundResource.Count > 0)
            {
                StartEating();
                WalkingRightNow = false;
                EatingRightNow = true;
                DrinkingRightNow = false;
                HealthIsGettingLow = false;
            }
            else
            {
                StartEating();
            }
            
        }
        else if (_healthValue > 50)
        {
            List<ResourceDetails> foundResourceMeat = resourcesFound.FindAll(resource => resource.nameOfResource.Contains("Food_Meat"));
            List<ResourceDetails> foundResourceWater = resourcesFound.FindAll(resource => resource.nameOfResource.Contains("WaterSource"));

            if (foundResourceMeat.Count > 0 && foundResourceWater.Count > 0)
            {
                HandleHealth();
                WalkingRightNow = false;
                EatingRightNow = false;
                DrinkingRightNow = false;
                HealthIsGettingLow = true;
            }
        }
    }
    
    IEnumerator IdleCurrentState()
    {
        if (_currentState == AIState.Idle)
        {
            yield return new WaitForSeconds(Random.Range(2, 5)); 
            StartWalking();
        }
    }
    
    private void UpdateNeedsValues()
    {
        _walkValue = needsOfSlime.needsToWalkValue / 100f * 100;
        _eatValue = needsOfSlime.needsToEatValue / 100f * 100;
        _healthValue = needsOfSlime.needsToGetHealth / 100f * 100;
        _waterValue = needsOfSlime.needsWaterValue / 100f * 100;
    }
    
    private void StartEating()
    {
        if (_currentState != AIState.Eating)
        {
            _currentState = AIState.Eating;
            StartCoroutine(EatingFunction());
        }
    }
    
    private void StartDrinking()
    {
        if (_currentState != AIState.Drinking)
        {
            _currentState = AIState.Drinking;
            StartCoroutine(DrinkingFunction());
        }
    }
    
    private void StartWalking()
    {
        WalkingRightNow = true;
        EatingRightNow = false;
        DrinkingRightNow = false;
        HealthIsGettingLow = false;
        
        if (_currentState != AIState.Walking && !IsBusy())
        {
            _currentState = AIState.Walking;
            StartCoroutine(RandomWalk());
        }
    }
    
    private void HandleHealth()
    {
        if (_currentState != AIState.FixingHealth)
        {
            _currentState = AIState.FixingHealth;
            StartCoroutine(FixYourHealth());
        }
    }
    
    IEnumerator RandomWalk()
    {
        _agent.speed = 4;
        _slimeAnimator.SetTrigger("Walking");
        float blendN = Random.Range(0, 1);
        _slimeAnimator.SetFloat("Blend", blendN, 0.1f, Time.deltaTime);
        
        while (_currentState == AIState.Walking)
        {
            SetRandomDestination();
            yield return new WaitForSeconds(Random.Range(10, 30));
            _agent.speed = 0;
            DebugHelper.Log("Idle Trigger Activated Here: " );
            _slimeAnimator.SetTrigger("Idle");
            _currentState = AIState.Idle;
        }
    }

    private AIState _currentState = AIState.Idle;
    
    private void SetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 200; 
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out var hit, 200, NavMesh.AllAreas))
        {
            _agent.SetDestination(hit.position);
        }
    }
    
    private bool IsBusy()
    {
        return EatingRightNow || DrinkingRightNow;
    }

    IEnumerator EatingFunction()
    {
        List<ResourceDetails> foundResource = resourcesFound.FindAll(resource => resource.nameOfResource.Contains("Food_Meat"));

        if (foundResource.Count > 0)
        {
            Debug.Log("Heading For Food Now");
            _currentState = AIState.MovingToResource;
            
            ResourceDetails selectedResource = foundResource[Random.Range(0, foundResource.Count)];
            
            Debug.Log(selectedResource.locationOfResource);
    
            _agent.SetDestination(selectedResource.locationOfResource);
    
            yield return new WaitWhile(() => _agent.pathPending);
            yield return new WaitUntil(() => _agent.remainingDistance <= _agent.stoppingDistance);
        
            _currentState = AIState.AtResource;
        
            yield return new WaitForSeconds(0.2f);
        
            _currentState = AIState.Eating;
            slimeDetails.currentHunger = slimeDetails.HungerSliderIncrease(slimeDetails.currentHunger);
            float currentValueHalf = needsOfSlime.needsToEatValue / 2;
            needsOfSlime.needsToEatValue = Mathf.Clamp(needsOfSlime.needsToEatValue - currentValueHalf, 0, 100);
            needsOfSlime.needsToWalkValue = Mathf.Clamp(needsOfSlime.needsToWalkValue + 50, 0, 100);
        
            foundResource.Remove(selectedResource);
            resourcesFound.Remove(selectedResource);
            yield return new WaitForSeconds(0.1f);

            _agent.speed = 0;
            DebugHelper.Log("Idle Trigger Activated Here: " );
            _slimeAnimator.SetTrigger("Idle");
            _currentState = AIState.Idle;
            yield return new WaitForSeconds(0.2f);
            StartWalking();
        }
        else
        {
            needsOfSlime.needsToEatValue = Mathf.Clamp(needsOfSlime.needsToEatValue - 5, 0, 100);
            needsOfSlime.needsToWalkValue = Mathf.Clamp(needsOfSlime.needsToWalkValue + 20, 0, 100);
           
            _agent.speed = 0;
            DebugHelper.Log("Idle Trigger Activated Here: " );
            _slimeAnimator.SetTrigger("Idle");
            _currentState = AIState.Idle;
            yield return new WaitForSeconds(0.2f);
            StartWalking();
        }
    }
    
    IEnumerator DrinkingFunction()
    {
        List<ResourceDetails> foundResource = resourcesFound.FindAll(resource => resource.nameOfResource.Contains("WaterSource"));

        if (foundResource.Count > 0)
        {
            _currentState = AIState.MovingToResource;
            ResourceDetails selectedResource = foundResource[Random.Range(0, foundResource.Count)];
        
            _agent.SetDestination(selectedResource.locationOfResource);
    
            yield return new WaitWhile(() => _agent.pathPending);
            yield return new WaitUntil(() => _agent.remainingDistance <= _agent.stoppingDistance);
        
            _currentState = AIState.AtResource;
        
            yield return new WaitForSeconds(0.2f);
        
            _currentState = AIState.Drinking;
            slimeDetails.currentThirst = slimeDetails.ThirstSliderIncrease(slimeDetails.currentThirst);
            float currentValueHalf = needsOfSlime.needsWaterValue / 2;
            needsOfSlime.needsWaterValue = Mathf.Clamp(needsOfSlime.needsWaterValue - currentValueHalf, 0, 100);
            needsOfSlime.needsToWalkValue = Mathf.Clamp(needsOfSlime.needsToWalkValue + 60, 0, 100);
            foundResource.Remove(selectedResource);
            resourcesFound.Remove(selectedResource);
            yield return new WaitForSeconds(0.1f);
            
            _agent.speed = 0;
            DebugHelper.Log("Idle Trigger Activated Here: " );
            _slimeAnimator.SetTrigger("Idle");
            _currentState = AIState.Idle;
            yield return new WaitForSeconds(0.2f);
            StartWalking();
        }
        else
        {
            needsOfSlime.needsWaterValue = Mathf.Clamp(needsOfSlime.needsWaterValue - 5, 0, 100);
            needsOfSlime.needsToWalkValue = Mathf.Clamp(needsOfSlime.needsToWalkValue + 20, 0, 100);

            _agent.speed = 0;
            DebugHelper.Log("Idle Trigger Activated Here: " );
            _slimeAnimator.SetTrigger("Idle");
            _currentState = AIState.Idle;
            yield return new WaitForSeconds(0.2f);
            StartWalking();
        }
    }
    
    IEnumerator FixYourHealth()
    {
        int whatShouldIDo = Random.Range(1, 2);

        if (whatShouldIDo == 1)
        {
            _currentState = AIState.FixingHealth;
            StartDrinking();
            needsOfSlime.needsToGetHealth = Mathf.Clamp(needsOfSlime.needsToGetHealth -20, 0, 100);
            slimeDetails.currentHealth = slimeDetails.HealthSliderIncrease(slimeDetails.currentHealth);
            _healthValue = -20;
        }

        if (whatShouldIDo == 2)
        {
            _currentState = AIState.FixingHealth;
            StartEating();
            needsOfSlime.needsToGetHealth = Mathf.Clamp(needsOfSlime.needsToGetHealth -20, 0, 100);
            slimeDetails.currentHealth = slimeDetails.HealthSliderIncrease(slimeDetails.currentHealth);
        }
        
        yield break;
    }
    
    private bool ContainsResource(List<ResourceDetails> list, ResourceDetails item)
    {
        return list.Any(x => x.nameOfResource == item.nameOfResource && x.locationOfResource == item.locationOfResource);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water") || other.CompareTag("Meat"))
        {
            ResourceDetails rDetails = new ResourceDetails(other.name, other.transform.position);
            if (!ContainsResource(resourcesFound, rDetails))
            {
                resourcesFound.Add(rDetails);
            }
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (_agent == enabled)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, visionRadius);
        }
    }
}
