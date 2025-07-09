namespace OMS.Api.Tests.Integration.Common;

internal abstract class TestDbContainer
{
    internal abstract Task StartAsync();

    internal abstract Task DisposeAsync();

    internal abstract string ConnectionString { get; }
}