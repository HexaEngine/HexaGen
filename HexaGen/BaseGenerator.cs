namespace HexaGen
{
    using HexaGen.Core;

    public delegate void GenEventHandler<TSender, TArgs>(TSender sender, TArgs args);

    public class BaseGenerator : LoggerBase
    {
        protected CsCodeGeneratorConfig config;

        public BaseGenerator(CsCodeGeneratorConfig config)
        {
            this.config = config;
        }

        public CsCodeGeneratorConfig Settings => config;

        public event GenEventHandler<CsCodeGenerator, CsCodeGeneratorConfig>? PostConfigure;

        protected virtual void OnPostConfigure(CsCodeGeneratorConfig config)
        {
            PostConfigure?.Invoke((CsCodeGenerator)this, config);
        }
    }
}