using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtension
{
    /// <summary>
    /// This extension method destroys all the children GameObjects.
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static Transform Clear(this Transform transform)
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        return transform;
    }
}