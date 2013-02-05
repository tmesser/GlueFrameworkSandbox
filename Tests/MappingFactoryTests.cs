using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.Company;
using Glue;
using NUnit.Framework;
using Tests.Attributes;

namespace Tests
{
    [TestFixture]
    public class MappingFactoryTests
    {
        /* This verifies that you have an appropriate number of mapping tests for the number of mappers you have.  It's
         * on Ignore right now because I haven't finished writing all the tests yet, so it fails.  It works, though.
         */
        [Test]
        [Ignore]
        public void Verify_Mapping_Classes()
        {
            var numberOfMappers = typeof (MappingFactory).GetProperties()
                .Count(x => 
                        x.PropertyType.IsGenericType && 
                        (x.PropertyType.GetGenericTypeDefinition() == typeof(Mapping<,>))
                    );
            var numberOfTests = typeof(MappingFactoryTests).GetMethods().Count(x => x.IsDefined(typeof(MappingTestAttribute),false));

            Assert.AreEqual(numberOfTests, numberOfMappers);
        }

        
        /* This is a BAD TEST.  AssertMapsCorrectlyToward only tests the RELATE/RELATETOWARDS clauses.
         * When you are exclusively using creators, this will trivially succeed. Do note that this test
         * DOES SUCCEED, and it's not immediately obvious that it's bad.
         */
        [Test]
        public void Verify_DbDateToDateTime_ThisIsABadTest_DangerWillRobinson()
        {
            var verifier = new MappingFactory().DbDateToDateTime.GetMapperVerification();

            Assert.DoesNotThrow(() => verifier.AssertMapsCorrectlyTowards(new DbDate()));
            Assert.DoesNotThrow(() => verifier.AssertMapsCorrectlyTowards(new DateTime()));
        }

        /* This is a better test.  There is no way to test creators with arbitrary values so you must use the
         * old fashioned static invocation.  This is fine, but won't AssertMapsCorrectly has an entire
         * random value generation that verifies your relations with arbitrary, unknown values, which is really
         * what you want when testing mappings like this.
         */
        [Test]
        public void Verify_DbDateToDateTime_ABetterTest()
        {
            var definedDate = new DateTime(2013, 1, 13);
            var definedDbDate = new DbDate {Date = "1/13/2013"};
            var mapper = new MappingFactory().DbDateToDateTime;

            var convertedDate = mapper.Map(definedDbDate);
            var convertedDbDate = mapper.Map(definedDate);

            Assert.AreEqual(definedDate, convertedDate);
            Assert.AreEqual(definedDbDate.Date, convertedDbDate.Date);
        }

        /* This is the sort of test you should insist upon in code reviews.  It tests the creators as well as can be expected,
         * but it also has Asserts that point out that not all properties are related, and that providing a ToObject will fail.
         * This unit test not only tests all potential code usages, it also documents the underlying code.
         */
        [MappingTest]
        [Test]
        public void Verify_DbDateToDateTime_AProperTest()
        {
            var definedDate = new DateTime(2013, 1, 13);
            var definedDbDate = new DbDate { Date = "1/13/2013" };
            var mapper = new MappingFactory().DbDateToDateTime;
            var verifier = mapper.GetRelationsVerification();
            var blankDbDate = new DbDate();
            var blankDate = DateTime.MaxValue; // New DateTime() default is DateTime.MinValue

            var convertedDate = mapper.Map(definedDbDate);
            var convertedDbDate = mapper.Map(definedDate);

            var toBlankDate = mapper.Map(definedDate, blankDbDate);
            var toBlankDbDate = mapper.Map(definedDbDate,blankDate);

            Assert.AreEqual(definedDate, convertedDate);
            Assert.AreEqual(definedDbDate.Date, convertedDbDate.Date);

            Assert.AreNotEqual(blankDbDate, toBlankDbDate);
            Assert.AreNotEqual(blankDate, toBlankDate);
            Assert.AreNotEqual(definedDate, toBlankDate);
            Assert.AreNotEqual(definedDbDate, toBlankDbDate);
            Assert.Throws<GlueException>(verifier.AssertAllPropertiesRelated<DateTime>);
            Assert.Throws<GlueException>(verifier.AssertAllPropertiesRelated<DbDate>);
        }
    }
}
