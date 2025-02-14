using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;
using System;

public class RCPRegistrator : MonoBehaviour
{
    private static bool _isRegistered;
    // Start is called before the first frame update
    void Start()
    {
        if (_isRegistered) return;
        var l = Assembly.GetAssembly(typeof(RCPBase)).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(RCPBase)));
        foreach (var t in l)
        {
            var a = Activator.CreateInstance(t);
            (a as RCPBase).RegisterMethods();
        }
    }
}
