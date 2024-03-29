﻿using Moneteer.Domain.Exceptions;
using Moneteer.Domain.Guards;
using Moneteer.Domain.Repositories;
using Moq;
using System;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace Moneteer.Domain.Tests.Guards
{
    public class AccountGuardTests
    {
        private Mock<IAccountRepository> _accountRepositoryMock;
        private Mock<IDbConnection> _connectionMock;
        private AccountGuard _sut;

        private Guid _accountId = Guid.NewGuid();
        private Guid _accountOwnerId = Guid.NewGuid();

        public AccountGuardTests()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _connectionMock = new Mock<IDbConnection>();

            _accountRepositoryMock.Setup(r => r.GetOwner(_accountId, It.IsAny<IDbConnection>())).ReturnsAsync(_accountOwnerId);
            
            _sut = new AccountGuard(_accountRepositoryMock.Object);
        }

        [Fact]
        public Task Blocks()
        {
            return Assert.ThrowsAsync<ForbiddenException>(() => _sut.Guard(_accountId, Guid.NewGuid(), _connectionMock.Object));
        }

        [Fact]
        public Task Allows()
        {
            return _sut.Guard(_accountId, _accountOwnerId, _connectionMock.Object);
        }
    }
}
