using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsObservablesGenerators.Tests
{
    public class EventAsObservableGeneratorTest
    {
        [Test]
        public void Generate()
        {
            var clickEvent = typeof(Control).GetEvents().Single(e => e.Name == "Click");
            var code = EventAsObservableGenerator.GenerateExtensionMethodOfEvent(clickEvent);
            string expectedCode =
                @"public static IObservable<EventPattern<EventArgs>> ClickAsObservable(this Control @this)
    {
        return Observable.FromEventPattern<EventHandler, EventArgs>(h => @this.Click += h, h => @this.Click -= h);
    }";
            Assert.AreEqual(expectedCode, code);
        }
    }
}