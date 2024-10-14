namespace HexaGen
{
    public class BaseGenerator : LoggerBase
    {
        protected readonly CsCodeGeneratorConfig config;

        public BaseGenerator(CsCodeGeneratorConfig settings)
        {
            this.config = settings;
            settings.TypeMappings.Add("HRESULT", "HResult");
        }

        public CsCodeGeneratorConfig Settings => config;
    }
}