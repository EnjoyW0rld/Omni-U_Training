using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GeneralRCP : MonoBehaviour
{
    private static Dictionary<string, MethodInfo> _methods;

    void Start()
    {
        _methods = new Dictionary<string, MethodInfo>();
        var types = Assembly.GetExecutingAssembly().GetTypes();
        for (int i = 0; i < types.Length; i++)
        {
            MethodInfo[] info = types[i].GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            foreach (MethodInfo method in info)
            {
                if (method.GetCustomAttribute<MyRCPAttribute>() != null)
                {
                    _methods.Add(method.Name, method);
                }
            }
        }
    }
    public static void UseRCP(string pName)
    {
        if (_methods == null || _methods.Count == 0) return;
        MethodInfo method = _methods[pName];
        if (method == null) return;
        Type classType = method.DeclaringType;
        UnityEngine.Object methodObject;
        if (classType == typeof(ServerBehaviour))
        {
            methodObject = ServerBehaviour.Instance;
        }
        else if (classType == typeof(ClientBehaviour))
        {
            methodObject = ClientBehaviour.Instance;
        }
        else
        {
            methodObject = ReferenceHandler.GetObject(method.DeclaringType, true);
        }
        method.Invoke(methodObject, null);
    }
}
public class RCPInvokeContainer : NetworkObject
{
    public string MethodName;
    public RCPInvokeContainer() { }
    public RCPInvokeContainer(string methodName)
    {
        MethodName = methodName;
    }

    public override void DeSerialize(NetworkPacket pPacket)
    {
        MethodName = pPacket.ReadString();
    }

    public override void Serialize(NetworkPacket pPacket)
    {
        pPacket.WriteString(MethodName);
    }

    public override void Use()
    {
        GeneralRCP.UseRCP(MethodName);
    }
}
