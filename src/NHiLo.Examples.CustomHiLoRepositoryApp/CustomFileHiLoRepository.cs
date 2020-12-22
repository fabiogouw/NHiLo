using NHiLo.HiLo;
using System;
using System.IO;

namespace NHiLo.Examples.CustomHiLoRepositoryApp
{
    class CustomFileHiLoRepository : IHiLoRepository
    {
        private string _fileName;
        public long GetNextHi()
        {
            long currentHi = Convert.ToInt64(File.ReadAllText(_fileName)) + 1;
            File.WriteAllText(_fileName, currentHi.ToString());
            return currentHi;
        }

        public void PrepareRepository(string entityName)
        {
            _fileName = $"{ Environment.CurrentDirectory }/{ entityName }.hilo.key";
            if (File.Exists(_fileName))
            {
                File.Delete(_fileName);
            }
            File.WriteAllText(_fileName, "0");
        }
    }
}
