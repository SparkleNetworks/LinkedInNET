
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
        public void FullProfile_Level1()
        {
            Person item;
            var serializer = new XmlSerializer(typeof(Person));
            using (var stream = this.GetType().Assembly.GetManifestResourceStream("Sparkle.LinkedInNET.Tests.Files.FullProfile.xml"))
            {
                item = (Person)serializer.Deserialize(stream);
            }

            // strings
            Assert.IsFalse(string.IsNullOrEmpty(item.Firstname));
            Assert.IsFalse(string.IsNullOrEmpty(item.FormattedName));
            Assert.IsFalse(string.IsNullOrEmpty(item.Headline));
            Assert.IsFalse(string.IsNullOrEmpty(item.Id));
            Assert.IsFalse(string.IsNullOrEmpty(item.Industry));
            Assert.IsFalse(string.IsNullOrEmpty(item.Lastname));
            ////Assert.IsFalse(string.IsNullOrEmpty(item.MaidenName));            // not in sample
            ////Assert.IsFalse(string.IsNullOrEmpty(item.FormattedPhoneticName)); // not in sample
            ////Assert.IsFalse(string.IsNullOrEmpty(item.PhoneticFirstName));     // not in sample
            ////Assert.IsFalse(string.IsNullOrEmpty(item.PhoneticLastName));      // not in sample
            Assert.IsFalse(string.IsNullOrEmpty(item.PictureUrl));
            Assert.IsFalse(string.IsNullOrEmpty(item.PublicProfileUrl));
            ////Assert.IsFalse(string.IsNullOrEmpty(item.Specialties));
            Assert.IsFalse(string.IsNullOrEmpty(item.Summary));

            // ints and longs
            Assert.AreEqual(4, item.Distance);
            Assert.AreEqual(142, item.NumConnections);
            Assert.AreEqual(1414145160000, item.CurrentStatusTimestamp);

            // bools
            Assert.IsTrue(item.NumConnectionsCapped.Value);
        }

        [TestMethod]
        public void FullProfile_Urls()
        {
            Person item;
            var serializer = new XmlSerializer(typeof(Person));
            using (var stream = this.GetType().Assembly.GetManifestResourceStream("Sparkle.LinkedInNET.Tests.Files.FullProfile.xml"))
            {
                item = (Person)serializer.Deserialize(stream);
            }

            Assert.IsNotNull(item.SiteStandardProfileRequest);
            Assert.IsFalse(string.IsNullOrEmpty(item.SiteStandardProfileRequest.Url));
        }

        [TestMethod]
        public void FullProfile_Location()
        {
            Person item;
            var serializer = new XmlSerializer(typeof(Person));
            using (var stream = this.GetType().Assembly.GetManifestResourceStream("Sparkle.LinkedInNET.Tests.Files.FullProfile.xml"))
            {
                item = (Person)serializer.Deserialize(stream);
            }

            Assert.IsNotNull(item.Location);
            Assert.IsFalse(string.IsNullOrEmpty(item.Location.Name));
        }

        [TestMethod]
        public void FullProfile_Positions()
        {
            Person item;
            var serializer = new XmlSerializer(typeof(Person));
            using (var stream = this.GetType().Assembly.GetManifestResourceStream("Sparkle.LinkedInNET.Tests.Files.FullProfile.xml"))
            {
                item = (Person)serializer.Deserialize(stream);
            }

            Assert.IsNotNull(item.Positions);
            Assert.IsNotNull(item.Positions.Position);
            Assert.AreEqual(5, item.Positions.Position.Count);

            var position = item.Positions.Position[0];
            Assert.AreEqual(1, position.Id);
            Assert.IsNotNull(position.StartDate);
            Assert.AreEqual((short)2013, position.StartDate.Year.Value);
            Assert.AreEqual((short)6, position.StartDate.Month.Value);
            Assert.IsTrue(position.IsCurrent);
            Assert.IsNotNull(position.Company);
            Assert.AreEqual(5161579, position.Company.Id);

            position = item.Positions.Position[1];
            Assert.AreEqual(2, position.Id);
            Assert.IsFalse(string.IsNullOrEmpty(position.Title));
            Assert.IsNotNull(position.StartDate);
            Assert.AreEqual((short)2011, position.StartDate.Year.Value);
            Assert.AreEqual((short)4, position.StartDate.Month.Value);
            Assert.AreEqual((short)2013, position.EndDate.Year.Value);
            Assert.AreEqual((short)5, position.EndDate.Month.Value);
            Assert.IsFalse(position.IsCurrent);
            Assert.IsNotNull(position.Company);
            Assert.AreEqual("Wygwam", position.Company.Name);
        }

        [TestMethod]
        public void FullProfile_RelationToViewer()
        {
            Person item;
            var serializer = new XmlSerializer(typeof(Person));
            using (var stream = this.GetType().Assembly.GetManifestResourceStream("Sparkle.LinkedInNET.Tests.Files.FullProfile.xml"))
            {
                item = (Person)serializer.Deserialize(stream);
            }

            Assert.IsNotNull(item.RelationToViewer);
            ////Assert.AreEqual(2, item.RelationToViewer.Distance);
            Assert.AreEqual(2, item.RelationToViewer.Distance);
        }

        [TestMethod]
        public void FullProfile_CurrentShare()
        {
            Person item;
            var serializer = new XmlSerializer(typeof(Person));
            using (var stream = this.GetType().Assembly.GetManifestResourceStream("Sparkle.LinkedInNET.Tests.Files.FullProfile.xml"))
            {
                item = (Person)serializer.Deserialize(stream);
            }

            Assert.AreEqual(1414145160000, item.CurrentStatusTimestamp);
            Assert.IsNotNull(item.CurrentShare);
            Assert.AreEqual("0000000000", item.CurrentShare.Id);
            Assert.AreEqual(1295953965000, item.CurrentShare.Timestamp);
            Assert.IsNotNull(item.CurrentShare.Visibility);
            Assert.AreEqual("anyone", item.CurrentShare.Visibility.Code);
            Assert.AreEqual("Student at SUPINFO London", item.CurrentShare.Comment);
            Assert.IsNotNull(item.CurrentShare.Source);
            Assert.IsNotNull(item.CurrentShare.Source.ServiceProvider);
            Assert.AreEqual("LINKEDIN", item.CurrentShare.Source.ServiceProvider.Name);
            Assert.IsNotNull(item.CurrentShare.Author);
            Assert.AreEqual("0000000420", item.CurrentShare.Author.Id);
            Assert.AreEqual("Antoine", item.CurrentShare.Author.FirstName);
            Assert.AreEqual("Sottiau", item.CurrentShare.Author.LastName);
        }

        [TestMethod]
        public void FullProfile_ApiStandardProfileRequest()
        {
            Person item;
            var serializer = new XmlSerializer(typeof(Person));
            using (var stream = this.GetType().Assembly.GetManifestResourceStream("Sparkle.LinkedInNET.Tests.Files.FullProfile.xml"))
            {
                item = (Person)serializer.Deserialize(stream);
            }

            Assert.IsNotNull(item.ApiStandardProfileRequest);
            Assert.IsFalse(string.IsNullOrEmpty(item.ApiStandardProfileRequest.Url));
        }
    }
}
