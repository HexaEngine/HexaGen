namespace HexaGen
{
    public class BaseGenerator : LoggerBase
    {
        protected readonly CsCodeGeneratorConfig config;

        public BaseGenerator(CsCodeGeneratorConfig settings)
        {
            config = settings;
        }

        public CsCodeGeneratorConfig Settings => config;
    }
}