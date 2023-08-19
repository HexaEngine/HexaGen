namespace HexaGen.Tests
{
    using HexaGen.Language;
    using HexaGen.Language.CSharp;
    using HexaGen.Language.CSharp.Nodes;
    using NUnit.Framework.Internal;

    public class CSharpParserTests
    {
        private CSharpParser parser;

        [SetUp]
        public void Setup()
        {
            parser = new();
        }

        [Test]
        public void ParserTestNamespace()
        {
            string input = "namespace TestNamespace\r\n{\r\n}";

            var result = parser.Parse(input, "LeakTracer.cs");

            SyntaxTree expected = new(new RootNode(new()
            {
                new NamespaceNode("TestNamespace")
            }));

            if (result.Diagnostics.HasErrors)
                Assert.Fail(result.Diagnostics.ToString());

            var output = result.SyntaxTree?.BuildDebugTree();

            Assert.That(output, Is.EqualTo(expected.BuildDebugTree()));
        }

        [Test]
        public void ParserTestClass()
        {
            string input = "namespace TestNamespace\r\n{\r\n    public class TestClass\r\n    {\r\n    }\r\n}";

            SyntaxTree expected = new(new RootNode(new()
            {
                new NamespaceNode("TestNamespace", new()
                {
                    new ClassNode("TestClass", new KeywordType[] { KeywordType.Public })
                })
            }));

            var result = parser.Parse(input, "LeakTracer.cs");

            if (result.Diagnostics.HasErrors)
                Assert.Fail(result.Diagnostics.ToString());

            var output = result.SyntaxTree?.BuildDebugTree();

            Assert.That(output, Is.EqualTo(expected.BuildDebugTree()));
        }

        [Test]
        public void ParserTestMethod()
        {
            string input = "namespace TestNamespace\r\n{\r\n    public class TestClass\r\n    {\r\n        public void TestMethod(string s)\r\n        {\r\n        }\r\n    }\r\n}";

            SyntaxTree expected = new(new RootNode(new()
            {
                new NamespaceNode("TestNamespace", new()
                {
                    new ClassNode("TestClass", new KeywordType[] { KeywordType.Public }, new()
                    {
                        new MethodNode("TestMethod", new KeywordType[] { KeywordType.Public }, new string[] { "string", "s" }, "void")
                    })
                })
            }));

            var result = parser.Parse(input, "LeakTracer.cs");

            if (result.Diagnostics.HasErrors)
                Assert.Fail(result.Diagnostics.ToString());

            var output = result.SyntaxTree?.BuildDebugTree();

            Assert.That(output, Is.EqualTo(expected.BuildDebugTree()));
        }

        [Test]
        public void ParserTestClass2Method2()
        {
            string input = "namespace TestNamespace\r\n{\r\n    public class TestClass\r\n    {\r\n        public void TestMethod(string s)\r\n        {\r\n        }\r\n    }\r\n\r\n    public class TestClass2\r\n    {\r\n        public void TestMethod2(string s)\r\n        {\r\n        }\r\n    }\r\n}";

            SyntaxTree expected = new(new RootNode(new()
            {
                new NamespaceNode("TestNamespace", new()
                {
                    new ClassNode("TestClass", new KeywordType[] { KeywordType.Public }, new()
                    {
                        new MethodNode("TestMethod", new KeywordType[] { KeywordType.Public }, new string[] { "string", "s" }, "void")
                    }),
                    new ClassNode("TestClass2", new KeywordType[] { KeywordType.Public }, new()
                    {
                        new MethodNode("TestMethod2", new KeywordType[] { KeywordType.Public }, new string[] { "string", "s" }, "void")
                    })
                })
            }));

            var result = parser.Parse(input, "LeakTracer.cs");

            if (result.Diagnostics.HasErrors)
                Assert.Fail(result.Diagnostics.ToString());

            var output = result.SyntaxTree?.BuildDebugTree();

            Assert.That(output, Is.EqualTo(expected.BuildDebugTree()));
        }

        [Test]
        public void ParserTestClass2Method2Nested()
        {
            string input = "namespace TestNamespace\r\n{\r\n    public class TestClass\r\n    {\r\n        public void TestMethod(string s)\r\n        {\r\n        }\r\n\r\n        public class TestClass2\r\n        {\r\n            public void TestMethod2(string s)\r\n            {\r\n            }\r\n        }\r\n    }\r\n}";

            SyntaxTree expected = new(new RootNode(new()
            {
                new NamespaceNode("TestNamespace", new()
                {
                    new ClassNode("TestClass", new KeywordType[] { KeywordType.Public }, new()
                    {
                        new MethodNode("TestMethod", new KeywordType[] { KeywordType.Public }, new string[] { "string", "s" }, "void"),
                        new ClassNode("TestClass2", new KeywordType[] { KeywordType.Public }, new()
                        {
                            new MethodNode("TestMethod2", new KeywordType[] { KeywordType.Public }, new string[] { "string", "s" }, "void")
                        })
                    })
                })
            }));

            var result = parser.Parse(input, "LeakTracer.cs");

            if (result.Diagnostics.HasErrors)
                Assert.Fail(result.Diagnostics.ToString());

            var output = result.SyntaxTree?.BuildDebugTree();

            Assert.That(output, Is.EqualTo(expected.BuildDebugTree()));
        }

        [Test]
        public void ParserTestFile()
        {
            string input = File.ReadAllText("C:\\Users\\juna\\source\\repos\\HexaGen\\HexaGen.Tests\\LeakTracer.cs");

            var result = parser.Parse(input, "LeakTracer.cs");

            if (result.Diagnostics.HasErrors)
                Assert.Fail(result.Diagnostics.ToString());

            var output = result.SyntaxTree?.BuildDebugTree();

            File.WriteAllText("test.txt", output);

            Assert.Pass();
        }
    }
}