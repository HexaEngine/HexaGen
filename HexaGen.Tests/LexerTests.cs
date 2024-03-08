namespace HexaGen.Tests
{
    using HexaGen.Language;
    using NUnit.Framework.Internal;

    public class LexerTests
    {
        private Lexer lexer;

        [SetUp]
        public void Setup()
        {
            lexer = new();
        }

        [Test]
        public void LexerTestOperator()
        {
            string input = "blah + blah;";

            Token[] expected = new Token[]
            {
                new Token(TokenType.Identifier,0,4, input),
                new Token(TokenType.Operator,5,1, input),
                new Token(TokenType.Identifier,7,4, input),
                new Token(TokenType.Punctuation,11,1, input),
            };

            var result = lexer.Tokenize(input, "");

            if (result.Diagnostics.HasErrors)
                Assert.Fail(result.Diagnostics.ToString());

            if (!expected.SequenceEqual(result.Tokens))
            {
                Assert.Fail();
            }
        }

        [Test]
        public void LexerTestIdentifier()
        {
            string input = "((blah+blah)*blah)";

            Token[] expected = new Token[]
            {
                new Token(TokenType.Punctuation,0,1, input),
                new Token(TokenType.Punctuation,1,1, input),
                new Token(TokenType.Identifier,2,4, input),
                new Token(TokenType.Operator,6,1, input),
                new Token(TokenType.Identifier,7,4, input),
                new Token(TokenType.Punctuation,11,1, input),
                new Token(TokenType.Operator,12,1, input),
                new Token(TokenType.Identifier,13,4, input),
                new Token(TokenType.Punctuation,17,1, input),
            };

            var result = lexer.Tokenize(input, "");

            if (result.Diagnostics.HasErrors)
                Assert.Fail(result.Diagnostics.ToString());

            if (!expected.SequenceEqual(result.Tokens))
            {
                Assert.Fail();
            }
        }

        [Test]
        public void LexerTestNumbers()
        {
            string input = "((1213+2213)*1325)";

            Token[] expected = new Token[]
            {
                new Token(TokenType.Punctuation,0,1, input),
                new Token(TokenType.Punctuation,1,1, input),
                new Token(TokenType.Literal,2,4, input),
                new Token(TokenType.Operator,6,1, input),
                new Token(TokenType.Literal,7,4, input),
                new Token(TokenType.Punctuation,11,1, input),
                new Token(TokenType.Operator,12,1, input),
                new Token(TokenType.Literal,13,4, input),
                new Token(TokenType.Punctuation,17,1, input),
            };

            var result = lexer.Tokenize(input, "");

            if (result.Diagnostics.HasErrors)
                Assert.Fail(result.Diagnostics.ToString());

            if (!expected.SequenceEqual(result.Tokens))
            {
                Assert.Fail();
            }
        }

        [Test]
        public void LexerTestMixed()
        {
            string input = "((1213+blah)*1325)";

            Token[] expected = new Token[]
            {
                new Token(TokenType.Punctuation,0,1, input),
                new Token(TokenType.Punctuation,1,1, input),
                new Token(TokenType.Literal,2,4, input),
                new Token(TokenType.Operator,6,1, input),
                new Token(TokenType.Identifier,7,4, input),
                new Token(TokenType.Punctuation,11,1, input),
                new Token(TokenType.Operator,12,1, input),
                new Token(TokenType.Literal,13,4, input),
                new Token(TokenType.Punctuation,17,1, input),
            };

            var result = lexer.Tokenize(input, "");

            if (result.Diagnostics.HasErrors)
                Assert.Fail(result.Diagnostics.ToString());

            if (!expected.SequenceEqual(result.Tokens))
            {
                Assert.Fail();
            }
        }

        [Test]
        public void LexerTestHexadecimal()
        {
            string input = "((1213+2213)*0xff)";

            Token[] expected = new Token[]
            {
                new Token(TokenType.Punctuation,0,1, input),
                new Token(TokenType.Punctuation,1,1, input),
                new Token(TokenType.Literal,2,4, input),
                new Token(TokenType.Operator,6,1, input),
                new Token(TokenType.Literal,7,4, input),
                new Token(TokenType.Punctuation,11,1, input),
                new Token(TokenType.Operator,12,1, input),
                new Token(TokenType.Literal,13,4, input),
                new Token(TokenType.Punctuation,17,1, input),
            };

            var result = lexer.Tokenize(input, "");

            if (result.Diagnostics.HasErrors)
                Assert.Fail(result.Diagnostics.ToString());

            if (!expected.SequenceEqual(result.Tokens))
            {
                Assert.Fail();
            }
        }

        [Test]
        public void LexerTestSuffix()
        {
            string input = "((1213+2213)*1.4f)";

            Token[] expected = new Token[]
            {
                new Token(TokenType.Punctuation,0,1, input),
                new Token(TokenType.Punctuation,1,1, input),
                new Token(TokenType.Literal,2,4, input),
                new Token(TokenType.Operator,6,1, input),
                new Token(TokenType.Literal,7,4, input),
                new Token(TokenType.Punctuation,11,1, input),
                new Token(TokenType.Operator,12,1, input),
                new Token(TokenType.Literal,13,4, input),
                new Token(TokenType.Punctuation,17,1, input),
            };

            var result = lexer.Tokenize(input, "");

            if (result.Diagnostics.HasErrors)
                Assert.Fail(result.Diagnostics.ToString());

            if (!expected.SequenceEqual(result.Tokens))
            {
                Assert.Fail();
            }
        }

        [Test]
        public void LexerTestExponent()
        {
            string input = "((1213+2213)*1e+5)";

            Token[] expected = new Token[]
            {
                new Token(TokenType.Punctuation,0,1, input),
                new Token(TokenType.Punctuation,1,1, input),
                new Token(TokenType.Literal,2,4, input),
                new Token(TokenType.Operator,6,1, input),
                new Token(TokenType.Literal,7,4, input),
                new Token(TokenType.Punctuation,11,1, input),
                new Token(TokenType.Operator,12,1, input),
                new Token(TokenType.Literal,13,4, input),
                new Token(TokenType.Punctuation,17,1, input),
            };

            var result = lexer.Tokenize(input, "");

            if (result.Diagnostics.HasErrors)
                Assert.Fail(result.Diagnostics.ToString());

            if (!expected.SequenceEqual(result.Tokens))
            {
                Assert.Fail();
            }
        }

        [Test]
        public void LexerTestString()
        {
            string input = "\"Hello World\"";

            Token[] expected = new Token[]
            {
                new Token(TokenType.Literal, 1, input.Length - 2, input),
            };

            var result = lexer.Tokenize(input, "");

            if (result.Diagnostics.HasErrors)
                Assert.Fail(result.Diagnostics.ToString());

            if (!expected.SequenceEqual(result.Tokens))
            {
                Assert.Fail();
            }
        }

        [Test]
        public void LexerTestChar()
        {
            string input = "'\\C'";

            Token[] expected = new Token[]
            {
                new Token(TokenType.Literal, 2, 1, input),
            };

            var result = lexer.Tokenize(input, "");

            if (result.Diagnostics.HasErrors)
                Assert.Fail(result.Diagnostics.ToString());

            if (!expected.SequenceEqual(result.Tokens))
            {
                Assert.Fail();
            }
        }

        [Test]
        public void LexerTestStringMulti()
        {
            string input = "\"Hello World\" + \"1\"";

            Token[] expected = new Token[]
            {
                new Token(TokenType.Literal, 1, 11, input),
                new Token(TokenType.Operator, 14, 1, input),
                new Token(TokenType.Literal, 17, 1, input),
            };

            var result = lexer.Tokenize(input, "");

            if (result.Diagnostics.HasErrors)
                Assert.Fail(result.Diagnostics.ToString());

            if (!expected.SequenceEqual(result.Tokens))
            {
                Assert.Fail();
            }
        }

        [Test]
        public void LexerTestComment()
        {
            string input = "((1213+2213)*1e+5) /* Test Comment */";

            Token[] expected = new Token[]
            {
                new Token(TokenType.Punctuation,0,1, input),
                new Token(TokenType.Punctuation,1,1, input),
                new Token(TokenType.Literal,2,4, input),
                new Token(TokenType.Operator,6,1, input),
                new Token(TokenType.Literal,7,4, input),
                new Token(TokenType.Punctuation,11,1, input),
                new Token(TokenType.Operator,12,1, input),
                new Token(TokenType.Literal,13,4, input),
                new Token(TokenType.Punctuation,17,1, input),
                new Token(TokenType.Comment,21,14, input)
            };

            var result = lexer.Tokenize(input, "");

            if (result.Diagnostics.HasErrors)
                Assert.Fail(result.Diagnostics.ToString());

            if (!expected.SequenceEqual(result.Tokens))
            {
                Assert.Fail();
            }
        }

        [Test]
        public void LexerTestCommentMultiline()
        {
            string input = "((1213+2213)*1e+5) /* Test \n Comment */";

            Token[] expected = new Token[]
            {
                new Token(TokenType.Punctuation,0,1, input),
                new Token(TokenType.Punctuation,1,1, input),
                new Token(TokenType.Literal,2,4, input),
                new Token(TokenType.Operator,6,1, input),
                new Token(TokenType.Literal,7,4, input),
                new Token(TokenType.Punctuation,11,1, input),
                new Token(TokenType.Operator,12,1, input),
                new Token(TokenType.Literal,13,4, input),
                new Token(TokenType.Punctuation,17,1, input),
                new Token(TokenType.Comment,21,16, input)
            };

            var result = lexer.Tokenize(input, "");

            if (result.Diagnostics.HasErrors)
                Assert.Fail(result.Diagnostics.ToString());

            if (!expected.SequenceEqual(result.Tokens))
            {
                Assert.Fail();
            }
        }

        [Test]
        public void LexerTestCommentSingleLine()
        {
            string input = "((1213+2213)*1e+5) // Test Comment";

            Token[] expected = new Token[]
            {
                new Token(TokenType.Punctuation,0,1, input),
                new Token(TokenType.Punctuation,1,1, input),
                new Token(TokenType.Literal,2,4, input),
                new Token(TokenType.Operator,6,1, input),
                new Token(TokenType.Literal,7,4, input),
                new Token(TokenType.Punctuation,11,1, input),
                new Token(TokenType.Operator,12,1, input),
                new Token(TokenType.Literal,13,4, input),
                new Token(TokenType.Punctuation,17,1, input),
                new Token(TokenType.Comment,21,13, input)
            };

            var result = lexer.Tokenize(input, "");

            if (result.Diagnostics.HasErrors)
                Assert.Fail(result.Diagnostics.ToString());

            if (!expected.SequenceEqual(result.Tokens))
            {
                Assert.Fail();
            }
        }

        [Test]
        public void LexerTestFile()
        {
            string input = File.ReadAllText("C:\\Users\\juna\\source\\repos\\HexaGen\\HexaGen.Tests\\LeakTracer.cs");

            var result = lexer.Tokenize(input, "");

            if (result.Diagnostics.HasErrors)
                Assert.Fail(result.Diagnostics.ToString());

            string test = string.Join("\n", result.Diagnostics);

            File.WriteAllText("tokens.txt", test);

            Assert.Pass();
        }

        [Test]
        public void LexerTestCharEofError()
        {
            string input = "((1213+2213)*1e+5) 'd";

            var result = lexer.Tokenize(input, "");

            if (result.Diagnostics.HasErrors)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public void LexerTestCharEof2Error()
        {
            string input = "((1213+2213)*1e+5) '\\d";

            var result = lexer.Tokenize(input, "");

            if (result.Diagnostics.HasErrors)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public void LexerTestCharInvalidError()
        {
            string input = "((1213+2213)*1e+5) '\\dä";

            var result = lexer.Tokenize(input, "");

            if (result.Diagnostics.HasErrors)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public void LexerTestCommentEofError()
        {
            string input = "((1213+2213)*1e+5) /* Comment";

            var result = lexer.Tokenize(input, "");

            if (result.Diagnostics.HasErrors)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public void LexerTestStringEofError()
        {
            string input = "((1213+2213)*1e+5) \"string";

            var result = lexer.Tokenize(input, "");

            if (result.Diagnostics.HasErrors)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }
    }
}