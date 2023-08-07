namespace RequestForwarding.Services
{
    public interface IHeaderOptions
    {
        ISet<string> Ignore { get; }

        ISet<string> ContentHeaders { get; }

        ISet<string> ExcludeFromResponse { get;}
    }
}
