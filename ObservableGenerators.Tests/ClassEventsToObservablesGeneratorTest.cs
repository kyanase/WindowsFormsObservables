using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using NUnit.Framework;

namespace ObservableGenerators.Tests
{
    internal class ClassEventsToObservablesGeneratorTest
    {
        [Test]
        public void ClassEvents()
        {
            var generate = TypeToObservablesGenerator.Generate(typeof(SampleClass));
            Console.WriteLine(generate);
            var expected = @"public static class SampleClassObservableExtensions
{
    public static IObservable<EventPattern<EventArgs>> SampleEventAsObservable(this SampleClass @this)
    {
        return Observable.FromEventPattern<EventHandler, EventArgs>(
            h => @this.SampleEvent += h, 
            h => @this.SampleEvent -= h);
    }
    public static IObservable<EventPattern<int>> SampleGenericEventAsObservable(this SampleClass @this)
    {
        return Observable.FromEventPattern<EventHandler<int>, int>(
            h => @this.SampleGenericEvent += h, 
            h => @this.SampleGenericEvent -= h);
    }
    public static IObservable<EventPattern<List<int>>> SampleGenericGenericEventAsObservable(this SampleClass @this)
    {
        return Observable.FromEventPattern<EventHandler<List<int>>, List<int>>(
            h => @this.SampleGenericGenericEvent += h, 
            h => @this.SampleGenericGenericEvent -= h);
    }
    public static IObservable<EventPattern<EventArgs>> StaticEventAsObservable()
    {
        return Observable.FromEventPattern<EventHandler, EventArgs>(
            h => SampleClass.StaticEvent += h, 
            h => SampleClass.StaticEvent -= h);
    }
}";
            Assert.AreEqual(expected, generate);
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
#pragma warning disable 67
        private class SampleClass
        {
            public event EventHandler SampleEvent;
            public event EventHandler<int> SampleGenericEvent;
            public event EventHandler<List<int>> SampleGenericGenericEvent;
            private event EventHandler PrivateEvent;
            public static event EventHandler StaticEvent;
        }
#pragma warning restore 67
    }
}