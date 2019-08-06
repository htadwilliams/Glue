using Glue.PropertyIO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    /// <summary>
    /// 
    /// Copy me for new tests
    /// 
    /// </summary>
    [TestClass]
    public class PropertyTests
    {
        public PropertyBag PropertyBag => propertyBag;

        private readonly PropertyBag propertyBag = new PropertyBag
        {
            // name                 type                // string initializer 
            { "someString",     new PropertyString(     "someStringValue") },
            { "someAnswer",     new PropertyInt(        "42") },
            { "someDuration",   new PropertyDuration(   "4s 323ms") }
        };

        /// <summary>
        /// 
        /// Tests expected use case setting / getting all property types with valid and safe inputs
        /// 
        /// </summary>
        [TestMethod]
        public void TestPropertyDurationHappyPath()
        {
            PropertyDuration propertyDuration = PropertyBag.GetProperty<PropertyDuration>("someDuration");

            Assert.AreEqual(PropertyType.Duration,  propertyDuration.Type);
            Assert.AreEqual("4s 323ms",             propertyDuration.StringValue);
            Assert.AreEqual(4323,                   propertyDuration.Value);
        }

        [TestMethod]
        public void TestPropertyStringHappyPath()
        {
            PropertyString propertyString = PropertyBag.GetProperty<PropertyString>("someString");

            Assert.AreEqual(PropertyType.String,    propertyString.Type);
            Assert.AreEqual("someStringValue",      propertyString.StringValue);
        }

        [TestMethod]
        public void TestPropertyIntHappyPath()
        {
            PropertyInt propertyAnswer = PropertyBag.GetProperty<PropertyInt>("someAnswer");

            Assert.AreEqual(PropertyType.Int,       propertyAnswer.Type);
            Assert.AreEqual("42",                   propertyAnswer.StringValue);
            Assert.AreEqual(42,                     propertyAnswer.Value);
        }

        [TestMethod]
        public void TestPropertyParseError()
        {
            // TODO should throw
            // PropertyDuration propertyBad = new PropertyDuration("bad string! you should be ashamed!");
            // propertyBad.Parse("Evil strings are everywhere! SELECT WHERE ELBOW IN *");
        }

        [TestMethod]
        public void TestPropertyTypecastError()
        {
            // TODO
            // should throw?
            // PropertyDuration propertyBad = propertyBag.GetProperty<PropertyInt>("someDuration");
        }
    }
}
