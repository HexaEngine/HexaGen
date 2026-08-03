namespace HexaGen.Core.Profiling
{
    using Hexa.NET.Utilities;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// A CPU flame profiler for measuring and visualizing execution time of various stages.
    /// </summary>
    public unsafe class CPUProfiler : ICPUFlameProfiler
    {
        private readonly ImGuiWidgetFlameGraph.ValuesGetter getter;
        private readonly UnsafeList<ProfilerEntry> entries = new(FrameCount);

        private readonly List<string> blockNames = new();
        private readonly Dictionary<string, uint> nameToId = new();
        private readonly Dictionary<uint, string> idToName = new();
        private readonly Queue<uint> destroyQueue = new();

        private uint id;

        private byte currentLevel;
        private int currentFrame = FrameCount - 1;

        private const int FrameCount = 3;

        private bool enabled;

        public static CPUProfiler Global { get; } = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="CPUProfiler"/> class.
        /// </summary>
        public CPUProfiler()
        {
            entries.Resize(FrameCount);
            entries.Erase();
            getter = ProfilerValueGetter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CPUProfiler"/> class with a list of predefined stages.
        /// </summary>
        /// <param name="stages">An array of stage names to be pre-created.</param>
        public CPUProfiler(string[] stages)
        {
            entries.Resize(FrameCount);
            entries.Erase();
            getter = ProfilerValueGetter;
            for (int i = 0; i < stages.Length; i++)
            {
                CreateStage(stages[i]);
            }
        }

        /// <summary>
        /// Gets or sets whether the GPU profiler is enabled.
        /// </summary>
        public bool Enabled { get => enabled; set => enabled = value; }

        /// <summary>
        /// Gets the current profiling entry.
        /// </summary>
        public ProfilerEntry* Current => &entries.Data[currentFrame - 1 < 0 ? FrameCount - 1 : currentFrame - 1];

        /// <summary>
        /// Gets the number of profiling stages created.
        /// </summary>
        public int StageCount => nameToId.Count;

        /// <summary>
        /// Gets the names of the blocks.
        /// </summary>
        public IReadOnlyList<string> BlockNames => blockNames;

        /// <summary>
        /// Gets the flame graph values getter for ImGui visualization.
        /// </summary>
        public ImGuiWidgetFlameGraph.ValuesGetter Getter => getter;

        /// <summary>
        /// Gets or sets a scope by index.
        /// </summary>
        /// <param name="index">The index of the scope to get or set.</param>
        public ref ProfilerScope this[int index]
        {
            get
            {
                return ref entries[currentFrame].Stages.Data[index];
            }
        }

        /// <summary>
        /// Gets or sets a scope by name.
        /// </summary>
        /// <param name="index">The name of the scope to get or set.</param>
        public ref ProfilerScope this[string index]
        {
            get
            {
                return ref entries[currentFrame].Stages.Data[nameToId[index]];
            }
        }

        /// <inheritdoc/>
        double ICPUProfiler.this[string index]
        {
            get
            {
                if (nameToId.TryGetValue(index, out var value))
                {
                    return entries[currentFrame].Stages.Data[value].Duration;
                }
                return 0;
            }
        }

        /// <summary>
        /// Begins a new profiling frame, clearing all previous stage data.
        /// </summary>
        public void BeginFrame()
        {
            if (!enabled)
            {
                return;
            }

            var prevEntry = entries.GetPointer(currentFrame);
            currentFrame = (currentFrame + 1) % FrameCount;
            prevEntry->End = entries.GetPointer(currentFrame)->Start = (ulong)Stopwatch.GetTimestamp();
        }

        /// <summary>
        /// Ends the current profiling frame, processing stage data.
        /// </summary>
        public void EndFrame()
        {
            if (!enabled)
            {
                return;
            }

            lock (blockNames)
            {
                int index = currentFrame;
                int oldIndex = currentFrame - 1 < 0 ? FrameCount - 1 : currentFrame - 1;

                ProfilerEntry* entry = entries.GetPointer(index);
                ProfilerEntry* oldEntry = entries.GetPointer(oldIndex);

                for (int i = 0; i < entry->Stages.Size; i++)
                {
                    ProfilerScope* oldStage = oldEntry->Stages.GetPointer(i);
                    ProfilerScope* stage = entry->Stages.GetPointer(i);

                    if (!stage->Used)
                    {
                        DestroyStage(stage->Id);
                        i--;
                        continue;
                    }
                    stage->Used = false;

                    var fltStart = stage->Start - entry->Start;
                    float sampleStart = fltStart / (float)Stopwatch.Frequency * 1000;

                    var fltEnd = stage->End - entry->Start;
                    float sampleEnd = fltEnd / (float)Stopwatch.Frequency * 1000;

                    if (oldStage->LastStartSample == 0)
                    {
                        oldStage->LastStartSample = sampleStart;
                    }
                    stage->LastStartSample = Lerp(oldStage->LastStartSample, sampleStart, 0.050f);

                    if (oldStage->LastEndSample == 0)
                    {
                        oldStage->LastEndSample = sampleEnd;
                    }
                    stage->LastEndSample = Lerp(oldStage->LastEndSample, sampleEnd, 0.050f);
                }
            }
        }

        private static float Lerp(float start, float end, float amount)
        {
            return start + (end - start) * amount;
        }

        /// <summary>
        /// Creates a new profiling stage with the specified name.
        /// </summary>
        /// <param name="name">The name of the stage to create.</param>
        void ICPUProfiler.CreateStage(string name)
        {
            CreateStage(name);
        }

        /// <summary>
        /// Creates a new profiling stage with the specified name.
        /// </summary>
        /// <param name="name">The name of the stage to create.</param>
        private uint CreateStage(string name)
        {
            lock (blockNames)
            {
                uint currentId = id;
                blockNames.Add(name);
                nameToId.Add(name, currentId);
                idToName.Add(currentId, name);
                for (int i = 0; i < FrameCount; i++)
                {
                    entries.GetPointer(i)->Stages.PushBack(new(currentId, name));
                }

                id++;
                return currentId;
            }
        }

        /// <summary>
        /// Destroys a profiling stage by name.
        /// </summary>
        /// <param name="name">The name of the stage to destroy.</param>
        public void DestroyStage(string name)
        {
            lock (blockNames)
            {
                var id = nameToId[name];
                for (int i = 0; i < FrameCount; i++)
                {
                    var entry = entries.GetPointer(i);
                    var index = entry->IdToIndex(id);
                    var stage = entry->Stages[index];
                    entry->Stages.RemoveAt(index);

                    stage.Name.Release();
                }

                nameToId.Remove(name);
                idToName.Remove(id);
                blockNames.Remove(name);
            }
        }

        /// <summary>
        /// Destroys a profiling stage by ID.
        /// </summary>
        /// <param name="id">The ID of the stage to destroy.</param>
        public void DestroyStage(uint id)
        {
            lock (blockNames)
            {
                var name = idToName[id];
                for (int i = 0; i < FrameCount; i++)
                {
                    var entry = entries.GetPointer(i);
                    var index = entry->IdToIndex(id);
                    var stage = entry->Stages[index];
                    entry->Stages.RemoveAt(index);

                    stage.Name.Release();
                }

                nameToId.Remove(name);
                idToName.Remove(id);
                blockNames.Remove(name);
            }
        }

        /// <summary>
        /// Begins profiling a stage with the specified name.
        /// </summary>
        /// <param name="name">The name of the stage to begin profiling.</param>
        public void Begin(string name)
        {
            if (!enabled)
            {
                return;
            }

            Trace.Assert(currentLevel < 255);

            if (!nameToId.TryGetValue(name, out var id))
            {
                id = CreateStage(name);
            }

            var entry = entries[currentFrame];

            var index = entry.IdToIndex(id);
            var stage = entry.Stages.GetPointer(index);

            stage->Level = currentLevel;
            currentLevel++;
            stage->Start = (ulong)Stopwatch.GetTimestamp();
            stage->Finalized = false;
            stage->Used = true;
        }

        /// <summary>
        /// Ends profiling a stage with the specified name.
        /// </summary>
        /// <param name="name">The name of the stage to end profiling.</param>
        public void End(string name)
        {
            if (!enabled)
            {
                return;
            }

            if (currentLevel == 0)
            {
                return;
            }

            if (!nameToId.ContainsKey(name))
            {
                CreateStage(name);
            }

            var entry = entries[currentFrame];

            var index = entry.IdToIndex(nameToId[name]);
            var stage = entry.Stages.GetPointer(index);

            Trace.Assert(!stage->Finalized);
            currentLevel--;
            Trace.Assert(currentLevel == stage->Level);
            stage->End = (ulong)Stopwatch.GetTimestamp();
            stage->Duration = (stage->End - stage->Start) / (double)Stopwatch.Frequency;
            stage->Finalized = true;
        }

        private readonly struct ProfileBlock : IDisposable
        {
            private readonly CPUProfiler profiler;
            private readonly string name;

            public ProfileBlock(CPUProfiler profiler, string name)
            {
                this.profiler = profiler;
                this.name = name;
            }

            public readonly void Dispose()
            {
                profiler.End(name);
            }
        }

        private readonly struct EmptyProfileBlock : IDisposable
        {
            public readonly void Dispose()
            {
            }
        }

        public IDisposable BeginBlock(string name)
        {
            if (!enabled)
            {
                return new EmptyProfileBlock();
            }

            Begin(name);
            return new ProfileBlock(this, name);
        }

        /// <summary>
        /// Gets the index of the previous entry in the circular buffer.
        /// </summary>
        /// <returns>The index of the previous entry.</returns>
        public int GetCurrentEntryIndex()
        {
            return (currentFrame + FrameCount - 1) % FrameCount;
        }

        private static (double factor, string unit) GetDisplayValues(double value, DisplayOptions displayOption)
        {
            double factor = 1;
            string unit;

            switch (displayOption)
            {
                case DisplayOptions.Milliseconds:
                    factor = 1000;
                    unit = "ms";
                    break;

                case DisplayOptions.Microseconds:
                    factor = 1_000_000;
                    unit = "µs";
                    break;

                case DisplayOptions.Nanoseconds:
                    factor = 1_000_000_000;
                    unit = "ns";
                    break;

                case DisplayOptions.Ticks:
                    factor = Stopwatch.Frequency;
                    unit = "ticks";
                    break;

                case DisplayOptions.Auto:
                default:
                    if (value >= 0.1)
                    {
                        unit = "s";
                    }
                    else if (value >= 0.00001)
                    {
                        factor = 1000;
                        unit = "ms";
                    }
                    else if (value >= 0.000_00001)
                    {
                        factor = 1_000_000;
                        unit = "µs";
                    }
                    else
                    {
                        factor = 1_000_000_000;
                        unit = "ns";
                    }
                    break;
            }

            return (factor, unit);
        }

        /// <summary>
        /// Prints the stages recorded for the current frame.
        /// </summary>
        public void Print()
        {
            if (!enabled)
            {
                return;
            }

            lock (blockNames)
            {
                ProfilerEntry* entry = entries.GetPointer(currentFrame);
                Span<char> durationBuffer = stackalloc char[32];

                Console.WriteLine("CPU Profile:");
                for (int i = 0; i < entry->Stages.Size; i++)
                {
                    ProfilerScope* stage = entry->Stages.GetPointer(i);
                    if (!stage->Finalized)
                    {
                        continue;
                    }

                    for (int j = 0; j < stage->Level; j++)
                    {
                        Console.Write("  ");
                    }

                    Console.Write(blockNames[i]);
                    Console.Write(": ");
                    PrintDuration(stage->Duration, durationBuffer);
                }
            }
        }

        /// <summary>
        /// Prints aggregate statistics for repeated profiling samples.
        /// </summary>
        public void PrintStatistics(string name, long count, double minimum, double maximum, double average)
        {
            if (!enabled || count == 0)
            {
                return;
            }

            Span<char> durationBuffer = stackalloc char[32];
            Console.Write(name);
            Console.WriteLine(" Statistics:");
            Console.Write("  Count: ");
            Console.WriteLine(count);
            Console.Write("  Min: ");
            PrintDuration(minimum, durationBuffer);
            Console.Write("  Max: ");
            PrintDuration(maximum, durationBuffer);
            Console.Write("  Avg: ");
            PrintDuration(average, durationBuffer);
        }

        private static void PrintDuration(double duration, Span<char> durationBuffer)
        {
            (double factor, string unit) = MiniProfiler.GetDisplayValues(duration, DisplayOptions.Auto);
            if ((duration * factor).TryFormat(durationBuffer, out int written, "F3", CultureInfo.InvariantCulture))
            {
                Console.Out.Write(durationBuffer[..written]);
            }
            Console.WriteLine(unit);
        }

        private void ProfilerValueGetter(float* startTimestamp, float* endTimestamp, byte* level, byte** caption, void* data, int idx)
        {
            var entry = (ProfilerEntry*)data;
            ProfilerScope* stage = &entry->Stages.Data[idx];

            if (startTimestamp != null)
            {
                *startTimestamp = stage->LastStartSample;
            }
            if (endTimestamp != null)
            {
                *endTimestamp = stage->LastEndSample;
            }
            if (level != null)
            {
                *level = stage->Level;
            }
            if (caption != null)
            {
                *caption = stage->Name.Data;
            }
        }
    }
}
