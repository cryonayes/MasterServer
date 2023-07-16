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
            { (int)GameServerToClient.Welcome, GameServerHandle.Welcome },
            { (int)GameServerToMaster.LobbyReady, GameServerHandle.LobbyIsReady },
            { (int)GameServerToMaster.PlayerConnected, GameServerHandle.PlayerConnected },
            { (int)GameServerToMaster.PlayerDisconnected, GameServerHandle.PlayerDisconnected },
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
                return true; 
        }

        while (_packetLength > 0 && _packetLength <= _receivedData.UnreadLength())
        {
            var _packetBytes = _receivedData.ReadBytes(_packetLength);
            ThreadManager.ExecuteOnMainThread(() =>
            {
                using var _packet = new Packet(_packetBytes);
                var _packetId = _packet.ReadInt();
                _packetHandlers[_packetId](_packet);
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