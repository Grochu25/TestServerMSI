﻿using TestServerMSI.Application.Interfaces;

namespace TestServerMSI.Application.Services
{
    public class StateWriter : IStateWriter
    {
        public StateWriter() { }
        public void SaveToFileStateOfAlghoritm(string path)
        {
            var stream = File.OpenWrite(path);
            stream.Close();
        }
    }
}
