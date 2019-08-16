using Moneteer.Backend.Managers;
using Moneteer.Models.Validation;

namespace Moneteer.Backend.Tests.Managers
{
    public partial class AccountManagerTests : ManagerTests
    {
        private readonly AccountManager _sut;

        public AccountManagerTests()
        {
            _sut = new AccountManager(BudgetRepository, AccountRepository, 
                EnvelopeRepository, TransactionRepository, TransactionAssignmentRepository,
                new AccountValidationStrategy(), ConnectionProvider, Guards);
        }
    }
}
