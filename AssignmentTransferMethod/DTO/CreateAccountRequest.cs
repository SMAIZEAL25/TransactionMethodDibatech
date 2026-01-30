namespace AssignmentTransferMethod.DTO
{
    public record CreateAccountRequest(
    decimal InitialBalance,
    string IdempotencyKey
);
}
