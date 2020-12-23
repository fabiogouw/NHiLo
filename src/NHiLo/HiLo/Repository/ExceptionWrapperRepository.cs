using NHiLo;
using NHiLo.HiLo;
using System;

namespace NHiLo.HiLo.Repository
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
                throw new NHiLoException(ErrorCodes.ErrorWhileCreatingTheRepository, ex);
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
                throw new NHiLoException(ErrorCodes.ErrorWhileGettingNextHiValue, ex);
            }
        }

        public void PrepareRepository(string entityName)
        {
            try
            {
                _repository.PrepareRepository(entityName);
            }
            catch (Exception ex)
            {
                throw new NHiLoException(ErrorCodes.ErrorWhilePreparingRepository, ex);
            }
        }
    }
}
