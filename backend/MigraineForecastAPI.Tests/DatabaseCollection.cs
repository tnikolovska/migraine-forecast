using MigraineForecastAPI.Tests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MigraineForecastAPI.Tests
{
    [CollectionDefinition("Database collection")]
    public class DatabaseCollection :  ICollectionFixture<PostgreSqlFixture>
    {
    }
}
