using System;
using System.Collections.Generic;
using FubuCore;

namespace ripple.Commands.Samples
{
    public class FileCollector
    {
        private readonly string _file;

        public FileCollector(string file)
        {
            _file = file;
        }

        private readonly IList<SampleCollector> _collectors = new List<SampleCollector>();

        public IList<SampleCollector> Read()
        {
            new FileSystem().ReadTextFile(_file, readLine);

            return _collectors;
        }

        private void readLine(string line)
        {
            if (line.TrimStart().StartsWith("// SAMPLE:"))
            {
                var sampleName = line.Split(':')[1].Trim();
                Console.WriteLine("    found '{0}'", sampleName);
                _collectors.Add(new SampleCollector(sampleName));
            }

            _collectors.Each(x => x.AddLine(line));
        }
    }
}