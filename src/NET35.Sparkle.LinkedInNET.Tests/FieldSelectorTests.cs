
namespace Sparkle.LinkedInNET.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FieldSelectorTests
    {
        [TestMethod]
        public void CreatesWithEmptyList()
        {
            var target = new FieldSelector<FieldSelectorTests>();
            Assert.AreEqual(0, target.Items.Length);
        }

        [TestMethod]
        public void Add1()
        {
            var target = new FieldSelector<FieldSelectorTests>();
            var result = target.Add("hello");
            Assert.AreEqual(1, target.Items.Length);
            Assert.AreEqual("hello", target.Items[0]);
            Assert.AreSame(target, result);
        }

        [TestMethod]
        public void Add2()
        {
            var target = new FieldSelector<FieldSelectorTests>();
            var result = target.Add("hello").Add("world");
            Assert.AreEqual(2, target.Items.Length);
            Assert.AreEqual("hello", target.Items[0]);
            Assert.AreEqual("world", target.Items[1]);
            Assert.AreSame(target, result);
        }

        [TestMethod]
        public void Remove()
        {
            var target = new FieldSelector<FieldSelectorTests>();
            var result = target.Add("hello").Remove("hello");
            Assert.AreEqual(0, target.Items.Length);
            Assert.AreSame(target, result);
        }

        [TestMethod]
        public void Clear()
        {
            var target = new FieldSelector<FieldSelectorTests>();
            var result = target.Add("hello").Add("world").Clear();
            Assert.AreEqual(0, target.Items.Length);
            Assert.AreSame(target, result);
        }
    }
}
