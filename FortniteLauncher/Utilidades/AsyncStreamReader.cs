using System;
using System.IO;

public class AsyncStreamReader
{
    public event EventHandler<string> DataReceived;

    public bool Active { get; private set; }

    public AsyncStreamReader(StreamReader reader)
    {
        _reader = reader;
        Active = false;
    }

    public void Start()
    {
        if (!Active)
        {
            Active = true;
            BeginReadAsync();
        }
    }

    public void Stop()
    {
        Active = false;
    }

    protected void BeginReadAsync()
    {
        if (Active) _reader.BaseStream.BeginRead(_buffer, 0, _buffer.Length, new AsyncCallback(ReadCallback), null);
    }

    private void ReadCallback(IAsyncResult asyncResult)
    {
        if (_reader == null) return;

        int num = _reader.BaseStream.EndRead(asyncResult);
        string data = null;

        if (num > 0)
            data = _reader.CurrentEncoding.GetString(_buffer, 0, num);
        else
            Active = false;

        DataReceived?.Invoke(this, data);

        BeginReadAsync();
    }

    protected readonly byte[] _buffer = new byte[4096];

    private StreamReader _reader;

    public delegate void EventHandler<args>(object sender, string data);
}
