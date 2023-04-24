using System.Text;

namespace Socks5Server;

public class UserInfo
{
    private readonly string _username;
    private readonly string _password;

    private UserInfo(string username, string password)
    {
        _username = username;
        _password = password;
    }

    public static UserInfo Parse(byte[] buffer)
    {
        var uLength = buffer[1];
        var pLength = buffer[uLength + 2];

        var uBytes = buffer[2..(uLength + 2)];
        var pBytes = buffer[(uLength + 3)..(uLength + pLength + 3)];

        return new UserInfo(Encoding.UTF8.GetString(uBytes), Encoding.UTF8.GetString(pBytes));
    }

    public bool Match(string username, string password)
    {
        return username == _username && password == _password;
    }
}