using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ObservableGenerators
{
    public static class EventAsObservableGenerator
    {
        public static string GenerateExtensionMethodOfEvent(EventInfo eventInfo)
        {
            var parameterTypeName = GetParameterTypeName(eventInfo);
            Debug.Assert(eventInfo.DeclaringType != null, "eventInfo.DeclaringType != null");
            var genericAwareTypeName = ToGenericAwareTypeName(eventInfo.EventHandlerType);
            return $@"public static IObservable<EventPattern<{parameterTypeName}>> {eventInfo.Name}AsObservable(this {ToGenericAwareTypeName(eventInfo.DeclaringType)} @this)
{{
    return Observable.FromEventPattern<{genericAwareTypeName}, {parameterTypeName}>(
        h => @this.{eventInfo.Name} += h, 
        h => @this.{eventInfo.Name} -= h);
}}";
        }

        public static string GenerateExtensionMethodOfStaticEvent(EventInfo eventInfo)
        {
            var parameterTypeName = GetParameterTypeName(eventInfo); Debug.Assert(eventInfo.DeclaringType != null, "eventInfo.DeclaringType != null");
            var genericAwareTypeName = ToGenericAwareTypeName(eventInfo.EventHandlerType);
            return $@"public static IObservable<EventPattern<{parameterTypeName}>> {eventInfo.Name}AsObservable()
{{
    return Observable.FromEventPattern<{genericAwareTypeName}, {parameterTypeName}>(
        h => {ToGenericAwareTypeName(eventInfo.DeclaringType)}.{eventInfo.Name} += h, 
        h => {ToGenericAwareTypeName(eventInfo.DeclaringType)}.{eventInfo.Name} -= h);
}}";
        }

        private static string GetParameterTypeName(EventInfo eventInfo)
        {
            var memberInfo = eventInfo.EventHandlerType.GetMethod("Invoke");
            var parameterInfos = memberInfo?.GetParameters() ?? new ParameterInfo[0];
            var parameterInfo = parameterInfos[1];
            var parameterTypeName = ToGenericAwareTypeName(parameterInfo.ParameterType);
            return parameterTypeName;
        }

        private static string ToGenericAwareTypeName(Type eventInfoEventHandlerType)
        {
            var genericArguments = eventInfoEventHandlerType.GetGenericArguments();
            if (genericArguments.Length == 0)
            {
                return ToName(eventInfoEventHandlerType);
            }
            var eventTypeName = Regex.Replace(eventInfoEventHandlerType.Name, "`.*$", "");
            return eventTypeName + "<" + string.Join(", ", genericArguments.Select(ToGenericAwareTypeName)) + ">";
        }

        private static readonly IDictionary<Type, string> PrimitiveTypeToNameDictionary = new Dictionary<Type, string>()
        {
            [typeof(bool)] = "bool",
            [typeof(byte)] = "byte",
            [typeof(char)] = "char",
            [typeof(decimal)] = "decimal",
            [typeof(double)] = "double",
            [typeof(float)] = "float",
            [typeof(int)] = "int",
            [typeof(long)] = "long",
            [typeof(object)] = "object",
            [typeof(sbyte)] = "sbyte",
            [typeof(short)] = "short",
            [typeof(string)] = "string",
            [typeof(uint)] = "uint",
            [typeof(ulong)] = "ulong",
            [typeof(ushort)] = "ushort",
            [typeof(void)] = "void"
        };
        private static string ToName(Type type)
        {
            if (PrimitiveTypeToNameDictionary.ContainsKey(type))
            {
                return PrimitiveTypeToNameDictionary[type];
            }
            return type.Name;
        }
    }
}