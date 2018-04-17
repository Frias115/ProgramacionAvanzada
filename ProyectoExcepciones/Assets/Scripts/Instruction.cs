using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Instruction {
    public Type InstructionType;
    public Vector3 Destination;
    public float Time;

    public enum Type {
        GoTo, Wait
    }
}
