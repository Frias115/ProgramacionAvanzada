﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.Linq;
using System.Threading;
using System.Security.Cryptography;
using SimpleJSON;

public class TAPNet
{
    public const int DATAGRAM_ACK = 0;
    public const int DATAGRAM_NORMAL = 1;
    public const int DATAGRAM_RELIABLE = 2;

    public string hostIp;
    public int hostPort;
    public ProcessResponseDelegate onResponseReceived;

    protected IPEndPoint _hostEP;
    protected UdpClient _client;
    protected int _datagramId;
    protected Dictionary<int, string[]> _receivedDatagrams;
    protected Dictionary<int, int[]> _pendingDatagramsToReceive;
    protected List<NetworkRequest> _pendingSentRequests;
    protected Thread _listenThread;

    public TAPNet(string hostIp, int hostPort)
    {
        this.hostIp = hostIp;
        this.hostPort = hostPort;

        _hostEP = new IPEndPoint(IPAddress.Parse(hostIp), hostPort);
        _client = new UdpClient();
        try
        {
            _client.Connect(_hostEP);
            _receivedDatagrams = new Dictionary<int, string[]>();
            _pendingDatagramsToReceive = new Dictionary<int, int[]>();
            _pendingSentRequests = new List<NetworkRequest>();
            _listenThread = new Thread(new ThreadStart(Listen));
            _listenThread.Start();
        }
        catch (SocketException)
        {
            Debug.Log("Socket exception");
        }
    }


    /// <summary>
    /// Se ejecuta en segundo plano. Escucha los mensajes que llegan desde el servidor
    /// </summary>
    public void Listen()
    {
        while (true)
        {
            var data = _client.Receive(ref _hostEP);
            
            //Debug.Log("Data: " + Encoding.UTF8.GetString(data));
            ReceivedData(data);
        }
    }

    /// <summary>
    /// Envía una NetworkRequest al servidor
    /// </summary>
    /// <param name="dataToSend">Req.</param>
    public void Send(string dataToSend, int request_type = DATAGRAM_NORMAL)
    {
        var msg = Encoding.UTF8.GetBytes(dataToSend);

        byte[] datagram = BitConverter.GetBytes(request_type)
                                      .Concat(BitConverter.GetBytes(_datagramId))
                                      .Concat(CalculateSha256(msg))
                                      .Concat(msg).ToArray();

        if (request_type == DATAGRAM_RELIABLE)
        {
            //Apuntamos este paquete como 'pendiente de confirmar su recepción'
            _pendingSentRequests.Add(new NetworkRequest(_datagramId, datagram));
        }
        try
        {
            _client.Send(datagram, datagram.Length);
        }
        catch (SocketException e)
        {
            Debug.Log(e);
        }
        finally
        {
            _datagramId++;
        }
    }

    /// <summary>
    /// Reintenta el envío de un NetworkRequest que tengamos pendiente
    /// </summary>
    /// <param name="request">NetworkRequest a enviar.</param>
    void Resend(NetworkRequest request)
    {
        request.Retries++;
        request.LastAttempt = DateTime.Now;
        try
        {
            _client.Send(request.Data, request.Data.Length);
        }
        catch (SocketException e)
        {
            Debug.Log(e);
        }
    }

    /// <summary>
    /// Envía el ACK.
    /// </summary>
    /// <param name="datagramId">Datagrama a confirmar.</param>
    /// <param name="chunkId">Fragmento del datagrama a confirmar.</param>
    void SendAck(int datagramId, int chunkId)
    {
        byte[] datagram = BitConverter.GetBytes(DATAGRAM_ACK)
                                      .Concat(BitConverter.GetBytes(datagramId))
                                      .Concat(BitConverter.GetBytes(chunkId))
                                      .ToArray();
        try
        {
            _client.Send(datagram, datagram.Length);
        }
        catch (SocketException e)
        {
            Debug.Log(e);
        }
    }

    /// <summary>
    /// Comprueba que hayan llegado los ACK de los paquetes confiables enviados
    /// </summary>
    public IEnumerator CheckAcks()
    {
        while (true)
        {
            var now = DateTime.Now;
            _pendingSentRequests = _pendingSentRequests.Where((p) => p.Retries < 3).ToList();

            foreach (var pending in _pendingSentRequests)
            {
                if ((now - pending.LastAttempt).TotalSeconds > 1.5)
                {
                    Debug.Log("Resending " + pending.Id);
                    Resend(pending);
                }
            }
            yield return new WaitForSeconds(.5f);
        }

    }

    /// <summary>
    /// Calcula el SHA256 de un byte[].
    /// </summary>
    /// <returns>El SHA256 calculado.</returns>
    /// <param name="source">Bytes de los que queremos hallar el SHA256.</param>
    byte[] CalculateSha256(byte[] source)
    {
        return SHA256.Create().ComputeHash(source);
    }

    /// <summary>
    /// Compara dos hashes para ver si son iguales
    /// </summary>
    /// <returns><c>true</c> Si los hashes son iguales, <c>false</c> en caso contrario.</returns>
    /// <param name="a">Primer hash a comparar.</param>
    /// <param name="b">Segundo hash a comparar.</param>
    bool CompareSha256(byte[] a, byte[] b)
    {
        return a.SequenceEqual(b);
    }

    void ReceivedData(byte[] data)
    {
        try
        {
            var datagramType = BitConverter.ToInt32(data, 0); // Obtener de los datos recibidos, es un Int32 codificado en 4 bytes, empezando por el 0
            var datagramId = BitConverter.ToInt32(data, 4); // Obtener de los datos recibidos, es un Int32 codificado en 4 bytes, empezando por el 4


            if (datagramType != DATAGRAM_ACK)
            {
                var expectedSha256 = data.Sub(8, 32);  // Obtener de los datos recibidos, son 32 bytes, empezando por el 8

                var numberOfChunks = BitConverter.ToInt32(data, 40); // Obtener de los datos recibidos, es un Int32 codificado en 4 bytes, empezando por el 40
                var currentChunk = BitConverter.ToInt32(data, 44); // Obtener de los datos recibidos, es un Int32 codificado en 4 bytes, empezando por el 44

                if (!_receivedDatagrams.ContainsKey(datagramId))
                {
                    _receivedDatagrams[datagramId] = new string[numberOfChunks];
                }

                var obtainedSha256 = CalculateSha256(data.Sub(48, data.Length - 48)); // Calculamos el Sha256 del mensaje (bytes desde el 48 hasta el final)

                if (CompareSha256(expectedSha256, obtainedSha256))
                {
                    _receivedDatagrams[datagramId][currentChunk] = Encoding.UTF8.GetString(data.Sub(48, data.Length - 48));
                    if (datagramType == DATAGRAM_RELIABLE)
                    {
                        SendAck(datagramId, currentChunk);
                    }

                    if (!_receivedDatagrams[datagramId].Any(y => y == null))
                    {
                        // Hemos recibido todas las partes correctamente
                        var responseString = String.Join("", _receivedDatagrams[datagramId]);

                        _receivedDatagrams.Remove(datagramId);
                        if (onResponseReceived != null)
                        {
                            onResponseReceived(JSON.Parse(responseString));
                        }
                    }
                }
            }
            else
            {
                // Si es un ACK, vemos qué paquete nos están confirmando
                // y lo eliminamos de la lista de pendientes por confirmar
                _pendingSentRequests = _pendingSentRequests.Where((r) => r.Id != datagramId).ToList();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    /// <summary>
    /// Tipo de delegado que se encargará de procesar las respuestas a nuestras peticiones
    /// </summary>
    public delegate void ProcessResponseDelegate(JSONNode json);

    /// <summary>
    /// Libera los recursos utilizados por el socket
    /// </summary>
	public void Cleanup()
    {
        _client.Close();
    }
}
