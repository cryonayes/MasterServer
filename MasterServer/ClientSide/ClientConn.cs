using System;
using System.Net.Sockets;
using MasterServer.Common;
using MasterServer.ServerSide.MasterServer;
using MasterServer.Threading;

namespace MasterServer.ClientSide;

public class ClientConn
{
    public TcpClient Socket;

    private readonly int _id;
    private NetworkStream _stream;
    private Packet _receivedData;
    private byte[] _receiveBuffer;

    public ClientConn(int _id)
    {
        this._id = _id;
    }

    public void Connect(TcpClient _socket)
    {
        Socket = _socket;
        Socket.ReceiveBufferSize = Client.DataBufferSize;
        Socket.SendBufferSize = Client.DataBufferSize;

        _stream = Socket.GetStream();
        _receivedData = new Packet();
        _receiveBuffer = new byte[Client.DataBufferSize];

        _stream.BeginRead(_receiveBuffer, 0, Client.DataBufferSize, ReceiveCallback, null);
        ClientSend.Welcome(_id, "Welcome to the MasterServer!");
    }

    public void SendData(Packet _packet)
    {
        try
        {
            if (Socket != null)
                _stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
        }
        catch (Exception _ex)
        {
            Console.WriteLine($"Error sending data to player {_id} via TCP: {_ex}");
        }
    }

    private void ReceiveCallback(IAsyncResult _result)
    {
        try
        {
            var _byteLength = _stream.EndRead(_result);
            if (_byteLength <= 0)
            {
                Server.Clients[_id].Disconnect();
                return;
            }

            var _data = new byte[_byteLength];
            Array.Copy(_receiveBuffer, _data, _byteLength);

            _receivedData.Reset(HandleData(_data)); 
            _stream.BeginRead(_receiveBuffer, 0, Client.DataBufferSize, ReceiveCallback, null);
        }
        catch (Exception _ex)
        {
            Console.WriteLine($"Error receiving TCP data: {_ex}");
            Server.Clients[_id].Disconnect();
        }
    }

    private bool HandleData(byte[] _data)
    {
        var _packetLength = 0;
        
        _receivedData.SetBytes(_data);
        if (_receivedData.UnreadLength() >= 4)
        {
            _packetLength = _receivedData.ReadInt();
            if (_packetLength <= 0)
                return true; 
        }

        while (_packetLength > 0 && _packetLength <= _receivedData.UnreadLength())
        {
            var _packetBytes = _receivedData.ReadBytes(_packetLength);
            ThreadManager.ExecuteOnMainThread(() =>
            {
                using var _packet = new Packet(_packetBytes);
                var _packetId = _packet.ReadInt();
                Server.PacketHandlers[_packetId](_id, _packet);
            });

            _packetLength = 0;
            if (_receivedData.UnreadLength() < 4) continue;
            _packetLength = _receivedData.ReadInt();
            if (_packetLength <= 0)
            {
                return true;
            }
        }

        return _packetLength <= 1;
    }

    public void Disconnect()
    {
        Socket.Close();
        _stream = null;
        _receivedData = null;
        _receiveBuffer = null;
        Socket = null;
    }
}