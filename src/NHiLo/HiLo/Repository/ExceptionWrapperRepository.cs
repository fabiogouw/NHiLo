using NHiLo;
using NHiLo.HiLo;
using System;

namespace NHilo.HiLo.Repository
{
    /// <summary>
    /// Wrap the calls to the underlying repository to handle the errors.
    /// </summary>
    public class ExceptionWrapperRepository : IHiLoRepository
    {
        private readonly IHiLoRepository _repository;
        public ExceptionWrapperRepository(Func<IHiLoRepository> funcCreateRepository)
        {
            try
            {
                _repository = funcCreateRepository();
            }
            catch (Exception ex)
            {
                throw new NHiloException(ErrorCodes.ErrorWhileCreatingTheRepository, ex);
            }
        }
        public long GetNextHi()
        {
            try
            {
                return _repository.GetNextHi();
            }
            catch (Exception ex)
            {
                throw new NHiloException(ErrorCodes.ErrorWhileGettingNextHiValue, ex);
            }
        }

        public void PrepareRepository()
        {
            try
            {
                _repository.PrepareRepository();
            }
            catch (Exception ex)
            {
                throw new NHiloException(ErrorCodes.ErrorWhilePreparingRepository, ex);
            }
        }
    }
}
