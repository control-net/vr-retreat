using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using VrRetreat.Core;
using VrRetreat.WebApp.Controllers;
using Xunit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace VrRetreat.Tests;

public class ArchitectureTests
{
    private static readonly Architecture Architecture =
            new ArchLoader().LoadAssemblies(typeof(Class1).Assembly, typeof(WeatherForecastController).Assembly)
            .Build();

    private readonly IObjectProvider<IType> CoreLayer = Types().That().ResideInAssembly("VrRetreat.Core").And().ResideInNamespace("VrRetreat").As("Core");
    private readonly IObjectProvider<IType> WebAppLayer = Types().That().ResideInAssembly("VrRetreat.WebApp").And().ResideInNamespace("VrRetreat").As("WebApp");

    [Fact]  
    public void CoreShouldNotDependOnOtherProjects()
    {
        IArchRule rule = Types().That().Are(CoreLayer).Should().NotDependOnAny(WebAppLayer).Because("Core has to remain dependency free");

        rule.Check(Architecture);
    }
}
