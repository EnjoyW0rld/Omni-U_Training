using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class used to cache the references to the object
/// </summary>
public class ReferenceHandler
{
    private static Dictionary<Type, UnityEngine.Object> _references;

    /// <summary>
    /// Treat it as FindObjectOfType method
    /// Will work only if you have only one instance of the script in the scene
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>If returns null, that means that there is no this object in the current scene</returns>
    public static T GetObject<T>(bool pIncludeInactive = false) where T : UnityEngine.Object
    {
        Type type = typeof(T);
        if (_references == null)
        {
            _references = new Dictionary<Type, UnityEngine.Object>();
            _references.Add(type, GameObject.FindObjectOfType<T>(pIncludeInactive));
        }
        if (_references.ContainsKey(type) && _references[type] == null)
        {
            _references[type] = GameObject.FindObjectOfType<T>(pIncludeInactive);
        }
        return (T)_references[type];
    }
}
