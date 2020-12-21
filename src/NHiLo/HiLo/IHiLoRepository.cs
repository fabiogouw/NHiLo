namespace NHiLo.HiLo
{
    public interface IHiLoRepository
    {
        void PrepareRepository();
        long GetNextHi();
    }
}
