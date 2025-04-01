// Disable some warnings since this class compiles out large parts of the code depending on compiler directives

#pragma warning disable 0162
#pragma warning disable 0414
#pragma warning disable 0429
//#define PROFILE // Uncomment to enable profiling
//#define KEEP_SAMPLES
using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Pathfinding
{
    public class Profile
    {
        private const bool PROFILE_MEM = false;

        public readonly string name;
        private readonly Stopwatch watch;
        private int counter;
        private long mem;
        private long smem;

#if KEEP_SAMPLES
		List<float> samples = new List<float>();
#endif

        private int control = 1 << 30;
        private const bool dontCountFirst = false;

        public int ControlValue()
        {
            return control;
        }

        public Profile(string name)
        {
            this.name = name;
            watch = new Stopwatch();
        }

        public static void WriteCSV(string path, params Profile[] profiles)
        {
#if KEEP_SAMPLES
			var s = new System.Text.StringBuilder();
			s.AppendLine("x, y");
			foreach (var profile in profiles) {
				for (int i = 0; i < profile.samples.Count; i++) {
					s.AppendLine(profile.name + ", " + profile.samples[i].ToString("R"));
				}
			}
			System.IO.File.WriteAllText(path, s.ToString());
#endif
        }

        public void Run(Action action)
        {
            Start();
            action();
            Stop();
        }

        [Conditional("PROFILE")]
        public void Start()
        {
            if (PROFILE_MEM) smem = GC.GetTotalMemory(false);
            if (dontCountFirst && counter == 1) return;
            watch.Start();
        }

        [Conditional("PROFILE")]
        public void Stop()
        {
            counter++;
            if (dontCountFirst && counter == 1) return;

            watch.Stop();
            if (PROFILE_MEM) mem += GC.GetTotalMemory(false) - smem;
#if KEEP_SAMPLES
			samples.Add((float)watch.Elapsed.TotalMilliseconds);
			watch.Reset();
#endif
        }

        [Conditional("PROFILE")]
        /// <summary>Log using Debug.Log</summary>
        public void Log()
        {
            Debug.Log(ToString());
        }

        [Conditional("PROFILE")]
        /// <summary>Log using System.Console</summary>
        public void ConsoleLog()
        {
#if !NETFX_CORE || UNITY_EDITOR
            Console.WriteLine(ToString());
#endif
        }

        [Conditional("PROFILE")]
        public void Stop(int control)
        {
            counter++;
            if (dontCountFirst && counter == 1) return;

            watch.Stop();
            if (PROFILE_MEM) mem += GC.GetTotalMemory(false) - smem;

            if (this.control == 1 << 30) this.control = control;
            else if (this.control != control)
                throw new Exception("Control numbers do not match " + this.control + " != " + control);
        }

        [Conditional("PROFILE")]
        public void Control(Profile other)
        {
            if (ControlValue() != other.ControlValue())
                throw new Exception("Control numbers do not match (" + name + " " + other.name + ") " + ControlValue() +
                                    " != " + other.ControlValue());
        }

        public override string ToString()
        {
            var s = name + " #" + counter + " " + watch.Elapsed.TotalMilliseconds.ToString("0.0 ms") + " avg: " +
                    (watch.Elapsed.TotalMilliseconds / counter).ToString("0.00 ms");

            if (PROFILE_MEM) s += " avg mem: " + (mem / (1.0 * counter)).ToString("0 bytes");
            return s;
        }
    }
}