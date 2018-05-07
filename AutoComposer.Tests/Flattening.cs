using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutoComposer.Tests
{
    /// <summary>
    /// These cases test the flattening feature of the library. 
    /// They should each return an array consisting of the top level class and each of its composable children.
    /// </summary>
    [TestClass]
    public class Flattening
    {
        [TestMethod]
        public void FlattenGeneric()
        {
            Composer composer = new Composer();
            Type[] types = composer.FlattenComposableType<Top>();
            Assert.AreEqual(types[0], typeof(Top));
            Assert.AreEqual(types[1], typeof(Middle));
            Assert.AreEqual(types[2], typeof(Bottom));
            Assert.AreEqual(types[3], typeof(Bottom));
            Assert.AreEqual(types[4], typeof(Middle));
            Assert.AreEqual(types[5], typeof(Bottom));
            Assert.AreEqual(types[6], typeof(Bottom));
        }

        [TestMethod]
        public void Flatten()
        {
            Composer composer = new Composer();
            Type[] types = composer.FlattenComposableType(typeof(Top));
            Assert.AreEqual(types[0], typeof(Top));
            Assert.AreEqual(types[1], typeof(Middle));
            Assert.AreEqual(types[2], typeof(Bottom));
            Assert.AreEqual(types[3], typeof(Bottom));
            Assert.AreEqual(types[4], typeof(Middle));
            Assert.AreEqual(types[5], typeof(Bottom));
            Assert.AreEqual(types[6], typeof(Bottom));
        }

        private class Top
        {
            [Composable(2)]
            public Middle Middle2 { get; set; }

            [Composable(1)]
            public Middle Middle { get; set; }
        }

        private class Middle
        {
            [Composable(2)]
            public Bottom Bottom2 { get; set; }

            [Composable(1)]
            public Bottom Bottom { get; set; }
        }

        private class Bottom
        {

        }
    }
}
