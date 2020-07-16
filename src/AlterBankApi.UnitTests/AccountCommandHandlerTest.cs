using AlterBankApi.Application.Commands;
using AlterBankApi.Application.Model;
using AlterBankApi.Application.Handlers;
using AlterBankApi.Infrastructure;
using Moq;
using NUnit.Framework;

namespace AlterBankApi.UnitTests
{
    public class AccountCommandHandlerTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async void FundTransfer_WithEnoughBalance_BalanceUpdated()
        {
            // Arrange
            // Act
            // Assert
            Assert.Pass();
        }
    }
}