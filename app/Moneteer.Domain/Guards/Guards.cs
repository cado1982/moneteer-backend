namespace Moneteer.Domain.Guards
{
    public class Guards
    {
        public BudgetGuard BudgetGuard { get; }
        public AccountGuard AccountGuard { get; }
        public PayeeGuard PayeeGuard { get; }
        public TransactionGuard TransactionGuard { get; }

        public Guards(BudgetGuard budgetGuard,
                      AccountGuard accountGuard,
                      PayeeGuard payeeGuard,
                      TransactionGuard transactionGuard)
        {
            BudgetGuard = budgetGuard;
            AccountGuard = accountGuard;
            PayeeGuard = payeeGuard;
            TransactionGuard = transactionGuard;
        }
    }
}
