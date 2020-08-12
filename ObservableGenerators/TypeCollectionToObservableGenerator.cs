using System;
using System.Collections.Generic;
using System.Linq;

namespace ObservableGenerators
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
                        var codeForTypes = grouping.Select(TypeToObservablesGenerator.Generate).Where(s => s != "")
                            .ToList();
                        if (!codeForTypes.Any())
                        {
                            return "";
                        }

                        var list = string.Join("\r\n", codeForTypes);
                        var text = $@"namespace Observables.{namespaceText}
{{
{TextIndentAdder.Indent(list, 4)}          
}}";
                        return text;
                    })
                .Where(s => s != "")
                .ToArray();
            return string.Join("\r\n", strings);
        }
    }
}