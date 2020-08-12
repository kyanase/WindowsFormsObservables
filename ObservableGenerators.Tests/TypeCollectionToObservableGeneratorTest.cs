using System;
using JetBrains.Annotations;
using NUnit.Framework;

namespace ObservableGenerators.Tests
{
    public class TypeCollectionToObservableGeneratorTest
    {
        [Test]
        public void Generate()
        {
            var types = new[] { typeof(SampleClass) };
            var actual = TypeCollectionToObservableGenerator.Generate(types);
            Console.WriteLine(actual);
            var expected =
                @"namespace Observables.ObservableGenerators.Tests
{
    public static class SampleClassObservableExtensions
    {
        public static IObservable<EventPattern<EventArgs>> SampleEventAsObservable(this SampleClass @this)
        {
            return Observable.FromEventPattern<EventHandler, EventArgs>(
                h => @this.SampleEvent += h, 
                h => @this.SampleEvent -= h);
        }
    }          
}";


            Assert.AreEqual(expected, actual);
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
#pragma warning disable 67
        private class SampleClass
        {
            public event EventHandler SampleEvent;
        }
#pragma warning restore 67

        [Test]
        public void GenerateForEmptyArray()
        {
            var actual = TypeCollectionToObservableGenerator.Generate(new Type[0]);
            Assert.AreEqual("", actual);
        }

        [Test]
        public void GenerateFromNoEventTypes()
        {
            var actual = TypeCollectionToObservableGenerator.Generate(new[] { typeof(SampleWithoutEventClass) });
            Assert.AreEqual("", actual);
        }

        private class SampleWithoutEventClass
        {
        }
    }
}