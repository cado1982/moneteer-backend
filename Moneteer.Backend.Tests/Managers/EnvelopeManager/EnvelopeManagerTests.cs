using Moneteer.Backend.Managers;

namespace Moneteer.Backend.Tests.Managers
{
    public partial class EnvelopeManagerTests : ManagerTests
    {
        private readonly EnvelopeManager _sut;

        public EnvelopeManagerTests()
        {
            _sut = new EnvelopeManager(ConnectionProvider, EnvelopeRepository, BudgetRepository, Guards);
        }
    }
}
