using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutoComposer.Tests
{
    [TestClass]
    public class OrderedComposition
    {
        [TestMethod]
        public void SingleNest()
        {
            Composer composer = new Composer();
            Object[] objects = { new Middle(), new Bottom(), new Bottom() };
            Middle middle = composer.Compose<Middle>(objects);
            Assert.AreEqual(objects[0], middle);
            Assert.AreEqual(objects[1], middle.Bottom);
            Assert.AreEqual(objects[2], middle.Bottom2);
        }

        [TestMethod]
        public void DoubleNest()
        {
            Composer composer = new Composer();
            Object[] objects = { new Top(), new Middle(), new Bottom(), new Bottom(), new Middle(), new Bottom(), new Bottom() };
            Top top = composer.Compose<Top>(objects);
            Assert.AreEqual(objects[0], top);
            Assert.AreEqual(objects[1], top.Middle);
            Assert.AreEqual(objects[2], top.Middle.Bottom);
            Assert.AreEqual(objects[3], top.Middle.Bottom2);
            Assert.AreEqual(objects[4], top.Middle2);
            Assert.AreEqual(objects[5], top.Middle2.Bottom);
            Assert.AreEqual(objects[6], top.Middle2.Bottom2);
        }

        private class Top
        {
            [Composable]
            [AssignOrder(2)]
            public Middle Middle2 { get; set; }

            [Composable]
            [AssignOrder(1)]
            public Middle Middle { get; set; }
        }

        private class Middle
        {
            [Composable]
            [AssignOrder(2)]
            public Bottom Bottom2 { get; set; }

            [Composable]
            [AssignOrder(1)]
            public Bottom Bottom { get; set; }
        }

        private class Bottom
        {

        }
    }
}
