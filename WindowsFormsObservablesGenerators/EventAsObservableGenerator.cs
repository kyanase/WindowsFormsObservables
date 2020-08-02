using System.Reflection;

namespace WindowsFormsObservablesGenerators
{
    public static class EventAsObservableGenerator
    {
        public static string GenerateExtensionMethodOfEvent(EventInfo clickEvent)
        {
            var memberInfo = clickEvent.EventHandlerType.GetMethod("Invoke");
            var parameterInfos = memberInfo.GetParameters();
            var parameterInfo = parameterInfos[1];
            var parameterTypeName = parameterInfo.ParameterType.Name;
            return $@"public static IObservable<EventPattern<{parameterTypeName}>> {clickEvent.Name}AsObservable(this {clickEvent.DeclaringType.Name} @this)
    {{
        return Observable.FromEventPattern<{clickEvent.EventHandlerType.Name}, {parameterTypeName}>(h => @this.{clickEvent.Name} += h, h => @this.{clickEvent.Name} -= h);
    }}";
        }
    }
}