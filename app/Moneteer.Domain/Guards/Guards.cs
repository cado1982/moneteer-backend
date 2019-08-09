namespace Moneteer.Domain.Guards
{
    public class Guards
    {
        public BudgetGuard BudgetGuard { get; }
        public AccountGuard AccountGuard { get; }
        public PayeeGuard PayeeGuard { get; }
        public TransactionGuard TransactionGuard { get; }
        public EnvelopeGuard EnvelopeGuard { get; }
        public EnvelopeCategoryGuard EnvelopeCategoryGuard { get; }

        public Guards(BudgetGuard budgetGuard,
                      AccountGuard accountGuard,
                      PayeeGuard payeeGuard,
                      TransactionGuard transactionGuard,
                      EnvelopeGuard envelopeGuard,
                      EnvelopeCategoryGuard envelopeCategoryGuard)
        {
            BudgetGuard = budgetGuard;
            AccountGuard = accountGuard;
            PayeeGuard = payeeGuard;
            TransactionGuard = transactionGuard;
            EnvelopeGuard = envelopeGuard;
            EnvelopeCategoryGuard = envelopeCategoryGuard;
        }
    }
}
