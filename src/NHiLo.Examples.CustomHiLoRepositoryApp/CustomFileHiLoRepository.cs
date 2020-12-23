using NHiLo.HiLo;
using System;
using System.IO;

namespace NHiLo.Examples.CustomHiLoRepositoryApp
{
    /// <summary>
    /// A custom repository that stores the hi value in flat files in the local disk.
    /// </summary>
    /// <remarks>
    /// This is an example of how to create a custom repository.
    /// </remarks>
    public class CustomFileHiLoRepository : IHiLoRepository
    {
        private string _fileName;

        /// <remarks>
        /// The PrepareRepository method is responsible for initializing the repository so
        /// the hi values can be stored.If the repository writes to a database, this method
        /// can, for example, create the table and initial records needed.
        /// Note that we can prepare the repository for each entity, meaning we can have different
        /// approaches based on the entity.
        /// In this implementation, this repository create a file in local disk named after the
        /// entity (plus the extension ".hilo.key") and writes "0" as its content.
        /// </remarks>
        public void PrepareRepository(string entityName)
        {
            _fileName = $"{ Environment.CurrentDirectory }/{ entityName }.hilo.key";
            if (File.Exists(_fileName))
            {
                File.Delete(_fileName);
            }
            File.WriteAllText(_fileName, "0");
        }

        /// <remarks>
        /// The GetNextHi method is responsible for getting he current hi value from the 
        /// underlying storage, adding one to it so we generate the next hi value, and
        /// store it.
        /// This is the most common approach used by NHiLo's repository implementations
        /// (and all of them consider a pessimistic lock to make the operation atomic), 
        /// but it's up to the developer to fihure out the best way to get the next hi value.
        /// </remarks>
        public long GetNextHi()
        {
            // First we get the current value ...
            long currentHi = Convert.ToInt64(File.ReadAllText(_fileName));
            // ... then add 1 ...
            currentHi = currentHi + 1;
            // ... and finally we update the storage with the new value
            File.WriteAllText(_fileName, currentHi.ToString());
            return currentHi;
        }
    }
}
