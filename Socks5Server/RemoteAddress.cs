namespace Socks5Server;

public record RemoteAddress(string Address, int Port)
{
    public override string ToString()
    {
        return $"RemoteAddress{{address={Address}, port={Port}}}";
    }
}