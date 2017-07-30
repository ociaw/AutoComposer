using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutoComposer.Tests
{
    [TestClass]
    public class MultipleComposition
    {
        [TestMethod]
        public void SingleNest()
        {
            Composer composer = new Composer();
            Object[] objects = { new MiddleLeft(), new BottomCenter(), new BottomLeft() };
            MiddleLeft middleLeft = composer.Compose<MiddleLeft>(objects);
            Assert.AreEqual(objects[0], middleLeft);
            Assert.AreEqual(objects[1], middleLeft.BottomCenter);
            Assert.AreEqual(objects[2], middleLeft.BottomLeft);
        }

        [TestMethod]
        public void DoubleNest()
        {
            Composer composer = new Composer();
            Object[] objects = { new Top(), new MiddleLeft(), new BottomCenter(), new BottomLeft(), new MiddleRight(), new BottomCenter(), new BottomRight() };
            Top top = composer.Compose<Top>(objects);
            Assert.AreEqual(objects[0], top);
            Assert.AreEqual(objects[1], top.MiddleLeft);
            Assert.AreEqual(objects[2], top.MiddleLeft.BottomCenter);
            Assert.AreEqual(objects[3], top.MiddleLeft.BottomLeft);
            Assert.AreEqual(objects[4], top.MiddleRight);
            Assert.AreEqual(objects[5], top.MiddleRight.BottomCenter);
            Assert.AreEqual(objects[6], top.MiddleRight.BottomRight);
        }

        private class Top
        {
            [Composable]
            public MiddleLeft MiddleLeft { get; set; }

            [Composable]
            public MiddleRight MiddleRight { get; set; }
        }

        private class MiddleLeft
        {
            [Composable]
            public BottomCenter BottomCenter { get; set; }

            [Composable]
            public BottomLeft BottomLeft { get; set; }
        }

        private class MiddleRight
        {
            [Composable]
            public BottomCenter BottomCenter { get; set; }

            [Composable]
            public BottomRight BottomRight { get; set; }
        }

        private class BottomCenter
        {

        }

        private class BottomLeft
        {

        }

        private class BottomRight
        {

        }
    }
}
