namespace HexaGen.Tests
{
    using HexaGen.Core.Logging;
    using HexaGen.Cpp2C;

    public class Cpp2CGeneratorTests
    {
        [SetUp]
        public void Setup()
        {
        }

        private static void EvaluateResult(BaseGenerator generator)
        {
            var messages = generator.Messages;
            int warns = 0;
            int errors = 0;
            for (int i = 0; i < messages.Count; i++)
            {
                var msg = messages[i];
                switch (msg.Severtiy)
                {
                    case LogSeverity.Warning:
                        Assert.Warn(messages[i].Message);
                        warns++;
                        break;

                    case LogSeverity.Error:
                        Assert.Warn(messages[i].Message);
                        errors++;
                        break;

                    case LogSeverity.Critical:
                        Assert.Warn(messages[i].Message);
                        errors++;
                        break;
                }
            }

            if (errors > 0)
                Assert.Fail();
        }

        [Test]
        public void ImGuiTest()
        {
            Cpp2CCodeGeneratorSettings settings = new();
            Cpp2CCodeGenerator generator = new(settings);

            generator.Generate("./cpp2c/imgui/imgui.h", "./cpp2c/results/imgui");

            EvaluateResult(generator);
            Assert.Pass();
        }
    }
}