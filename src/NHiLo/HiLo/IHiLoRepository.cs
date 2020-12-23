namespace NHiLo.HiLo
{
    public interface IHiLoRepository
    {
        void PrepareRepository(string entityName);
        long GetNextHi();
    }
}
