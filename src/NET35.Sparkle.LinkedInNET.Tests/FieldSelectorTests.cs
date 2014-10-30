
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

        [TestMethod]
        public void ToStringEmpty()
        {
            var target = new FieldSelector<FieldSelectorTests>();
            var result = target.ToString();
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void ToString1()
        {
            var target = new FieldSelector<FieldSelectorTests>().Add("hello");
            var result = target.ToString();
            Assert.AreEqual(":(hello)", result);
        }

        [TestMethod]
        public void ToString2()
        {
            var target = new FieldSelector<FieldSelectorTests>().Add("hello").Add("world");
            var result = target.ToString();
            Assert.AreEqual(":(hello,world)", result);
        }

        [TestMethod]
        public void ToString3()
        {
            var target = new FieldSelector<FieldSelectorTests>().Add("hello").Add("location:(name,country)").Add("world");
            var result = target.ToString();
            Assert.AreEqual(":(hello,location:(name,country),world)", result);
        }

        [TestMethod]
        public void DeDuplicate()
        {
            var selector = new FieldSelector<object>().Add("location").Add("location");
            Assert.AreEqual(2, selector.Items.Length);
            Assert.AreEqual(":(location)", selector.ToString());
        }

        [TestMethod]
        public void MergeSimple()
        {
            var selector = new FieldSelector<object>().Add("location:(name)").Add("location:(code)");
            Assert.AreEqual(2, selector.Items.Length);
            Assert.AreEqual(":(location:(name,code))", selector.ToString());
        }

        [TestMethod]
        public void Merge2Levels()
        {
            var selector = new FieldSelector<object>().Add("location:(name)").Add("location:(country:(code))");
            Assert.AreEqual(2, selector.Items.Length);
            Assert.AreEqual(":(location:(name,country:(code)))", selector.ToString());
        }
    }
}
