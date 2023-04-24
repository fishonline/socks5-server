using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace Socks5Server;

public class Socks5Acceptor
{
    private readonly int _port;
    private readonly BlockingCollection<Socket> _queue;

    public Socks5Acceptor(int port, BlockingCollection<Socket> queue)
    {
        _port = port;
        _queue = queue;
    }

    public void Accept()
    {
        try
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, _port));
            socket.Listen();
            Console.WriteLine($"socks5 server listen on port: {_port}");
            while (!_queue.IsAddingCompleted)
                _queue.Add(socket.Accept());
        }
        catch
        {
            // ignored
        }
    }
}