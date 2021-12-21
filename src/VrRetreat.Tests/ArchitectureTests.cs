using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using VrRetreat.Core.Entities;
using VrRetreat.Infrastructure;
using VrRetreat.WebApp.Controllers;
using Xunit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace VrRetreat.Tests;

public class ArchitectureTests
{
    private static readonly Architecture Architecture =
            new ArchLoader().LoadAssemblies(typeof(VrChatUser).Assembly, typeof(HomeController).Assembly, typeof(VrChat).Assembly)
            .Build();

    private readonly IObjectProvider<IType> CoreLayer = Types().That().ResideInAssembly("VrRetreat.Core").And().ResideInNamespace("VrRetreat").As("Core");
    private readonly IObjectProvider<IType> WebAppLayer = Types().That().ResideInAssembly("VrRetreat.WebApp").And().ResideInNamespace("VrRetreat").As("WebApp");
    private readonly IObjectProvider<IType> Infrastructure = Types().That().ResideInAssembly("VrRetreat.Infrastructure").And().ResideInNamespace("VrRetreat").As("Infrastructure");

    [Fact]
    public void CoreShouldNotDependOnOtherProjects()
    {
        IArchRule coreToWebApp = Types().That().Are(CoreLayer).Should().NotDependOnAny(WebAppLayer).Because("Core has to remain dependency free");
        IArchRule coreToInfrastructure = Types().That().Are(CoreLayer).Should().NotDependOnAny(Infrastructure).Because("Core has to remain dependency free");

        coreToWebApp.Check(Architecture);
        coreToInfrastructure.Check(Architecture);
    }
}
