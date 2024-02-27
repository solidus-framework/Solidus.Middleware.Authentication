using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;

namespace Solidus.Middleware.Authentication;

// TODO: Move to common project, reference non-full MVC.

internal static partial class ControllerHelpers
{
    [GeneratedRegex("Controller$")]
    private static partial Regex ControllerRouteNameRegex();

    public static string GetControllerRouteName<TController>()
        where TController : ControllerBase
    {
        var controllerName = typeof(TController).Name;
        if (ControllerRouteNameRegex().IsMatch(controllerName) is not true)
        {
            throw new NotSupportedException("Controller type name must ends with 'Controller'.");
        }

        return ControllerRouteNameRegex().Replace(controllerName, "");
    }

    public static string GetControllerArea<TController>()
        where TController : ControllerBase
    {
        var areaAttribute = typeof(TController).GetCustomAttribute<AreaAttribute>()
            ?? throw new NotSupportedException($"Controller type name must have '{typeof(AreaAttribute).FullName}' attribute.");

        return areaAttribute.RouteValue;
    }

    public static string GetActionRouteName<TController>(Expression<Func<TController, Delegate>> actionSelector)
        where TController : ControllerBase
    {
        var actionFinder = new ActionMethodFinderExpressionVisitor();
        actionFinder.Visit(actionSelector);

        if (actionFinder.FoundActionMethodInfo == null)
        {
            throw new ArgumentException("Expression tree must contain action delegate.", nameof(actionSelector));
        }

        var actionNameAttribute = actionFinder.FoundActionMethodInfo.GetCustomAttribute<ActionNameAttribute>();
        return actionNameAttribute?.Name ?? actionFinder.FoundActionMethodInfo.Name;
    }

    public static string GetActionRouteConstraints<TController>(params Expression<Func<TController, Delegate>>[] actionSelectors)
        where TController : ControllerBase
    {
        var routeNames = actionSelectors
            .Select(GetActionRouteName)
            .Select(Regex.Escape);

        return string.Join("|", routeNames);
    }

    private class ActionMethodFinderExpressionVisitor : ExpressionVisitor
    {
        public MethodInfo? FoundActionMethodInfo { get; private set; }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value is MethodInfo methodInfo
                && (methodInfo.ReturnType.IsAssignableFrom(typeof(IActionResult))
                    || methodInfo.ReturnType.IsAssignableFrom(typeof(Task<IActionResult>))))
            {
                FoundActionMethodInfo = methodInfo;
            }

            return node;
        }
    }
}
