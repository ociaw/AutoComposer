using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutoComposer.Tests
{
    [TestClass]
    public class SingleComposition
    {
        [TestMethod]
        public void NoNest()
        {
            Composer composer = new Composer();
            Object[] objects = { new Bottom() };
            Bottom bottom = composer.Compose<Bottom>(objects);
            Assert.AreEqual(objects[0], bottom);
        }

        [TestMethod]
        public void SingleNest()
        {
            Composer composer = new Composer();
            Object[] objects = { new Middle(), new Bottom() };
            Middle middle = composer.Compose<Middle>(objects);
            Assert.AreEqual(objects[0], middle);
            Assert.AreEqual(objects[1], middle.Bottom);
        }

        [TestMethod]
        public void DoubleNest()
        {
            Composer composer = new Composer();
            Object[] objects = {new Top(), new Middle(), new Bottom()};
            Top top = composer.Compose<Top>(objects);
            Assert.AreEqual(objects[0], top);
            Assert.AreEqual(objects[1], top.Middle);
            Assert.AreEqual(objects[2], top.Middle.Bottom);
        }

        [TestMethod]
        public void DoubleNestWithNull()
        {
            Composer composer = new Composer();
            Object[] objects = { null, new Middle(), new Bottom() };
            Top top = composer.Compose<Top>(objects);
            Assert.AreEqual(objects[0], top);
        }

        [TestMethod]
        public void BadDoubleNest()
        {
            Composer composer = new Composer();
            Object[] objects = { new Top(), new Middle() };
            Assert.ThrowsException<ArgumentException>(delegate { composer.Compose<Top>(objects); });
        }

        [TestMethod]
        public void NoNestEnumerator()
        {
            Composer composer = new Composer();
            Object[] objects = { new Bottom() };
            Object bottom = composer.Compose(((IEnumerable<Object>)objects).GetEnumerator());
            Assert.AreEqual(objects[0], bottom);
        }

        [TestMethod]
        public void NoNestEnumeratorGeneric()
        {
            Composer composer = new Composer();
            Object[] objects = { new Top(), new Middle(), new Bottom() };
            Top top = composer.Compose<Top>(((IEnumerable<Object>)objects).GetEnumerator());
            Assert.AreEqual(objects[0], top);
            Assert.AreEqual(objects[1], top.Middle);
            Assert.AreEqual(objects[2], top.Middle.Bottom);
        }

        private class Top
        {
            [Composable]
            public Middle Middle { get; set; }
        }

        private class Middle
        {
            [Composable]
            public Bottom Bottom { get; set; }
        }

        private class Bottom
        {
            
        }
    }
}
