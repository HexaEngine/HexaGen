namespace HexaGen.Runtime
{
    public abstract class GLExtension : IDisposable
    {
        protected FunctionTable funcTable = null!;
        protected IGLContext context = null!;
        private readonly int length;

        public GLExtension(int length)
        {
            this.length = length;
        }

        public abstract bool IsSupported(IGLContext context);

        public void Init(IGLContext context)
        {
            this.context = context;
            funcTable = new(context, length);
            InitTable(funcTable);
        }

        protected abstract void InitTable(FunctionTable funcTable);

        public void Dispose()
        {
            funcTable.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}