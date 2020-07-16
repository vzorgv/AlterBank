namespace AlterBankApi.Application.Model
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The account entity
    /// </summary>
    public sealed class Account
    {
        /// <summary>
        /// Account number
        /// </summary>
        [MinLength(1)]
        [MaxLength(10)]
        [Required]
        public string AccountNum { get; set; }

        /// <summary>
        /// Balance amount
        /// </summary>
        public decimal Balance { get; set; }
    }
}
