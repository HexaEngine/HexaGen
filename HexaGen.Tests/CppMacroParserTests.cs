namespace HexaGen.Tests
{
    using HexaGen.Language.Cpp;

    public class CppMacroParserTests
    {
        private CppMacroParser parser;

        [SetUp]
        public void Setup()
        {
            parser = new();
        }

        [Test]
        public void TestConstant1()
        {
            var input = "(SDL_INIT_TIMER|SDL_INIT_AUDIO|SDL_INIT_VIDEO|SDL_INIT_EVENTS|SDL_INIT_JOYSTICK|SDL_INIT_HAPTIC|SDL_INIT_GAMECONTROLLER|SDL_INIT_SENSOR)";

            var result = parser.Parse(input, "");
        }

        [Test]
        public void TestConstant2()
        {
            var input = "(1u<<14)";

            var result = parser.Parse(input, "");
        }

        [Test]
        public void TestConstant3()
        {
            var input = "SDL_VERSIONNUM(SDL_MAJOR_VERSION,SDL_MINOR_VERSION,SDL_PATCHLEVEL)";

            var result = parser.Parse(input, "");
        }

        [Test]
        public void TestConstant4()
        {
            var input = "((Uint16)0xFFFF)";

            var result = parser.Parse(input, "");
        }

        [Test]
        public void TestConstant5()
        {
            var input = "__cdecl";

            var result = parser.Parse(input, "");
        }

        [Test]
        public void TestConstant6()
        {
            var input = "(!WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_APP));";

            var result = parser.Parse(input, "");

            string e = result.SyntaxTree.BuildDebugTree();
        }
    }
}