using Microsoft.EntityFrameworkCore;
using SevColApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SevColApp.Repositories
{
    public class BankRepository : IBankRepository
    {
        private SevColContext _context;
        public BankRepository(SevColContext context)
        {
            _context = context;
        }
        public async Task<List<BankAccount>> GetBankAccountsOfUser(int userId)
        {
            var accounts =  await _context.BankAccounts.Where(x => x.userId == userId).ToListAsync();

            foreach( var account in accounts)
            {
                var bank = await _context.Banks.Where(bank => bank.Id == account.BankId).FirstOrDefaultAsync();

                account.Bank = bank;
            }

            return accounts;
        }

        public async Task<BankAccount> GetBankAccountById(int id)
        {
            return await _context.BankAccounts.FindAsync(id);
        }
    }
}
