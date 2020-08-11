using System.Linq;
using System.Text.RegularExpressions;

namespace WindowsFormsObservablesGenerators
{
    internal static class TextIndentAdder
    {
        public static string Indent(string text, int indentCount)
        {
            var lines = new Regex(@"\r?\n", RegexOptions.Compiled).Split(text);
            return string.Join("\r\n", lines.Select(line => new string(' ', indentCount) + line));
        }
    }
}