using UnityEngine;
[System.Serializable]  // This attribute makes the class visible in the Unity Inspector.
public class ResourceDetails
{
    public string nameOfResource;
    public Vector3 locationOfResource;

    // Constructor to initialize new resources easily
    public ResourceDetails(string name, Vector3 location)
    {
        nameOfResource = name;
        locationOfResource = location;
    }
}
