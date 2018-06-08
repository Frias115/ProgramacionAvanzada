using System;
using UnityEngine;

[Serializable]
public abstract class NetworkData {
    public string ToJson() {
        return JsonUtility.ToJson(this);
    }
}
