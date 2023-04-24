using System.Net.Sockets;

namespace Socks5Server;

public static class Socks5RelayHandler
{
    public static void DoRelay(Socket client, string address, int port)
    {
        Socket server = null!;
        try
        {
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Connect(address, port);

            var clientStream = new NetworkStream(client);
            var serverStream = new NetworkStream(server);
            var clientStreamTask = clientStream.CopyToAsync(serverStream);
            var serverStreamTask = serverStream.CopyToAsync(clientStream);

            Task.WhenAll(clientStreamTask, serverStreamTask).ContinueWith(task =>
            {
                if (!task.IsCompleted)
                    return;
                serverStream.Close();
                clientStream.Close();
                Close(server, client);
            });
        }
        catch
        {
            Close(server, client);
        }
    }

    private static void Close(Socket server, Socket client)
    {
        server.Close();
        client.Close();
    }
}