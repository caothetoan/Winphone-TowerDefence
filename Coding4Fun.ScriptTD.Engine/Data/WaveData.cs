using System.Collections.Generic;
using Coding4Fun.ScriptTD.Common.Abstracts;

namespace Coding4Fun.ScriptTD.Engine.Data
{
    public class WaveData : Wave
    {
        public readonly List<WavePartData> WaveParts = new List<WavePartData>();
    }
}
