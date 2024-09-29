using System.Collections.Generic;
using UnityEngine;

public static class HelperMethods
{
    public static bool GetComponentsAtBoxLocation<T>(out List<T> listComponentsAtBoxPosition, Vector2 point, Vector2 size, float angle)
    {
        List<T> componentList = new ();

        Collider2D[] colliders = Physics2D.OverlapBoxAll(point, size, angle);

        foreach (var collider in colliders)
        {
            T component = collider.gameObject.GetComponentInParent<T>();
            if (component != null)
            {
                componentList.Add(component);
                continue; 
            }

            component = collider.gameObject.GetComponentInChildren<T>();
            if (component != null)
            {
                componentList.Add(component);
            }
        }

        listComponentsAtBoxPosition = componentList;
        return componentList.Count > 0;
    }
}
