using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting.FullSerializer;

public class RCPBase
{
    private static Dictionary<string, MethodInfo> methods;
    public string RCPName;
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
    public void UseRCP(NetworkPacket pPacket)
    {
        Debug.Log("Recieved rcp");
        RCPName = pPacket.ReadString();
        MethodInfo method = methods[RCPName];
        if (method != null)
        {
            method.Invoke(this, new object[0]);
        }
    }
    public void UseRCP(string pName)
    {
        MethodInfo method = methods[pName];
        if (method != null)
        {
            method.Invoke(this, new object[0]);
        }
    }
}