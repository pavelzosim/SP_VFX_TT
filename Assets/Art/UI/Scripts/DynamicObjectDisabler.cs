using UnityEngine;
using System.Collections.Generic;

public class DynamicObjectDisabler : MonoBehaviour
{
    // List to hold the objects you want to disable
    public List<GameObject> objectsToDisable = new List<GameObject>();

    // Function to dynamically add objects to the list
    public void AddObject(GameObject obj)
    {
        if (!objectsToDisable.Contains(obj))
        {
            objectsToDisable.Add(obj);
        }
    }

    // Function to dynamically remove objects from the list
    public void RemoveObject(GameObject obj)
    {
        if (objectsToDisable.Contains(obj))
        {
            objectsToDisable.Remove(obj);
        }
    }

    // Function to disable all objects in the list
    public void DisableObjects()
    {
        foreach (var obj in objectsToDisable)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }

    // Function to enable all objects in the list (optional)
    public void EnableObjects()
    {
        foreach (var obj in objectsToDisable)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }
}