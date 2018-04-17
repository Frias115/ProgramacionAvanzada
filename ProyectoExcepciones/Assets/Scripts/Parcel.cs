using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Parcel : MonoBehaviour {
    public const int MAX_ATTEMPTS = 2;
    public User Sender; // Remitente
    public User Recipient; // Destinatario
    public Vector3 SenderAddress; // Dirección del remitente
    public Vector3 RecipientAddress; // Dirección del destinatario
    public bool Done; // Almacena si hemos terminado con la gestión de este envío o no
    public List<string> TrackingStatus; //Información de seguimiento del paquete
    public int Attempts; // Veces que se ha intentado repartir el paquete al destinatario

    public Vector3 TargetAdress { // Dirección a la que llevar el paquete
        get {
            return Attempts < MAX_ATTEMPTS ? RecipientAddress : SenderAddress;
        }
    }

    public User Target { // A quién vamos a intentar llevar el paquete
        get {
            return Attempts < MAX_ATTEMPTS ? Recipient : Sender;
        }
    }

    protected Text _text;

    protected void Awake() {
        _text = transform.GetComponentInChildren<Text>();
        _text.text = Sender.Name + ">" + Recipient.Name;
        _text.color = Color.blue;

        TrackingStatus = new List<string>();
        TrackingStatus.Add(Time.time.ToString("n1") + ": Created Parcel");
    }

    /// <summary>
    /// Añade al paquete la incidencia de "Dirección incorrecta"
    /// </summary>
    public void AddInvalidAddressIncidence() {
        TrackingStatus.Add(Time.time.ToString("n1") + ": Dirección incorrecta");
    }

    /// <summary>
    /// Añade al paquete la incidencia de "El destinatario estaba ausente"
    /// </summary>
    public void AddRecipientMissingIncidence() {
        TrackingStatus.Add(Time.time.ToString("n1") + ": Destinatario ausente");
    }

    /// <summary>
    /// Añade el estado de "En raparto" a la información de seguimiento del paquete
    /// </summary>
    public void AddOutForDeliveryTrackingStatus() {
        TrackingStatus.Add(Time.time.ToString("n1") + ": En reparto");
    }


    /// <summary>
    /// Añade el estado de "Entregado" a la información de seguimiento del paquete
    /// </summary>
    public void AddDeliveredTrackingStatus() {
        TrackingStatus.Add(Time.time.ToString("n1") + ": Entregado");
    }

    /// <summary>
    /// Marca el paquete para que se devuelva al remitente. Debe llamarse a este método cuando intentemos entregar un
    /// paquete a una dirección incorrecta
    /// </summary>
    public virtual void MarkAsReturnToSender() {
        Attempts = MAX_ATTEMPTS;
    }

    /// <summary>
    /// Hace las operaciones necesarias para entregar el paquete
    /// </summary>
    public virtual void Deliver() {
        Done = true;
        AddDeliveredTrackingStatus();
        transform.localPosition = Vector3.up * .5f;
        transform.parent = null;
        if (Target == Sender) {
            GameManager.Instance.Returned++;
        } else if (Target == Recipient) {
            GameManager.Instance.Delivered++;
        }
    }
}
