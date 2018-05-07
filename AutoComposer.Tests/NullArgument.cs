using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutoComposer.Tests
{
    /// <summary>
    /// These cases test the behavior when null input is provided to the methods where a null is not permitted.
    /// In all cases, the system should throw an ArgumentNullException in response to the invalid input.
    /// </summary>
    [TestClass]
    public class NullArgument
    {
        [TestMethod]
        public void Enumerator()
        {
            Composer composer = new Composer();
            Assert.ThrowsException<ArgumentNullException>(delegate { composer.Compose(null); });
        }

        [TestMethod]
        public void GenericEnumerator()
        {
            Composer composer = new Composer();
            IEnumerator<Object> enumerator = null;
            Assert.ThrowsException<ArgumentNullException>(delegate { composer.Compose<Object>(enumerator); });
        }

        [TestMethod]
        public void GenericArray()
        {
            Composer composer = new Composer();
            Object[] array = null;
            Assert.ThrowsException<ArgumentNullException>(delegate { composer.Compose<Object>(array); });
        }

        [TestMethod]
        public void Flatten()
        {
            Composer composer = new Composer();
            Assert.ThrowsException<ArgumentNullException>(delegate { composer.FlattenComposableType(null); });
        }
    }
}
