using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WindowsFormsObservablesGenerators
{
    public static class TypeCollectionToObservableGenerator
    {
        public static string Generate(IEnumerable<Type> types)
        {
            var lookup = types.ToLookup(type => type.Namespace);
            var strings = lookup.Select(
                grouping =>
                {
                    var namespaceText = grouping.Key;
                    var classText =
                        grouping.Where(type => type.GetEvents(BindingFlags.Instance | BindingFlags.Public).Any())
                            .Select(TypeToObservablesGenerator.Generate)
                            .Where(s => s != "").ToList();
                    if (classText.Any())
                    {
                        var list = string.Join("\r\n", classText);
                        var text = $@"namespace Observables.{namespaceText}
{{
{TextIndentAdder.Indent(list, 4)}          
}}";
                        return text;
                    }

                    return "";
                })
                .Where(s => s!="")
                .ToArray();
            return string.Join("\r\n", strings);
        }
    }
}