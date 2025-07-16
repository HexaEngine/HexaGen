namespace HexaGen
{
    public interface IConfigComposer
    {
        void Compose(ref CsCodeGeneratorConfig config);
    }
}