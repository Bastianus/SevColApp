using SevColApp.Models;
using System.Collections.Generic;

namespace SevColApp.Helpers
{
    public static class BankValidator
    {
        public static InputOutputAccountCreate ValidateAccountInput(InputOutputAccountCreate input, List<string> possibleBankNames)
        {
            if (string.IsNullOrEmpty(input.AccountName)) input.Errors.Add("Account name was invalid.");
            else if (input.AccountName.Length > 50) input.Errors.Add("Account name was too long, max length is 50.");

            if (!possibleBankNames.Contains(input.BankName)) input.Errors.Add($"Bank name \"{input.BankName}\" does not exist.");

            if (input.Password == null || input.Password.Length < 1) input.Errors.Add("Password should have at least 1 character.");
            else if (input.Password.Length > 50) input.Errors.Add("Password can have 50 characters max.");

            if (input.WealthLevel != null && input.WealthLevel.Length > 50) input.Errors.Add("Please stop trying to insert ridiculously long non-functional wealth levels, Rik.");

            return input;
        }

        public static Transfer ValidateTransfer(Transfer input, List<string> possibleAccounts)
        {
            var payingAccount = input.PayingAccountNumber;
            if (payingAccount == null) input.Errors.Add("Paying account number was not set.");
            else if (!possibleAccounts.Contains(payingAccount))
            {
                input.Errors.Add($"The paying account \"{payingAccount}\" does not exist.");
            }

            var receivingAccount = input.ReceivingAccountNumber;
            if (receivingAccount == null) input.Errors.Add("Receiving account number was not set.");
            else if (!possibleAccounts.Contains(receivingAccount))
            {
                input.Errors.Add($"The receiving account \"{receivingAccount}\" does not exist.");
            }

            if (input.Amount < 0) input.Errors.Add("Only positive numbers can be transferred.");

            return input;
        }
    }
}
