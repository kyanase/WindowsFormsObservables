using NUnit.Framework;
using System;
using System.Linq;
using JetBrains.Annotations;

namespace WindowsFormsObservablesGenerators.Tests
{
    public class EventAsObservableGeneratorTest
    {
        [Test]
        public void Generate()
        {
            var clickEvent = typeof(SampleClass).GetEvents().Single(e => e.Name == "Click");
            var code = EventAsObservableGenerator.GenerateExtensionMethodOfEvent(clickEvent);
            string expectedCode =
                @"public static IObservable<EventPattern<EventArgs>> ClickAsObservable(this SampleClass @this)
{
    return Observable.FromEventPattern<EventHandler, EventArgs>(h => @this.Click += h, h => @this.Click -= h);
}";
            Assert.AreEqual(expectedCode, code);
        }
        [Test]
        public void GeneratePrimitive()
        {
            var clickEvent = typeof(SampleClass).GetEvents().Single(e => e.Name == "GenericPrimitiveEvent");
            var code = EventAsObservableGenerator.GenerateExtensionMethodOfEvent(clickEvent);
            string expectedCode =
                @"public static IObservable<EventPattern<int>> GenericPrimitiveEventAsObservable(this SampleClass @this)
{
    return Observable.FromEventPattern<EventHandler<int>, int>(h => @this.GenericPrimitiveEvent += h, h => @this.GenericPrimitiveEvent -= h);
}";
            Assert.AreEqual(expectedCode, code);
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private class SampleClass
        {
            public event EventHandler Click;
            public event EventHandler<int> GenericPrimitiveEvent;
        }
    }
}