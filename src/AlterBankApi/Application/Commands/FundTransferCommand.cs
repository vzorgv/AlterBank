﻿namespace AlterBankApi.Application.Commands
{
    using System.ComponentModel.DataAnnotations;
    using MediatR;
    using AlterBankApi.Application.Responses;

    /// <summary>
    /// Command to fund transfer
    /// </summary>
    public sealed class FundTransferCommand : IRequest<ExecutionResult<FundTransferResponse>>
    {
        /// <summary>
        /// The account to be withdrawn
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(10)]
        public string AccountNumDebit { get; }

        /// <summary>
        /// The account to be deposited
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(10)]
        public string AccountNumCredit { get; }

        /// <summary>
        /// Transfer amount
        /// </summary>
        public decimal Amount { get; }

        /// <summary>
        /// Constructs class instance
        /// </summary>
        /// <param name="accountNumCredit">The account to be deposited</param>
        /// <param name="accountNumDebit">The account to be withdrew</param>
        /// <param name="amount">Transfer amount</param>
        public FundTransferCommand(string accountNumCredit, string accountNumDebit, decimal amount)
        {
            AccountNumDebit = accountNumDebit;
            AccountNumCredit = accountNumCredit;
            Amount = amount;
        }
    }
}
