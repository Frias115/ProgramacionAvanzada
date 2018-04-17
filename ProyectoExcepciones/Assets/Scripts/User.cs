using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class User : Movable {
    public string Name;
    //public Vector3 Address;

    public int CurrentStepIndex;
    public Instruction[] Steps;
    public Instruction CurrentStep {
        get {
            return Steps[CurrentStepIndex];
        }
    }

    protected Animator _anim;
    protected Text _text;

    protected override void Awake() {
        _anim = GetComponent<Animator>();
        _text = transform.GetComponentInChildren<Text>();
        _text.text = Name;
        base.Awake();
	}

    /// <summary>
    /// Realiza la siguiente acción, en función del estado actual.
    /// </summary>
	public override void NextAction() {
        _anim.SetFloat("Speed", 0);
        if (CurrentStepIndex < Steps.Length) {
            switch (CurrentStep.InstructionType) {
                case Instruction.Type.Wait:
                    Invoke("NextAction", CurrentStep.Time);
                    break;
                case Instruction.Type.GoTo:
                    GoTo(CurrentStep.Destination);
                    break;
            }
            CurrentStepIndex++;
        }
	}

    /// <summary>
    /// Inicia el movimiento a la posición deseada.
    /// </summary>
    /// <param name="destination">Posición a la que nos queremos mover.</param>
    protected override void GoTo(Vector3 destination) {
        base.GoTo(destination);
        _anim.SetFloat("Speed", Speed);
    }

}
