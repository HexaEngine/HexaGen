namespace HexaGen.Tests
{
    using HexaGen.Language;

    public class PreprocessorTests
    {
        private Preprocessor preprocessor;

        [SetUp]
        public void Setup()
        {
            preprocessor = new();
        }

        [Test]
        public void PreprocessorTestIf()
        {
            string input = "namespace TestNamespace\r\n{\r\n#if Test\r\n    public class TestClass\r\n    {\r\n        public void TestMethod(string s)\r\n        {\r\n        }\r\n\r\n        public class TestClass2\r\n        {\r\n            public void TestMethod2(string s)\r\n            {\r\n            }\r\n        }\r\n    }\r\n#endif\r\n}";

            var result = preprocessor.Process(input, "");

            //if (result.Diagnostics.HasErrors)
            //    Assert.Fail(result.Diagnostics.ToString());

            // CollectionAssert.AreEqual(expected, result.Tokens);
        }
    }
}