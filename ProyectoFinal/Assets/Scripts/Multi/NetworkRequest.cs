using System;

public class NetworkRequest
{
    public DateTime LastAttempt
    {
        get
        {
            return _lastAttempt;
        }
        set
        {
            _lastAttempt = value;
        }
    }
    public int Retries
    {
        get
        {
            return _retries;
        }
        set
        {
            _retries = value;
        }
    }
    public byte[] Data
    {
        get
        {
            return _data;
        }
    }
    public int Id
    {
        get
        {
            return _id;
        }
    }

    protected DateTime _lastAttempt;
    protected int _retries;
    protected int _id;
    protected byte[] _data;

    public NetworkRequest(int id, byte[] data)
    {
        _id = id;
        _data = data;
        _lastAttempt = DateTime.Now;
    }

}
