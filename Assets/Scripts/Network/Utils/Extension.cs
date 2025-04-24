
using UnityEngine;

public static class TransformExtensions
{
    public static void DestroyAllChildObjects(this Transform parent)
    {
        // Loop through all children and destroy them
        foreach (Transform child in parent)
        {
            if (child.gameObject.activeSelf) GameObject.Destroy(child.gameObject);
        }
    }
}