using System.ComponentModel;

namespace NHiLo
{
    public enum ErrorCodes : int
    {
        /// <summary>
        /// NHilo was unable to find a connection string to connect to the repository to store
        /// the key values.
        /// </summary>
        [Description("No connection strings available. NHiLo can't identify the database to use.")]
        NoConnectionStringAvailable = 1000,
        /// <summary>
        /// Entity name has invalid characters (not only letters and numbers).
        /// </summary>
        [Description("An entity name must only contains letters and numbers, starting with a letter.")]
        InvalidEntityName = 1001,
        /// <summary>
        /// Entity name has invalid characters (not only letters and numbers).
        /// </summary>
        [Description("A sequence name must only contains letters and numbers, starting with a letter.")]
        InvalidSequencePrefixName = 1002,
        /// <summary>
        /// NHilo could not identify which provider should it use to create repositories.
        /// </summary>
        [Description("No provider name has been supplied.")]
        NoProviderName = 2000,
        /// <summary>
        /// NHilo could not find the specified provider in the list of the implemented providers.
        /// </summary>
        [Description("The provider has not been found in the current available providers.")]
        ProviderNotImplemented = 2001,
        /// <summary>
        /// NHilo could not prepare the repository for the given provider.
        /// </summary>
        [Description("An error occured while creating the repository.")]
        ErrorWhileCreatingTheRepository = 2100,
        /// <summary>
        /// NHilo could not prepare the repository for the given provider.
        /// </summary>
        [Description("An error occured while preparing the repository.")]
        ErrorWhilePreparingRepository = 2101,
        /// <summary>
        /// NHilo could not get the next hi value from the repository.
        /// </summary>
        [Description("An error occured while getting the next hi value from the repository.")]
        ErrorWhileGettingNextHiValue = 2102
    }
}
