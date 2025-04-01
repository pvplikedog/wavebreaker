//Uncomment the next line to enable debugging (also uncomment it in AstarPath.cs)
//#define ProfileAstar //@SHOWINEDITOR
//#define ASTAR_UNITY_PRO_PROFILER //@SHOWINEDITOR Requires ProfileAstar, profiles section of astar code which will show up in the Unity Pro Profiler.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Debug = UnityEngine.Debug;
#if UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;
#endif

namespace Pathfinding
{
    public class AstarProfiler
    {
        private static readonly Dictionary<string, ProfilePoint> profiles = new();
        private static DateTime startTime = DateTime.UtcNow;

        public static ProfilePoint[] fastProfiles;
        public static string[] fastProfileNames;

        private AstarProfiler()
        {
        }

        [Conditional("ProfileAstar")]
        public static void InitializeFastProfile(string[] profileNames)
        {
            fastProfileNames = new string[profileNames.Length + 2];
            Array.Copy(profileNames, fastProfileNames, profileNames.Length);
            fastProfileNames[fastProfileNames.Length - 2] = "__Control1__";
            fastProfileNames[fastProfileNames.Length - 1] = "__Control2__";
            fastProfiles = new ProfilePoint[fastProfileNames.Length];
            for (var i = 0; i < fastProfiles.Length; i++) fastProfiles[i] = new ProfilePoint();
        }

        [Conditional("ProfileAstar")]
        public static void StartFastProfile(int tag)
        {
            //profiles.TryGetValue(tag, out point);
            fastProfiles[tag].watch.Start(); //lastRecorded = DateTime.UtcNow;
        }

        [Conditional("ProfileAstar")]
        public static void EndFastProfile(int tag)
        {
            /*if (!profiles.ContainsKey(tag))
             * {
             *  Debug.LogError("Can only end profiling for a tag which has already been started (tag was " + tag + ")");
             *  return;
             * }*/
            var point = fastProfiles[tag];

            point.totalCalls++;
            point.watch.Stop();
            //DateTime now = DateTime.UtcNow;
            //point.totalTime += now - point.lastRecorded;
            //fastProfiles[tag] = point;
        }

        [Conditional("ASTAR_UNITY_PRO_PROFILER")]
        public static void EndProfile()
        {
            Profiler.EndSample();
        }

        [Conditional("ProfileAstar")]
        public static void StartProfile(string tag)
        {
#if ASTAR_UNITY_PRO_PROFILER
			Profiler.BeginSample(tag);
#else
            //Console.WriteLine ("Profile Start - " + tag);
            ProfilePoint point;

            profiles.TryGetValue(tag, out point);
            if (point == null)
            {
                point = new ProfilePoint();
                profiles[tag] = point;
            }

            point.tmpBytes = GC.GetTotalMemory(false);
            point.watch.Start();
            //point.lastRecorded = DateTime.UtcNow;
            //Debug.Log ("Starting " + tag);
#endif
        }

        [Conditional("ProfileAstar")]
        public static void EndProfile(string tag)
        {
#if !ASTAR_UNITY_PRO_PROFILER
            if (!profiles.ContainsKey(tag))
            {
                Debug.LogError("Can only end profiling for a tag which has already been started (tag was " + tag + ")");
                return;
            }

            //Console.WriteLine ("Profile End - " + tag);
            //DateTime now = DateTime.UtcNow;
            var point = profiles[tag];
            //point.totalTime += now - point.lastRecorded;
            ++point.totalCalls;
            point.watch.Stop();
            point.totalBytes += GC.GetTotalMemory(false) - point.tmpBytes;
            //profiles[tag] = point;
            //Debug.Log ("Ending " + tag);
#else
			EndProfile();
#endif
        }

        [Conditional("ProfileAstar")]
        public static void Reset()
        {
            profiles.Clear();
            startTime = DateTime.UtcNow;

            if (fastProfiles != null)
                for (var i = 0; i < fastProfiles.Length; i++)
                    fastProfiles[i] = new ProfilePoint();
        }

        [Conditional("ProfileAstar")]
        public static void PrintFastResults()
        {
            if (fastProfiles == null)
                return;

            StartFastProfile(fastProfiles.Length - 2);
            for (var i = 0; i < 1000; i++)
            {
                StartFastProfile(fastProfiles.Length - 1);
                EndFastProfile(fastProfiles.Length - 1);
            }

            EndFastProfile(fastProfiles.Length - 2);

            var avgOverhead = fastProfiles[fastProfiles.Length - 2].watch.Elapsed.TotalMilliseconds / 1000.0;

            var endTime = DateTime.UtcNow - startTime;
            var output = new StringBuilder();
            output.Append("============================\n\t\t\t\tProfile results:\n============================\n");
            output.Append("Name		|	Total Time	|	Total Calls	|	Avg/Call	|	Bytes");
            //foreach(KeyValuePair<string, ProfilePoint> pair in profiles)
            for (var i = 0; i < fastProfiles.Length; i++)
            {
                var name = fastProfileNames[i];
                var value = fastProfiles[i];

                var totalCalls = value.totalCalls;
                var totalTime = value.watch.Elapsed.TotalMilliseconds - avgOverhead * totalCalls;

                if (totalCalls < 1) continue;


                output.Append("\n").Append(name.PadLeft(10)).Append("|   ");
                output.Append(totalTime.ToString("0.0 ").PadLeft(10))
                    .Append(value.watch.Elapsed.TotalMilliseconds.ToString("(0.0)").PadLeft(10)).Append("|   ");
                output.Append(totalCalls.ToString().PadLeft(10)).Append("|   ");
                output.Append((totalTime / totalCalls).ToString("0.000").PadLeft(10));


                /* output.Append("\nProfile");
                 * output.Append(name);
                 * output.Append(" took \t");
                 * output.Append(totalTime.ToString("0.0"));
                 * output.Append(" ms to complete over ");
                 * output.Append(totalCalls);
                 * output.Append(" iteration");
                 * if (totalCalls != 1) output.Append("s");
                 * output.Append(", averaging \t");
                 * output.Append((totalTime / totalCalls).ToString("0.000"));
                 * output.Append(" ms per call"); */
            }

            output.Append("\n\n============================\n\t\tTotal runtime: ");
            output.Append(endTime.TotalSeconds.ToString("F3"));
            output.Append(" seconds\n============================");
            Debug.Log(output.ToString());
        }

        [Conditional("ProfileAstar")]
        public static void PrintResults()
        {
            var endTime = DateTime.UtcNow - startTime;
            var output = new StringBuilder();

            output.Append("============================\n\t\t\t\tProfile results:\n============================\n");

            var maxLength = 5;
            foreach (var pair in profiles) maxLength = Math.Max(pair.Key.Length, maxLength);

            output.Append(" Name ".PadRight(maxLength)).Append("|").Append(" Total Time	".PadRight(20)).Append("|")
                .Append(" Total Calls ".PadRight(20)).Append("|").Append(" Avg/Call ".PadRight(20));


            foreach (var pair in profiles)
            {
                var totalTime = pair.Value.watch.Elapsed.TotalMilliseconds;
                var totalCalls = pair.Value.totalCalls;
                if (totalCalls < 1) continue;

                var name = pair.Key;

                output.Append("\n").Append(name.PadRight(maxLength)).Append("| ");
                output.Append(totalTime.ToString("0.0").PadRight(20)).Append("| ");
                output.Append(totalCalls.ToString().PadRight(20)).Append("| ");
                output.Append((totalTime / totalCalls).ToString("0.000").PadRight(20));
                output.Append(AstarMath.FormatBytesBinary((int)pair.Value.totalBytes).PadLeft(10));

                /*output.Append("\nProfile ");
                 * output.Append(pair.Key);
                 * output.Append(" took ");
                 * output.Append(totalTime.ToString("0"));
                 * output.Append(" ms to complete over ");
                 * output.Append(totalCalls);
                 * output.Append(" iteration");
                 * if (totalCalls != 1) output.Append("s");
                 * output.Append(", averaging ");
                 * output.Append((totalTime / totalCalls).ToString("0.0"));
                 * output.Append(" ms per call");*/
            }

            output.Append("\n\n============================\n\t\tTotal runtime: ");
            output.Append(endTime.TotalSeconds.ToString("F3"));
            output.Append(" seconds\n============================");
            Debug.Log(output.ToString());
        }

        public class ProfilePoint
        {
            public long tmpBytes;
            public long totalBytes;

            public int totalCalls;

            //public DateTime lastRecorded;
            //public TimeSpan totalTime;
            public Stopwatch watch = new();
        }
    }
}