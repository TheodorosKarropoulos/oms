using DotNet.Testcontainers.Builders;
using Testcontainers.MsSql;

namespace OMS.Api.Tests.Integration.Common;

internal class MsSqlDbContainer : TestDbContainer
{
    private readonly MsSqlContainer _sqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithWaitStrategy(Wait.ForUnixContainer()
            .UntilPortIsAvailable(1433)
            .UntilMessageIsLogged(
                "SQL Server is now ready for client connections",
                waitStrategy =>
                {
                    waitStrategy.WithTimeout(TimeSpan.FromMinutes(5));
                }))
        .Build();

    internal override async Task StartAsync()
    {
        await _sqlContainer.StartAsync();
    }

    internal override async Task DisposeAsync()
    {
        await _sqlContainer.DisposeAsync();
    }

    internal override string ConnectionString => _sqlContainer.GetConnectionString();
}