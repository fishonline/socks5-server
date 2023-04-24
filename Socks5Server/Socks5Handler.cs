using System.Net.Sockets;
using System.Text;

namespace Socks5Server;

public static class Socks5Handler
{
    public static void Handle(Socket client, bool allowAnonymous)
    {
        Task.Run(() =>
        {
            try
            {
                Connect(client, allowAnonymous);
            }
            catch
            {
                // ignored
            }
        });
    }

    private static void Connect(Socket client, bool allowAnonymous)
    {
        var buffer = new byte[257];
        var length = client.Receive(buffer);

        if (length <= 0)
            return;

        var version = buffer[0];
        if (version != 0x05)
        {
            client.Send(new byte[] {5, 255});
            return;
        }

        if (allowAnonymous)
        {
            client.Send(new byte[] {5, 0});
            WaitingRequest(client);
            return;
        }

        if (length <= 1)
        {
            client.Send(new byte[] {5, 255});
            return;
        }

        var methods = buffer[1];
        for (var i = 0; i < methods; i++)
        {
            if (buffer[i + 2] != 0x02)
                continue;
            client.Send(new byte[] {5, 2});
            if (DoAuthentication(client))
                WaitingRequest(client);
            return;
        }
        client.Send(new byte[] {5, 255});
    }

    private static byte[] GetBuffer(Socket client)
    {
        var data = new List<byte>();
        var buffer = new byte[1024];
        do
        {
            data.AddRange(buffer[..client.Receive(buffer)]);
        } while (client.Available > 0);
        return data.ToArray();
    }

    private static void WaitingRequest(Socket client)
    {
        var buffer = GetBuffer(client);
        var length = buffer.Length;
        if (length <= 0)
            return;

        var version = buffer[0];
        if (version != 0x05)
        {
            client.Send(new byte[] {5, 1, 0, 1, 0, 0, 0, 0, 0});
            return;
        }

        var command = buffer[1];
        if (command != 0x01)
        {
            client.Send(new byte[] {5, 1, 0, 1, 0, 0, 0, 0, 0});
            return;
        }

        var addressInfo = GetRemoteAddressInfo(buffer, length);
        client.Send(new byte[] {5, 0, 0, 1, 0, 0, 0, 0, 0, 0});
        
        Socks5RelayHandler.DoRelay(client, addressInfo.Address, addressInfo.Port);
    }

    private static bool DoAuthentication(Socket client)
    {
        var buffer = new byte[513];
        var length = client.Receive(buffer);
        if (length <= 0)
            return false;

        var version = buffer[0];
        if (version != 0x01)
        {
            client.Send(new byte[] {1, 1});
            return false;
        }

        if (length <= 1)
        {
            client.Send(new byte[] {1, 1});
            return false;
        }

        var userInfo = UserInfo.Parse(buffer);
        if (userInfo.Match("vpn", "John1234"))
        {
            client.Send(new byte[] {1, 0});
            return true;
        }

        client.Send(new byte[] {1, 1});
        return false;
    }

    private static RemoteAddress GetRemoteAddressInfo(byte[] bytes, int length)
    {
        var data = bytes[5..(length - 2)];
        var address = Encoding.UTF8.GetString(data).Trim();
        var port = bytes[length - 2] << 8 | bytes[length - 1];
        return new RemoteAddress(address, port);
    }
}