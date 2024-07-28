namespace Hexa.NET.VMA
{
    using HexaGen.Runtime;
    using System.Runtime.InteropServices;

    [NativeName(NativeNameType.StructOrClass, "VmaCurrentBudgetData")]
    [StructLayout(LayoutKind.Sequential)]
    public partial struct VmaCurrentBudgetData
    {
        [NativeName(NativeNameType.Field, "m_BlockCount")]
        [NativeName(NativeNameType.Type, "atomic[16]")]
        public Atomic<uint> MBlockCount_0;

        public Atomic<uint> MBlockCount_1;
        public Atomic<uint> MBlockCount_2;
        public Atomic<uint> MBlockCount_3;
        public Atomic<uint> MBlockCount_4;
        public Atomic<uint> MBlockCount_5;
        public Atomic<uint> MBlockCount_6;
        public Atomic<uint> MBlockCount_7;
        public Atomic<uint> MBlockCount_8;
        public Atomic<uint> MBlockCount_9;
        public Atomic<uint> MBlockCount_10;
        public Atomic<uint> MBlockCount_11;
        public Atomic<uint> MBlockCount_12;
        public Atomic<uint> MBlockCount_13;
        public Atomic<uint> MBlockCount_14;
        public Atomic<uint> MBlockCount_15;

        [NativeName(NativeNameType.Field, "m_AllocationCount")]
        [NativeName(NativeNameType.Type, "atomic[16]")]
        public Atomic<uint> MAllocationCount_0;

        public Atomic<uint> MAllocationCount_1;
        public Atomic<uint> MAllocationCount_2;
        public Atomic<uint> MAllocationCount_3;
        public Atomic<uint> MAllocationCount_4;
        public Atomic<uint> MAllocationCount_5;
        public Atomic<uint> MAllocationCount_6;
        public Atomic<uint> MAllocationCount_7;
        public Atomic<uint> MAllocationCount_8;
        public Atomic<uint> MAllocationCount_9;
        public Atomic<uint> MAllocationCount_10;
        public Atomic<uint> MAllocationCount_11;
        public Atomic<uint> MAllocationCount_12;
        public Atomic<uint> MAllocationCount_13;
        public Atomic<uint> MAllocationCount_14;
        public Atomic<uint> MAllocationCount_15;

        [NativeName(NativeNameType.Field, "m_BlockBytes")]
        [NativeName(NativeNameType.Type, "atomic[16]")]
        public Atomic<ulong> MBlockBytes_0;

        public Atomic<ulong> MBlockBytes_1;
        public Atomic<ulong> MBlockBytes_2;
        public Atomic<ulong> MBlockBytes_3;
        public Atomic<ulong> MBlockBytes_4;
        public Atomic<ulong> MBlockBytes_5;
        public Atomic<ulong> MBlockBytes_6;
        public Atomic<ulong> MBlockBytes_7;
        public Atomic<ulong> MBlockBytes_8;
        public Atomic<ulong> MBlockBytes_9;
        public Atomic<ulong> MBlockBytes_10;
        public Atomic<ulong> MBlockBytes_11;
        public Atomic<ulong> MBlockBytes_12;
        public Atomic<ulong> MBlockBytes_13;
        public Atomic<ulong> MBlockBytes_14;
        public Atomic<ulong> MBlockBytes_15;

        [NativeName(NativeNameType.Field, "m_AllocationBytes")]
        [NativeName(NativeNameType.Type, "atomic[16]")]
        public Atomic<ulong> MAllocationBytes_0;

        public Atomic<ulong> MAllocationBytes_1;
        public Atomic<ulong> MAllocationBytes_2;
        public Atomic<ulong> MAllocationBytes_3;
        public Atomic<ulong> MAllocationBytes_4;
        public Atomic<ulong> MAllocationBytes_5;
        public Atomic<ulong> MAllocationBytes_6;
        public Atomic<ulong> MAllocationBytes_7;
        public Atomic<ulong> MAllocationBytes_8;
        public Atomic<ulong> MAllocationBytes_9;
        public Atomic<ulong> MAllocationBytes_10;
        public Atomic<ulong> MAllocationBytes_11;
        public Atomic<ulong> MAllocationBytes_12;
        public Atomic<ulong> MAllocationBytes_13;
        public Atomic<ulong> MAllocationBytes_14;
        public Atomic<ulong> MAllocationBytes_15;

        [NativeName(NativeNameType.Field, "m_OperationsSinceBudgetFetch")]
        [NativeName(NativeNameType.Type, "atomic")]
        public Atomic<uint> MOperationsSinceBudgetFetch;

        [NativeName(NativeNameType.Field, "m_BudgetMutex")]
        [NativeName(NativeNameType.Type, "VmaRWMutex")]
        public unsafe void* MBudgetMutex;

        [NativeName(NativeNameType.Field, "m_VulkanUsage")]
        [NativeName(NativeNameType.Type, "uint64_t[16]")]
        public ulong MVulkanUsage_0;

        public ulong MVulkanUsage_1;
        public ulong MVulkanUsage_2;
        public ulong MVulkanUsage_3;
        public ulong MVulkanUsage_4;
        public ulong MVulkanUsage_5;
        public ulong MVulkanUsage_6;
        public ulong MVulkanUsage_7;
        public ulong MVulkanUsage_8;
        public ulong MVulkanUsage_9;
        public ulong MVulkanUsage_10;
        public ulong MVulkanUsage_11;
        public ulong MVulkanUsage_12;
        public ulong MVulkanUsage_13;
        public ulong MVulkanUsage_14;
        public ulong MVulkanUsage_15;

        [NativeName(NativeNameType.Field, "m_VulkanBudget")]
        [NativeName(NativeNameType.Type, "uint64_t[16]")]
        public ulong MVulkanBudget_0;

        public ulong MVulkanBudget_1;
        public ulong MVulkanBudget_2;
        public ulong MVulkanBudget_3;
        public ulong MVulkanBudget_4;
        public ulong MVulkanBudget_5;
        public ulong MVulkanBudget_6;
        public ulong MVulkanBudget_7;
        public ulong MVulkanBudget_8;
        public ulong MVulkanBudget_9;
        public ulong MVulkanBudget_10;
        public ulong MVulkanBudget_11;
        public ulong MVulkanBudget_12;
        public ulong MVulkanBudget_13;
        public ulong MVulkanBudget_14;
        public ulong MVulkanBudget_15;

        [NativeName(NativeNameType.Field, "m_BlockBytesAtBudgetFetch")]
        [NativeName(NativeNameType.Type, "uint64_t[16]")]
        public ulong MBlockBytesAtBudgetFetch_0;

        public ulong MBlockBytesAtBudgetFetch_1;
        public ulong MBlockBytesAtBudgetFetch_2;
        public ulong MBlockBytesAtBudgetFetch_3;
        public ulong MBlockBytesAtBudgetFetch_4;
        public ulong MBlockBytesAtBudgetFetch_5;
        public ulong MBlockBytesAtBudgetFetch_6;
        public ulong MBlockBytesAtBudgetFetch_7;
        public ulong MBlockBytesAtBudgetFetch_8;
        public ulong MBlockBytesAtBudgetFetch_9;
        public ulong MBlockBytesAtBudgetFetch_10;
        public ulong MBlockBytesAtBudgetFetch_11;
        public ulong MBlockBytesAtBudgetFetch_12;
        public ulong MBlockBytesAtBudgetFetch_13;
        public ulong MBlockBytesAtBudgetFetch_14;
        public ulong MBlockBytesAtBudgetFetch_15;

        public unsafe VmaCurrentBudgetData(Atomic<uint>* mBlockcount = default, Atomic<uint>* mAllocationcount = default, Atomic<ulong>* mBlockbytes = default, Atomic<ulong>* mAllocationbytes = default, Atomic<uint> mOperationssincebudgetfetch = default, void* mBudgetmutex = default, ulong* mVulkanusage = default, ulong* mVulkanbudget = default, ulong* mBlockbytesatbudgetfetch = default)
        {
            if (mBlockcount != default)
            {
                MBlockCount_0 = mBlockcount[0];
                MBlockCount_1 = mBlockcount[1];
                MBlockCount_2 = mBlockcount[2];
                MBlockCount_3 = mBlockcount[3];
                MBlockCount_4 = mBlockcount[4];
                MBlockCount_5 = mBlockcount[5];
                MBlockCount_6 = mBlockcount[6];
                MBlockCount_7 = mBlockcount[7];
                MBlockCount_8 = mBlockcount[8];
                MBlockCount_9 = mBlockcount[9];
                MBlockCount_10 = mBlockcount[10];
                MBlockCount_11 = mBlockcount[11];
                MBlockCount_12 = mBlockcount[12];
                MBlockCount_13 = mBlockcount[13];
                MBlockCount_14 = mBlockcount[14];
                MBlockCount_15 = mBlockcount[15];
            }
            if (mAllocationcount != default)
            {
                MAllocationCount_0 = mAllocationcount[0];
                MAllocationCount_1 = mAllocationcount[1];
                MAllocationCount_2 = mAllocationcount[2];
                MAllocationCount_3 = mAllocationcount[3];
                MAllocationCount_4 = mAllocationcount[4];
                MAllocationCount_5 = mAllocationcount[5];
                MAllocationCount_6 = mAllocationcount[6];
                MAllocationCount_7 = mAllocationcount[7];
                MAllocationCount_8 = mAllocationcount[8];
                MAllocationCount_9 = mAllocationcount[9];
                MAllocationCount_10 = mAllocationcount[10];
                MAllocationCount_11 = mAllocationcount[11];
                MAllocationCount_12 = mAllocationcount[12];
                MAllocationCount_13 = mAllocationcount[13];
                MAllocationCount_14 = mAllocationcount[14];
                MAllocationCount_15 = mAllocationcount[15];
            }
            if (mBlockbytes != default)
            {
                MBlockBytes_0 = mBlockbytes[0];
                MBlockBytes_1 = mBlockbytes[1];
                MBlockBytes_2 = mBlockbytes[2];
                MBlockBytes_3 = mBlockbytes[3];
                MBlockBytes_4 = mBlockbytes[4];
                MBlockBytes_5 = mBlockbytes[5];
                MBlockBytes_6 = mBlockbytes[6];
                MBlockBytes_7 = mBlockbytes[7];
                MBlockBytes_8 = mBlockbytes[8];
                MBlockBytes_9 = mBlockbytes[9];
                MBlockBytes_10 = mBlockbytes[10];
                MBlockBytes_11 = mBlockbytes[11];
                MBlockBytes_12 = mBlockbytes[12];
                MBlockBytes_13 = mBlockbytes[13];
                MBlockBytes_14 = mBlockbytes[14];
                MBlockBytes_15 = mBlockbytes[15];
            }
            if (mAllocationbytes != default)
            {
                MAllocationBytes_0 = mAllocationbytes[0];
                MAllocationBytes_1 = mAllocationbytes[1];
                MAllocationBytes_2 = mAllocationbytes[2];
                MAllocationBytes_3 = mAllocationbytes[3];
                MAllocationBytes_4 = mAllocationbytes[4];
                MAllocationBytes_5 = mAllocationbytes[5];
                MAllocationBytes_6 = mAllocationbytes[6];
                MAllocationBytes_7 = mAllocationbytes[7];
                MAllocationBytes_8 = mAllocationbytes[8];
                MAllocationBytes_9 = mAllocationbytes[9];
                MAllocationBytes_10 = mAllocationbytes[10];
                MAllocationBytes_11 = mAllocationbytes[11];
                MAllocationBytes_12 = mAllocationbytes[12];
                MAllocationBytes_13 = mAllocationbytes[13];
                MAllocationBytes_14 = mAllocationbytes[14];
                MAllocationBytes_15 = mAllocationbytes[15];
            }
            MOperationsSinceBudgetFetch = mOperationssincebudgetfetch;
            MBudgetMutex = mBudgetmutex;
            if (mVulkanusage != default)
            {
                MVulkanUsage_0 = mVulkanusage[0];
                MVulkanUsage_1 = mVulkanusage[1];
                MVulkanUsage_2 = mVulkanusage[2];
                MVulkanUsage_3 = mVulkanusage[3];
                MVulkanUsage_4 = mVulkanusage[4];
                MVulkanUsage_5 = mVulkanusage[5];
                MVulkanUsage_6 = mVulkanusage[6];
                MVulkanUsage_7 = mVulkanusage[7];
                MVulkanUsage_8 = mVulkanusage[8];
                MVulkanUsage_9 = mVulkanusage[9];
                MVulkanUsage_10 = mVulkanusage[10];
                MVulkanUsage_11 = mVulkanusage[11];
                MVulkanUsage_12 = mVulkanusage[12];
                MVulkanUsage_13 = mVulkanusage[13];
                MVulkanUsage_14 = mVulkanusage[14];
                MVulkanUsage_15 = mVulkanusage[15];
            }
            if (mVulkanbudget != default)
            {
                MVulkanBudget_0 = mVulkanbudget[0];
                MVulkanBudget_1 = mVulkanbudget[1];
                MVulkanBudget_2 = mVulkanbudget[2];
                MVulkanBudget_3 = mVulkanbudget[3];
                MVulkanBudget_4 = mVulkanbudget[4];
                MVulkanBudget_5 = mVulkanbudget[5];
                MVulkanBudget_6 = mVulkanbudget[6];
                MVulkanBudget_7 = mVulkanbudget[7];
                MVulkanBudget_8 = mVulkanbudget[8];
                MVulkanBudget_9 = mVulkanbudget[9];
                MVulkanBudget_10 = mVulkanbudget[10];
                MVulkanBudget_11 = mVulkanbudget[11];
                MVulkanBudget_12 = mVulkanbudget[12];
                MVulkanBudget_13 = mVulkanbudget[13];
                MVulkanBudget_14 = mVulkanbudget[14];
                MVulkanBudget_15 = mVulkanbudget[15];
            }
            if (mBlockbytesatbudgetfetch != default)
            {
                MBlockBytesAtBudgetFetch_0 = mBlockbytesatbudgetfetch[0];
                MBlockBytesAtBudgetFetch_1 = mBlockbytesatbudgetfetch[1];
                MBlockBytesAtBudgetFetch_2 = mBlockbytesatbudgetfetch[2];
                MBlockBytesAtBudgetFetch_3 = mBlockbytesatbudgetfetch[3];
                MBlockBytesAtBudgetFetch_4 = mBlockbytesatbudgetfetch[4];
                MBlockBytesAtBudgetFetch_5 = mBlockbytesatbudgetfetch[5];
                MBlockBytesAtBudgetFetch_6 = mBlockbytesatbudgetfetch[6];
                MBlockBytesAtBudgetFetch_7 = mBlockbytesatbudgetfetch[7];
                MBlockBytesAtBudgetFetch_8 = mBlockbytesatbudgetfetch[8];
                MBlockBytesAtBudgetFetch_9 = mBlockbytesatbudgetfetch[9];
                MBlockBytesAtBudgetFetch_10 = mBlockbytesatbudgetfetch[10];
                MBlockBytesAtBudgetFetch_11 = mBlockbytesatbudgetfetch[11];
                MBlockBytesAtBudgetFetch_12 = mBlockbytesatbudgetfetch[12];
                MBlockBytesAtBudgetFetch_13 = mBlockbytesatbudgetfetch[13];
                MBlockBytesAtBudgetFetch_14 = mBlockbytesatbudgetfetch[14];
                MBlockBytesAtBudgetFetch_15 = mBlockbytesatbudgetfetch[15];
            }
        }

        public unsafe VmaCurrentBudgetData(Span<Atomic<uint>> mBlockcount = default, Span<Atomic<uint>> mAllocationcount = default, Span<Atomic<ulong>> mBlockbytes = default, Span<Atomic<ulong>> mAllocationbytes = default, Atomic<uint> mOperationssincebudgetfetch = default, void* mBudgetmutex = default, Span<ulong> mVulkanusage = default, Span<ulong> mVulkanbudget = default, Span<ulong> mBlockbytesatbudgetfetch = default)
        {
            if (mBlockcount != default)
            {
                MBlockCount_0 = mBlockcount[0];
                MBlockCount_1 = mBlockcount[1];
                MBlockCount_2 = mBlockcount[2];
                MBlockCount_3 = mBlockcount[3];
                MBlockCount_4 = mBlockcount[4];
                MBlockCount_5 = mBlockcount[5];
                MBlockCount_6 = mBlockcount[6];
                MBlockCount_7 = mBlockcount[7];
                MBlockCount_8 = mBlockcount[8];
                MBlockCount_9 = mBlockcount[9];
                MBlockCount_10 = mBlockcount[10];
                MBlockCount_11 = mBlockcount[11];
                MBlockCount_12 = mBlockcount[12];
                MBlockCount_13 = mBlockcount[13];
                MBlockCount_14 = mBlockcount[14];
                MBlockCount_15 = mBlockcount[15];
            }
            if (mAllocationcount != default)
            {
                MAllocationCount_0 = mAllocationcount[0];
                MAllocationCount_1 = mAllocationcount[1];
                MAllocationCount_2 = mAllocationcount[2];
                MAllocationCount_3 = mAllocationcount[3];
                MAllocationCount_4 = mAllocationcount[4];
                MAllocationCount_5 = mAllocationcount[5];
                MAllocationCount_6 = mAllocationcount[6];
                MAllocationCount_7 = mAllocationcount[7];
                MAllocationCount_8 = mAllocationcount[8];
                MAllocationCount_9 = mAllocationcount[9];
                MAllocationCount_10 = mAllocationcount[10];
                MAllocationCount_11 = mAllocationcount[11];
                MAllocationCount_12 = mAllocationcount[12];
                MAllocationCount_13 = mAllocationcount[13];
                MAllocationCount_14 = mAllocationcount[14];
                MAllocationCount_15 = mAllocationcount[15];
            }
            if (mBlockbytes != default)
            {
                MBlockBytes_0 = mBlockbytes[0];
                MBlockBytes_1 = mBlockbytes[1];
                MBlockBytes_2 = mBlockbytes[2];
                MBlockBytes_3 = mBlockbytes[3];
                MBlockBytes_4 = mBlockbytes[4];
                MBlockBytes_5 = mBlockbytes[5];
                MBlockBytes_6 = mBlockbytes[6];
                MBlockBytes_7 = mBlockbytes[7];
                MBlockBytes_8 = mBlockbytes[8];
                MBlockBytes_9 = mBlockbytes[9];
                MBlockBytes_10 = mBlockbytes[10];
                MBlockBytes_11 = mBlockbytes[11];
                MBlockBytes_12 = mBlockbytes[12];
                MBlockBytes_13 = mBlockbytes[13];
                MBlockBytes_14 = mBlockbytes[14];
                MBlockBytes_15 = mBlockbytes[15];
            }
            if (mAllocationbytes != default)
            {
                MAllocationBytes_0 = mAllocationbytes[0];
                MAllocationBytes_1 = mAllocationbytes[1];
                MAllocationBytes_2 = mAllocationbytes[2];
                MAllocationBytes_3 = mAllocationbytes[3];
                MAllocationBytes_4 = mAllocationbytes[4];
                MAllocationBytes_5 = mAllocationbytes[5];
                MAllocationBytes_6 = mAllocationbytes[6];
                MAllocationBytes_7 = mAllocationbytes[7];
                MAllocationBytes_8 = mAllocationbytes[8];
                MAllocationBytes_9 = mAllocationbytes[9];
                MAllocationBytes_10 = mAllocationbytes[10];
                MAllocationBytes_11 = mAllocationbytes[11];
                MAllocationBytes_12 = mAllocationbytes[12];
                MAllocationBytes_13 = mAllocationbytes[13];
                MAllocationBytes_14 = mAllocationbytes[14];
                MAllocationBytes_15 = mAllocationbytes[15];
            }
            MOperationsSinceBudgetFetch = mOperationssincebudgetfetch;
            MBudgetMutex = mBudgetmutex;
            if (mVulkanusage != default)
            {
                MVulkanUsage_0 = mVulkanusage[0];
                MVulkanUsage_1 = mVulkanusage[1];
                MVulkanUsage_2 = mVulkanusage[2];
                MVulkanUsage_3 = mVulkanusage[3];
                MVulkanUsage_4 = mVulkanusage[4];
                MVulkanUsage_5 = mVulkanusage[5];
                MVulkanUsage_6 = mVulkanusage[6];
                MVulkanUsage_7 = mVulkanusage[7];
                MVulkanUsage_8 = mVulkanusage[8];
                MVulkanUsage_9 = mVulkanusage[9];
                MVulkanUsage_10 = mVulkanusage[10];
                MVulkanUsage_11 = mVulkanusage[11];
                MVulkanUsage_12 = mVulkanusage[12];
                MVulkanUsage_13 = mVulkanusage[13];
                MVulkanUsage_14 = mVulkanusage[14];
                MVulkanUsage_15 = mVulkanusage[15];
            }
            if (mVulkanbudget != default)
            {
                MVulkanBudget_0 = mVulkanbudget[0];
                MVulkanBudget_1 = mVulkanbudget[1];
                MVulkanBudget_2 = mVulkanbudget[2];
                MVulkanBudget_3 = mVulkanbudget[3];
                MVulkanBudget_4 = mVulkanbudget[4];
                MVulkanBudget_5 = mVulkanbudget[5];
                MVulkanBudget_6 = mVulkanbudget[6];
                MVulkanBudget_7 = mVulkanbudget[7];
                MVulkanBudget_8 = mVulkanbudget[8];
                MVulkanBudget_9 = mVulkanbudget[9];
                MVulkanBudget_10 = mVulkanbudget[10];
                MVulkanBudget_11 = mVulkanbudget[11];
                MVulkanBudget_12 = mVulkanbudget[12];
                MVulkanBudget_13 = mVulkanbudget[13];
                MVulkanBudget_14 = mVulkanbudget[14];
                MVulkanBudget_15 = mVulkanbudget[15];
            }
            if (mBlockbytesatbudgetfetch != default)
            {
                MBlockBytesAtBudgetFetch_0 = mBlockbytesatbudgetfetch[0];
                MBlockBytesAtBudgetFetch_1 = mBlockbytesatbudgetfetch[1];
                MBlockBytesAtBudgetFetch_2 = mBlockbytesatbudgetfetch[2];
                MBlockBytesAtBudgetFetch_3 = mBlockbytesatbudgetfetch[3];
                MBlockBytesAtBudgetFetch_4 = mBlockbytesatbudgetfetch[4];
                MBlockBytesAtBudgetFetch_5 = mBlockbytesatbudgetfetch[5];
                MBlockBytesAtBudgetFetch_6 = mBlockbytesatbudgetfetch[6];
                MBlockBytesAtBudgetFetch_7 = mBlockbytesatbudgetfetch[7];
                MBlockBytesAtBudgetFetch_8 = mBlockbytesatbudgetfetch[8];
                MBlockBytesAtBudgetFetch_9 = mBlockbytesatbudgetfetch[9];
                MBlockBytesAtBudgetFetch_10 = mBlockbytesatbudgetfetch[10];
                MBlockBytesAtBudgetFetch_11 = mBlockbytesatbudgetfetch[11];
                MBlockBytesAtBudgetFetch_12 = mBlockbytesatbudgetfetch[12];
                MBlockBytesAtBudgetFetch_13 = mBlockbytesatbudgetfetch[13];
                MBlockBytesAtBudgetFetch_14 = mBlockbytesatbudgetfetch[14];
                MBlockBytesAtBudgetFetch_15 = mBlockbytesatbudgetfetch[15];
            }
        }

        public unsafe Span<Atomic<uint>> MBlockCount

        {
            get
            {
                fixed (Atomic<uint>* p = &this.MBlockCount_0)
                {
                    return new Span<Atomic<uint>>(p, 16);
                }
            }
        }

        public unsafe Span<Atomic<uint>> MAllocationCount

        {
            get
            {
                fixed (Atomic<uint>* p = &this.MAllocationCount_0)
                {
                    return new Span<Atomic<uint>>(p, 16);
                }
            }
        }

        public unsafe Span<Atomic<ulong>> MBlockBytes

        {
            get
            {
                fixed (Atomic<ulong>* p = &this.MBlockBytes_0)
                {
                    return new Span<Atomic<ulong>>(p, 16);
                }
            }
        }

        public unsafe Span<Atomic<ulong>> MAllocationBytes

        {
            get
            {
                fixed (Atomic<ulong>* p = &this.MAllocationBytes_0)
                {
                    return new Span<Atomic<ulong>>(p, 16);
                }
            }
        }
    }
}