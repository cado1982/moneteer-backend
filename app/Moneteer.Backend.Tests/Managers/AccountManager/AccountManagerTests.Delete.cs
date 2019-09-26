using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moneteer.Domain.Exceptions;

namespace Moneteer.Backend.Tests.Managers
{
    public partial class AccountManagerTests : ManagerTests
    {
        [Fact]
        public void Delete_GuardsAccount()
        {
            var accountId = Guid.NewGuid();

            Mock.Get(AccountRepository).Setup(r => r.GetOwner(accountId, DbConnection)).ReturnsAsync(Guid.NewGuid());

            Func<Task> testCase = () => _sut.Delete(accountId, UserId);

            testCase.Should().Throw<ForbiddenException>();
        }

        [Fact]
        public async Task Delete_CallsRepository()
        {
            var accountId = Guid.NewGuid();

            await _sut.Delete(accountId, UserId);

            Mock.Get(AccountRepository).Verify(r => r.Delete(accountId, DbConnection));
        }

    }
}
