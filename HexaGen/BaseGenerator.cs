namespace HexaGen
{
    using HexaGen.Core.Logging;
    using System;
    using System.Collections.Generic;

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