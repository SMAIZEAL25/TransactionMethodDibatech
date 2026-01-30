namespace AssignmentTransferMethod.Response
{
    public record CreateAccountResponse(
        bool Succeeded,
        string? ErrorCode = null,
        string? Message = null,
        Guid? AccountId = null
    );
}
