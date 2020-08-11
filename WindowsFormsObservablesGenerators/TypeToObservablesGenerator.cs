using System;
using System.Linq;
using System.Reflection;

namespace WindowsFormsObservablesGenerators
{
    public static class TypeToObservablesGenerator
    {
        public static string Generate(Type type)
        {
            var memberInfos = type.GetEvents(BindingFlags.Instance | BindingFlags.Public);
            var methods = memberInfos.Select(EventAsObservableGenerator.GenerateExtensionMethodOfEvent).ToList();
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
    }
}