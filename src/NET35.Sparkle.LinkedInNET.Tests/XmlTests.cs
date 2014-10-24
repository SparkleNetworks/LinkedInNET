
namespace Sparkle.LinkedInNET.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sparkle.LinkedInNET.Profiles;

    [TestClass]
    public class XmlTests
    {
        [TestMethod]
        public void FullProfile()
        {
            Person item;
            var serializer = new XmlSerializer(typeof(Person));
            using (var stream = this.GetType().Assembly.GetManifestResourceStream("Sparkle.LinkedInNET.Tests.Files.FullProfile.xml"))
            {
                item = (Person)serializer.Deserialize(stream); 
            }

            Assert.IsFalse(string.IsNullOrEmpty(item.CurrentShare));
            ////Assert.IsNotNull(item.CurrentStatus);
            Assert.IsFalse(string.IsNullOrEmpty(item.CurrentStatusTimestamp));
            Assert.IsFalse(string.IsNullOrEmpty(item.Distance));
            Assert.IsFalse(string.IsNullOrEmpty(item.Firstname));
            Assert.IsFalse(string.IsNullOrEmpty(item.FormattedName));
            ////Assert.IsFalse(string.IsNullOrEmpty(item.FormattedPhoneticName));
            Assert.IsFalse(string.IsNullOrEmpty(item.Headline));
            Assert.IsFalse(string.IsNullOrEmpty(item.Id));
            Assert.IsFalse(string.IsNullOrEmpty(item.Industry));
            Assert.IsFalse(string.IsNullOrEmpty(item.Lastname));
            ////Assert.IsFalse(string.IsNullOrEmpty(item.MaidenName));
            Assert.IsFalse(string.IsNullOrEmpty(item.NumConnections));
            ////Assert.IsFalse(string.IsNullOrEmpty(item.NumConnectionsCapped));
            Assert.IsFalse(string.IsNullOrEmpty(item.PhoneticFirstName));
            Assert.IsFalse(string.IsNullOrEmpty(item.PhoneticLastName));
            Assert.IsFalse(string.IsNullOrEmpty(item.PictureUrl));
            Assert.IsFalse(string.IsNullOrEmpty(item.Positions));
            Assert.IsFalse(string.IsNullOrEmpty(item.PublicProfileUrl));
            Assert.IsFalse(string.IsNullOrEmpty(item.Specialties));
            Assert.IsFalse(string.IsNullOrEmpty(item.Summary));
        }
    }
}
