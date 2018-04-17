using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movable : MonoBehaviour {
    public float Speed = 2; // Velocidad de movimiento

    protected virtual void Awake() {
        NextAction();
    }
	
    /// <summary>
    /// Inicia el movimiento a la posición deseada.
    /// </summary>
    /// <param name="destination">Posición a la que nos queremos mover.</param>
    protected virtual void GoTo(Vector3 destination) {
        iTween.MoveTo(gameObject, iTween.Hash("time", (destination - transform.position).magnitude / Speed, "orienttopath", true, "position", destination, "oncomplete", "NextAction", "easetype", iTween.EaseType.linear));
    }

    /// <summary>
    /// Realiza la siguiente acción, en función del estado actual.
    /// </summary>
    public abstract void NextAction();
}

