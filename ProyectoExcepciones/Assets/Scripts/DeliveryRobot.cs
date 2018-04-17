using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class DeliveryRobot : Movable {
    public Parcel CurrentParcel; // Paquete que estamos entregando (si existe)
    public Hub[] Hubs; // Lista de hubs de la escena
    public State CurrentState; // Estado actual del repartidor

    protected int _currentHubIndex;
    protected Transform _wheelTransform, _bodyTransform;

    /// <summary>
    /// Hub actual.
    /// </summary>
    /// <value>Hub actual.</value>
    protected Hub _currentHub {
        get {
            return Hubs[_currentHubIndex];
        }
    }

    /// <summary>
    /// ¿Está el destinatario (o remitente si estamos realizando una devolución) del paquete disponible?
    /// </summary>
    /// <value><c>true</c>, En caso de que esté disponible, <c>false</c> si no.</value>
    protected bool _isTargetAvailable {
        get {
            return Vector3.Distance(transform.position, CurrentParcel.Target.transform.position) < 1;
        }
    }

    /// <summary>
    /// ¿Existe la dirección en la que estamos intentando entregar el paquete?
    /// </summary>
    protected bool _isAddressValid {
        get {
            return Physics.OverlapSphere(transform.position, 1f, 1 << Layers.House).Length > 0;
        }
    }

	protected override void Awake() {
        _wheelTransform = transform.Find("WHEELS");
        _bodyTransform = transform.Find("BODY");
        // Buscamos todos los hubs de la escena
        Hubs = FindObjectsOfType<Hub>();

        // Hacemos que nuestro Hub inicial sea el más cercano

        var minDist = float.PositiveInfinity;
        var closest = 0;

        for (int i = 0; i < Hubs.Length; i++) {
            if (Vector3.Distance(transform.position, Hubs[i].transform.position) < minDist) {
                minDist = Vector3.Distance(transform.position, Hubs[i].transform.position);
                closest = i;
            }
        }

        _currentHubIndex = closest;
        CurrentState = State.MustGoToHub;
        base.Awake();
	}

    /// <summary>
    /// Realiza la siguiente acción, en función del estado actual.
    /// </summary>
    public override void NextAction() {
        switch (CurrentState) {
            case State.MustGoToHub:
                // Estamos sin hacer nada, vamos a nuestro hub para ver si hay paquetes
                CurrentState = State.GoingToHub;
                GoTo(_currentHub.transform.position);
                break;
            case State.GoingToHub:
                // Hemos llegado al hub
                ArrivedToHub();
                break;
            case State.DeliveringParcel:
                // Hemos llegado al punto de entrega del paquete
                DeliverCurrentParcel();
                CurrentState = State.GoingToHub;
                GoTo(_currentHub.transform.position);
                break;

        }
    }

    /// <summary>
    /// Inicia el movimiento a la posición deseada.
    /// </summary>
    /// <param name="destination">Posición a la que nos queremos mover.</param>
    protected override void GoTo(Vector3 destination) {
        base.GoTo(destination);
        var distance = (destination - transform.position).magnitude;
        var timeToReachDestination = distance / Speed;
        iTween.RotateBy(_wheelTransform.gameObject, iTween.Hash("time", timeToReachDestination, "amount", Vector3.right * distance / (2 * Mathf.PI), "easetype", iTween.EaseType.linear));
    }

    /// <summary>
    /// Cambia de hub. Debe llamarse a este método cuando lleguemos a nuestro hub para recoger paquetes y no tenga ninguno.
    /// </summary>
    void ChangeHub() {
        _currentHubIndex = (_currentHubIndex + 1) % Hubs.Length;
        CurrentState = State.MustGoToHub;
        NextAction();
    }

    /// <summary>
    /// Se llama cuando llegamos al punto de entrega de un paquete
    /// </summary>
    void DeliverCurrentParcel() {
        try
        {
            var validAddress = _isAddressValid;
            var targetAvailable = _isTargetAvailable;

            CurrentParcel.Attempts++;

            // Comprobamos que el punto de entrega sea válido (existe una casa en el lugar)
            if (validAddress)
            {
                // La dirección es correcta, ahora comprobamos el usuario se encuentre en casa
                if (targetAvailable)
                {
                    // Podemos entregar el paquete
                    CurrentParcel.Deliver();
                    CurrentParcel = null;
                }
                else
                {
                    // El destinatario estaba ausente, no podemos entregar el paquete
                    Debug.LogWarning(gameObject.name + ": Error en la entrega del paquete " + CurrentParcel.name + ". Motivo: Destinatario ausente");
                    throw new RecipientMissingException();
                }
            }
            else
            {
                // Dirección incorrecta
                Debug.LogWarning(gameObject.name + ": Error en la entrega del paquete " + CurrentParcel.name + ". Motivo: Dirección incorrecta");
                throw new InvalidAddressException();
            }
        }
        catch (InvalidAddressException)
        {
            CurrentParcel.AddInvalidAddressIncidence();
            CurrentParcel.MarkAsReturnToSender();
        }
        catch (RecipientMissingException)
        {
            CurrentParcel.AddRecipientMissingIncidence();
        }
    }

    /// <summary>
    /// Se llama cuando llegamos al Hub
    /// </summary>
    void ArrivedToHub() {
        if (CurrentParcel == null) {
            try
            {
                CurrentParcel = _currentHub.RequestParcel();
                CurrentState = State.DeliveringParcel;
                CurrentParcel.transform.parent = transform;
                CurrentParcel.transform.localPosition = new Vector3(0, 1.4f, .75f);
                GoTo(CurrentParcel.TargetAdress);
            }
            catch (NoParcelsAvailableException)
            {
                ChangeHub();
            }

        } else {
            // Dejamos el paquete que llevamos (venimos de intentar entregarlo y no hemos podido)
            _currentHub.StoreParcel(CurrentParcel);
            CurrentParcel = null;
            CurrentState = State.MustGoToHub;
            NextAction();
        }
    }

    /// <summary>
    /// Estados en los que puede estar el repartidor.
    /// </summary>
    public enum State {
        MustGoToHub, GoingToHub, DeliveringParcel
    }

}

