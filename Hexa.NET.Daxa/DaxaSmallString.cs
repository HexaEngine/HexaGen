namespace Hexa.NET.Daxa
{
    using HexaGen.Runtime;
    using System.Runtime.InteropServices;
    using System.Text;

    [NativeName(NativeNameType.StructOrClass, "daxa_SmallString")]
    [StructLayout(LayoutKind.Sequential)]
    public partial struct DaxaSmallString
    {
        [NativeName(NativeNameType.Field, "data")]
        [NativeName(NativeNameType.Type, "char[63]")]
        public byte Data_0;

        public byte Data_1;
        public byte Data_2;
        public byte Data_3;
        public byte Data_4;
        public byte Data_5;
        public byte Data_6;
        public byte Data_7;
        public byte Data_8;
        public byte Data_9;
        public byte Data_10;
        public byte Data_11;
        public byte Data_12;
        public byte Data_13;
        public byte Data_14;
        public byte Data_15;
        public byte Data_16;
        public byte Data_17;
        public byte Data_18;
        public byte Data_19;
        public byte Data_20;
        public byte Data_21;
        public byte Data_22;
        public byte Data_23;
        public byte Data_24;
        public byte Data_25;
        public byte Data_26;
        public byte Data_27;
        public byte Data_28;
        public byte Data_29;
        public byte Data_30;
        public byte Data_31;
        public byte Data_32;
        public byte Data_33;
        public byte Data_34;
        public byte Data_35;
        public byte Data_36;
        public byte Data_37;
        public byte Data_38;
        public byte Data_39;
        public byte Data_40;
        public byte Data_41;
        public byte Data_42;
        public byte Data_43;
        public byte Data_44;
        public byte Data_45;
        public byte Data_46;
        public byte Data_47;
        public byte Data_48;
        public byte Data_49;
        public byte Data_50;
        public byte Data_51;
        public byte Data_52;
        public byte Data_53;
        public byte Data_54;
        public byte Data_55;
        public byte Data_56;
        public byte Data_57;
        public byte Data_58;
        public byte Data_59;
        public byte Data_60;
        public byte Data_61;
        public byte Data_62;

        [NativeName(NativeNameType.Field, "size")]
        [NativeName(NativeNameType.Type, "uint8_t")]
        public byte Size;

        public unsafe DaxaSmallString(byte* data = default, byte size = default)
        {
            fixed (byte* pData = &Data_0)
            {
                for (int i = 0; i < size; i++)
                {
                    pData[i] = data[i];
                }
            }
            Size = size;
        }

        public unsafe DaxaSmallString(ReadOnlySpan<byte> data = default)
        {
            fixed (byte* pData = &Data_0)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    pData[i] = data[i];
                }
            }

            Size = (byte)data.Length;
        }

        public unsafe DaxaSmallString(string str) : this(Encoding.UTF8.GetBytes(str))
        {
        }

        public static implicit operator DaxaSmallString(string str) => new(str);

        public static implicit operator string(DaxaSmallString str) => str.ToString();

        public override unsafe string ToString()
        {
            fixed (byte* pData = &Data_0)
            {
                return Encoding.UTF8.GetString(new Span<byte>(pData, Size));
            }
        }
    }
}