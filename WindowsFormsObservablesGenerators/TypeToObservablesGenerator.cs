using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WindowsFormsObservablesGenerators
{
    public static class TypeToObservablesGenerator
    {
        public static string Generate(Type type)
        {
            var methods = CreateMethodsForInstanceEvent(type)
                .Concat(CreateMethodsForStaticEvent(type));
            if (!methods.Any())
            {
                return "";
            }

            var methodsText = TextIndentAdder.Indent(string.Join("\r\n", methods), 4);
            return
                $@"public static class {type.Name}ObservableExtensions
{{
{methodsText}
}}";
        }

        private static IEnumerable<string> CreateMethodsForStaticEvent(Type type)
        {
            var memberInfos = type.GetEvents(BindingFlags.Static | BindingFlags.Public);
            var methods = memberInfos.Select(EventAsObservableGenerator.GenerateExtensionMethodOfStaticEvent).ToList();
            return methods;
        }

        private static List<string> CreateMethodsForInstanceEvent(Type type)
        {
            var memberInfos = type.GetEvents(BindingFlags.Instance | BindingFlags.Public);
            var methods = memberInfos.Select(EventAsObservableGenerator.GenerateExtensionMethodOfEvent).ToList();
            return methods;
        }
    }
}