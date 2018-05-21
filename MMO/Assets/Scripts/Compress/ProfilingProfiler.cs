using UnityEngine;
using System.Collections;

public class ProfilingProfiler {

	public static void BeginSample(string name)
	{
#if UNITY_5_6
        UnityEngine.Profiling.Profiler.BeginSample(name);
#endif
#if UNITY_5_3
		UnityEngine.Profiler.BeginSample(name);
#endif
	}
public static void EndSample()
	{
#if UNITY_5_6
        UnityEngine.Profiling.Profiler.EndSample();
#endif
#if UNITY_5_3
		UnityEngine.Profiler.EndSample();
#endif
	}
    public static long usedHeapSize
    {
        get
        {
   
#if UNITY_5_6
            return UnityEngine.Profiling.Profiler.usedHeapSizeLong;
#endif
#if UNITY_5_3
			return UnityEngine.Profiler.usedHeapSize;
#endif
            return UnityEngine.Profiler.usedHeapSize;
        }
    }
    public static long GetTotalReservedMemory()
    {
#if UNITY_5_6
        return UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong();
#endif
#if UNITY_5_3
		return UnityEngine.Profiler.GetTotalReservedMemory();
#endif
        return UnityEngine.Profiler.GetTotalReservedMemory();
    }
    public static long GetTotalAllocatedMemory()
    {
#if UNITY_5_6
        return UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong();
#endif
#if UNITY_5_3
		return UnityEngine.Profiler.GetTotalAllocatedMemory();
#endif
        return UnityEngine.Profiler.GetTotalAllocatedMemory();
    }
    public static long GetMonoHeapSize()
    {
#if UNITY_5_6
        return UnityEngine.Profiling.Profiler.GetMonoHeapSizeLong();
#endif
#if UNITY_5_3
		return UnityEngine.Profiler.GetMonoHeapSize();
#endif
        return UnityEngine.Profiler.GetMonoHeapSize();
    }
    public static long GetRuntimeMemorySize(UnityEngine.Object o)
    {
#if UNITY_5_6
        return UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(o);
#endif
#if UNITY_5_3
		return UnityEngine.Profiler.GetRuntimeMemorySize(o);
#endif
        return UnityEngine.Profiler.GetRuntimeMemorySize(o);
    }
    public static long GetMonoUsedSize()
    {
#if UNITY_5_6
        return UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong();
#endif
#if UNITY_5_3
		return UnityEngine.Profiler.GetMonoUsedSize();
#endif
        return UnityEngine.Profiler.GetMonoUsedSize();
    }
    public static long GetTotalUnusedReservedMemory()
    {
#if UNITY_5_6
        return UnityEngine.Profiling.Profiler.GetTotalUnusedReservedMemoryLong();
#endif
#if UNITY_5_3
		return UnityEngine.Profiler.GetTotalUnusedReservedMemory();
#endif
        return UnityEngine.Profiler.GetTotalUnusedReservedMemory();
    }
}