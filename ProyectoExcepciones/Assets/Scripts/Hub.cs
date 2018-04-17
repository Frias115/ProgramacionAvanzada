using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Hub : MonoBehaviour {
    public List<Parcel> PendingParcels;
    public DeliveryRobot[] Workers;
    public GameObject ParcelPrefab;
    public float ParcelSpacing = 1.5f;

	// Use this for initialization
	void Awake () {
        Workers = FindObjectsOfType<DeliveryRobot>();
        PendingParcels = new List<Parcel>();

        foreach (var parcel in transform.GetComponentsInChildren<Parcel>()) {
            PendingParcels.Add(parcel);
        }
        ArrangeParcels();
	}
	
    /// <summary>
    /// Solicita un paquete al Hub.
    /// En caso de no tener paquetes disponibles, lanzar una excepción.
    /// </summary>
    /// <returns>El paquete solicitado</returns>
    public Parcel RequestParcel () {
        if (PendingParcels.Count == 0) {
            throw new NoParcelsAvailableException();
        }
        var parcel = PendingParcels[0];
        PendingParcels.RemoveAt(0);
        ArrangeParcels();
        parcel.AddOutForDeliveryTrackingStatus();
        return parcel;
    }

    /// <summary>
    /// Almacena un paquete en el Hub, al final de la lista de paquetes pendientes
    /// </summary>
    /// <param name="parcel">Paquete a almacenar</param>
    public void StoreParcel(Parcel parcel) {
        PendingParcels.Add(parcel);
        parcel.transform.parent = transform;
        parcel.transform.position -= Vector3.up;
        ArrangeParcels();
    }

    /// <summary>
    /// Coloca los paquetes en la escena, para que se vean bien
    /// </summary>
    void ArrangeParcels() {
        for (int i = 0; i < PendingParcels.Count; i++) {
            PendingParcels[i].transform.localPosition = -(Vector3.right * ParcelSpacing * (i - PendingParcels.Count / 2f)) + Vector3.up * .5f;
        }
    }

}
