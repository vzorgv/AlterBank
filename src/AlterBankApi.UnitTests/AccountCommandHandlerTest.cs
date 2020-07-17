using AlterBankApi.Application.Commands;
using AlterBankApi.Application.Model;
using AlterBankApi.Application.Handlers;
using AlterBankApi.Infrastructure;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

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
            //TODO stub for UTs
            // Arrange
            // Act
            // Assert
            await Task.FromResult(0);
            Assert.Pass();
        }
    }
}