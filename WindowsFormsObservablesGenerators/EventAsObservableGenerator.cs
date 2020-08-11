using System;
using System.CodeDom;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.CSharp;

namespace WindowsFormsObservablesGenerators
{
    public static class EventAsObservableGenerator
    {
        public static string GenerateExtensionMethodOfEvent(EventInfo eventInfo)
        {
            var memberInfo = eventInfo.EventHandlerType.GetMethod("Invoke");
            var parameterInfos = memberInfo?.GetParameters() ?? new ParameterInfo[0];
            var parameterInfo = parameterInfos[1];
            var parameterTypeName = ToGenericAwareTypeName(parameterInfo.ParameterType);
            Debug.Assert(eventInfo.DeclaringType != null, "eventInfo.DeclaringType != null");
            var genericAwareTypeName = ToGenericAwareTypeName(eventInfo.EventHandlerType);
            return $@"public static IObservable<EventPattern<{parameterTypeName}>> {eventInfo.Name}AsObservable(this {ToGenericAwareTypeName(eventInfo.DeclaringType)} @this)
{{
    return Observable.FromEventPattern<{genericAwareTypeName}, {parameterTypeName}>(h => @this.{eventInfo.Name} += h, h => @this.{eventInfo.Name} -= h);
}}";
        }

        private static string ToGenericAwareTypeName(Type eventInfoEventHandlerType)
        {
            var genericArguments = eventInfoEventHandlerType.GetGenericArguments();
            if (genericArguments.Length == 0)
            {
                return ToName(eventInfoEventHandlerType);
            }
            var eventTypeName = Regex.Replace(eventInfoEventHandlerType.Name, "`.*$","");
            return eventTypeName + "<" + string.Join(", ", genericArguments.Select(ToGenericAwareTypeName)) +">";
        }

        private static string ToName(Type type)
        {
            if (type.IsPrimitive)
            {
                var compiler = new CSharpCodeProvider();
                return compiler.GetTypeOutput(new CodeTypeReference(type));
            }

            return type.Name;
        }
    }
}