using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterActions : MonoBehaviour
{
    public void CallChangeScene()
    {
        NetworkSceneManager scene = new NetworkSceneManager();
        scene._sceneName = "SampleScene";
        NetworkPacket packet = new NetworkPacket();
        packet.Write(scene);
        ServerBehaviour.Instance.ScheduleMessage(packet);
    }
}
