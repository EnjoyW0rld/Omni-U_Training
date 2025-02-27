using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting.FullSerializer;

/// <summary>
/// Inheriting from the class will enable usability of the RCP system
/// To make specific function to be an RCP function mark it with attribute [MyRCP]
/// Keep in mind that RCP are very abstract and can cause a problem if parameters that are passed are wrong type or
/// in the wrong order
/// </summary>
public class RCPBase
{
    private static Dictionary<string, MethodInfo> methods;
    public string RCPName;
    /// <summary>
    /// Finds and registers methods in the current class that are marked with attribute
    /// </summary>
    public void RegisterMethods()
    {
        if (methods == null) methods = new Dictionary<string, MethodInfo>();
        MethodInfo[] info = this.GetType().GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        foreach (MethodInfo method in info)
        {
            if (method.GetCustomAttribute<MyRCPAttribute>() != null)
            {
                methods.Add(method.Name, method);
            }
        }
    }
    /// <summary>
    /// Function will read the values from NetworkPacket and call the designated function if specified
    /// </summary>
    /// <param name="pPacket"></param>
    public void UseRCP(NetworkPacket pPacket)
    {
        RCPName = pPacket.ReadString();
        Debug.Log("Executing RCP for " + RCPName);
        MethodInfo method = methods[RCPName];
        ParameterInfo[] parameters = method.GetParameters();

        object[] args = null;
        if (parameters != null && parameters.Length > 0)
        {
            args = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                args[i] = pPacket.ReadByType(parameters[i].ParameterType);
                Debug.Log("Args " + args[i].ToString());
            }
        }
        if (method != null)
        {
            method.Invoke(this, args);
        }
    }
}