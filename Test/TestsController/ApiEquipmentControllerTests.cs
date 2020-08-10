using Keas.Core.Models;
using Keas.Mvc.Attributes;
using Keas.Mvc.Controllers.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shouldly;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using TestHelpers.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Test.TestsController
{
    [Trait("Category", "ControllerTests")]
    public class ApiEquipmentControllerTests
    {
        //TODO
    }

    [Trait("Category", "ControllerTests")]
    public class ApiEquipmentControllerReflectionTests
    {
        private readonly ITestOutputHelper output;
        public ControllerReflection ControllerReflection;

        public ApiEquipmentControllerReflectionTests(ITestOutputHelper output)
        {
            this.output = output;
            ControllerReflection = new ControllerReflection(this.output, typeof(EquipmentController));
        }

        #region Controller Class Tests

        [Fact]
        public void TestControllerClassAttributes()
        {
            ControllerReflection.ControllerInherits("SuperController");
            var authAttribute = ControllerReflection.ClassExpectedAttribute<AuthorizeAttribute>(5);
            authAttribute.ElementAt(0).Roles.ShouldBe(null);
            authAttribute.ElementAt(0).Policy.ShouldBe(AccessCodes.Codes.EquipMasterAccess);

            ControllerReflection.ClassExpectedAttribute<AutoValidateAntiforgeryTokenOrApiAttribute>(5);
            ControllerReflection.ClassExpectedAttribute<ControllerAttribute>(5);
            ControllerReflection.ClassExpectedAttribute<ApiControllerAttribute>(5);

            var route = ControllerReflection.ClassExpectedAttribute<RouteAttribute>(5);
            route.ElementAt(0).Template.ShouldBe("api/{teamName}/equipment/[action]");
        }

        #endregion Controller Class Tests

        #region Controller Method Tests

        [Fact]
        public void TestControllerContainsExpectedNumberOfPublicMethods()
        {
            ControllerReflection.ControllerPublicMethods(16);
        }

        [Fact]
        public void TestControllerMethodAttributes()
        {

#if DEBUG
            var countAdjustment = 1;
#else
            var countAdjustment = 0;
#endif

            //1
            var apiVal = ControllerReflection.MethodExpectedAttribute<ApiExplorerSettingsAttribute>("GetTeam", 1, "GetTeam-1", showListOfAttributes: false);
            apiVal.ElementAt(0).IgnoreApi.ShouldBeTrue();
            //2
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Search", 3 + countAdjustment, "Search", showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<HttpGetAttribute>("Search", 3 + countAdjustment, "Search", showListOfAttributes: true);
            var responseType = ControllerReflection.MethodExpectedAttribute<ProducesResponseTypeAttribute>("Search", 3 + countAdjustment, "Search", showListOfAttributes: false);
            responseType.ElementAt(0).Type.GenericTypeArguments.ElementAt(0).Name.ShouldBe("Equipment");
            responseType.ElementAt(0).StatusCode.ShouldBe(StatusCodes.Status200OK);
            //3
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("GetEquipmentInSpace", 2 + countAdjustment, "GetEquipmentInSpace", showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<HttpGetAttribute>("GetEquipmentInSpace", 2 + countAdjustment, "GetEquipmentInSpace", showListOfAttributes: false);
            //4
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("CommonAttributeKeys", 2 + countAdjustment, "CommonAttributeKeys", showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<HttpGetAttribute>("CommonAttributeKeys", 2 + countAdjustment, "CommonAttributeKeys", showListOfAttributes: false);
            //5
            ControllerReflection.MethodExpectedAttribute<HttpGetAttribute>("ListEquipmentTypes", 1, "ListEquipmentTypes", showListOfAttributes: false);
            //6
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("ListAssigned", 2 + countAdjustment, "ListAssigned", showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<HttpGetAttribute>("ListAssigned", 2 + countAdjustment, "ListAssigned", showListOfAttributes: false);
            //7
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("List", 2 + countAdjustment, "List", showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<HttpGetAttribute>("List", 2 + countAdjustment, "List", showListOfAttributes: false);
            //8
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Details", 2 + countAdjustment, "Details", showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<HttpGetAttribute>("Details", 2 + countAdjustment, "Details", showListOfAttributes: false);
            //9
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>("Create", 2 + countAdjustment, "Create", showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Create", 2 + countAdjustment, "Create", showListOfAttributes: false);
            //10
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>("Assign", 2 + countAdjustment, "Assign", showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Assign", 2 + countAdjustment, "Assign", showListOfAttributes: false);
            //11
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>("Update", 2 + countAdjustment, "Update", showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Update", 2 + countAdjustment, "Update", showListOfAttributes: false);
            //12
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>("Revoke", 2 + countAdjustment, "Revoke", showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Revoke", 2 + countAdjustment, "Revoke", showListOfAttributes: false);
            //13
            ControllerReflection.MethodExpectedAttribute<HttpPostAttribute>("Delete", 2 + countAdjustment, "Delete", showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("Delete", 2 + countAdjustment, "Delete", showListOfAttributes: false);
            //14
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("GetHistory", 2 + countAdjustment, "GetHistory", showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<HttpGetAttribute>("GetHistory", 2 + countAdjustment, "GetHistory", showListOfAttributes: false);
            //15
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("GetComputer", 2 + countAdjustment, "GetComputer", showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<HttpGetAttribute>("GetComputer", 2 + countAdjustment, "GetComputer", showListOfAttributes: false);
            //16
            ControllerReflection.MethodExpectedAttribute<AsyncStateMachineAttribute>("GetComputersBySearch", 2 + countAdjustment, "GetComputersBySearch", showListOfAttributes: false);
            ControllerReflection.MethodExpectedAttribute<HttpGetAttribute>("GetComputersBySearch", 2 + countAdjustment, "GetComputersBySearch", showListOfAttributes: false);
        }

        #endregion
    }
}
