using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using MasterServer.Common;
using MasterServer.Threading;

namespace MasterServer.ServerSide.GameServer;

public class GameServerConn
{
    private static GameServerConn _instance = new();
    private readonly TcpClient _socket;
    private readonly NetworkStream _stream;
    private Packet _receivedData;
    
    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> _packetHandlers;

    public static GameServerConn Instance => _instance;
    
    public event EventHandler OnConnected;
    public event EventHandler OnDisconnected;

    private GameServerConn() {}
    private GameServerConn(string _host, int _port)
    {
        _socket = new TcpClient(_host, _port);
        _stream = _socket.GetStream();
        InitializeHandlers();
    }

    public static void CreateInstance(string _host, int _port)
    {
        _instance = new GameServerConn(_host, _port);
    }

    private void InitializeHandlers()
    {
        _packetHandlers = new Dictionary<int, PacketHandler>
        {
            { (int)GameServerToMaster.Welcome, GameServerHandle.Welcome }
        };
    }

    public async Task Start()
    {
        OnConnected?.Invoke(this, EventArgs.Empty);
        _receivedData = new Packet();
        var _buffer = new byte[1024];

        await using (var _networkStream = _socket.GetStream())
        {
            while (await _networkStream.ReadAsync(_buffer) > 0)
                _receivedData.Reset(HandleData(_buffer));
        }
        OnDisconnected?.Invoke(this, EventArgs.Empty);
    }

    private bool HandleData(byte[] _data)
    {
        var _packetLength = 0;

        _receivedData.SetBytes(_data);

        if (_receivedData.UnreadLength() >= 4)
        {
            _packetLength = _receivedData.ReadInt();
            if (_packetLength <= 0)
                return true; // Reset receivedData instance to allow it to be reused
        }

        while (_packetLength > 0 && _packetLength <= _receivedData.UnreadLength())
        {
            // While packet contains data AND packet data length doesn't exceed the length of the packet we're reading
            var _packetBytes = _receivedData.ReadBytes(_packetLength);
            ThreadManager.ExecuteOnMainThread(() =>
            {
                using var _packet = new Packet(_packetBytes);
                var _packetId = _packet.ReadInt();
                _packetHandlers[_packetId](_packet); // Call appropriate method to handle the packet
            });

            _packetLength = 0; // Reset packet length
            if (_receivedData.UnreadLength() < 4) continue;
            // If client's received data contains another packet
            _packetLength = _receivedData.ReadInt();
            if (_packetLength <= 0)
                return true; // Reset receivedData instance to allow it to be reused
        }

        if (_packetLength <= 1)
            return true; // Reset receivedData instance to allow it to be reused

        return false;
    }
    
    public void SendData(Packet _packet)
    {
        try
        {
            if (_socket != null)
                _stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
        }
        catch (Exception _ex)
        {
            Console.WriteLine($"Error sending data to GameServer via TCP: {_ex}");
        }
    }
}