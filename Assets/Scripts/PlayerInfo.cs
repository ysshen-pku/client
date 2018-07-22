using UnityEngine;
using System;

[Serializable]
public class PlayerInfo
{
    [SerializeField]
    public int id;
    [SerializeField]
    public Vector3 pos;
    [SerializeField]
    public Vector3 rot;
    [SerializeField]
    public int s;

    // Given JSON input:
    // {"name":"Dr Charles","lives":3,"health":0.8}
    // this example will return a PlayerInfo object with
    // name == "Dr Charles", lives == 3, and health == 0.8f.
}
