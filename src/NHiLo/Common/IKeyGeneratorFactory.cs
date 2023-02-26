namespace NHiLo // this should be available at the root namespace
{
    /// <summary>
    /// Represents the contract used to create objects that generate keys.
    /// </summary>
    /// <typeparam name="T">The type of the key (long, string, etc).</typeparam>
    public interface IKeyGeneratorFactory<out T>
    {
        /// <summary>
        /// Returns the object that produces new key values.
        /// </summary>
        /// <param name="entityName">The name of the entity you'll get the key value for. Must only contains letters and numbers, starting with a letter. Can't have its length more than 100 characters.</param>
        /// <returns></returns>
        IKeyGenerator<T> GetKeyGenerator(string entityName);
    }
}
