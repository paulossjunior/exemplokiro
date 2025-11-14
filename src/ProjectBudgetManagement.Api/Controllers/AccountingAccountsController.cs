using Microsoft.AspNetCore.Mvc;
using ProjectBudgetManagement.Api.Models;
using ProjectBudgetManagement.Application.Commands;
using ProjectBudgetManagement.Application.Queries;
using ProjectBudgetManagement.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace ProjectBudgetManagement.Api.Controllers;

/// <summary>
/// Manages accounting accounts for transaction categorization.
/// </summary>
/// <remarks>
/// Accounting accounts provide classification categories for transactions, enabling proper financial reporting.
/// Each account has a unique identifier following a defined format pattern.
/// 
/// **Performance**: All endpoints respond in &lt;100ms
/// </remarks>
[ApiController]
[Route("api/accounting-accounts")]
[Produces("application/json")]
public class AccountingAccountsController : ControllerBase
{
    private readonly CreateAccountingAccountCommandHandler _createHandler;
    private readonly ListAccountingAccountsQueryHandler _listHandler;

    /// <summary>
    /// Initializes a new instance of the AccountingAccountsController class.
    /// </summary>
    public AccountingAccountsController(
        CreateAccountingAccountCommandHandler createHandler,
        ListAccountingAccountsQueryHandler listHandler)
    {
        _createHandler = createHandler;
        _listHandler = listHandler;
    }

    /// <summary>
    /// Creates a new accounting account for transaction categorization
    /// </summary>
    /// <param name="request">The accounting account creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created accounting account</returns>
    /// <remarks>
    /// Creates a new accounting account with a unique identifier.
    /// 
    /// **Business Rules**:
    /// - Identifier must be unique across all accounting accounts
    /// - Identifier must follow the defined format pattern (e.g., "XXXX.XX.XXXX")
    /// - Name is required
    /// 
    /// **Protection**: Accounting accounts with associated transactions cannot be deleted.
    /// 
    /// **Performance**: Responds in &lt;100ms
    /// 
    /// Sample request:
    /// 
    ///     POST /api/accounting-accounts
    ///     {
    ///       "name": "Office Supplies",
    ///       "identifier": "4010.01.0001"
    ///     }
    /// 
    /// </remarks>
    /// <response code="201">Accounting account successfully created.</response>
    /// <response code="400">Invalid input data or duplicate identifier.</response>
    /// <response code="409">Accounting account with this identifier already exists.</response>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create accounting account",
        Description = "Creates a new accounting account for transaction categorization",
        OperationId = "CreateAccountingAccount",
        Tags = new[] { "Accounting Accounts" }
    )]
    [ProducesResponseType(typeof(AccountingAccountResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAccountingAccount(
        [FromBody] CreateAccountingAccountRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new CreateAccountingAccountCommand
            {
                Name = request.Name,
                Identifier = request.Identifier
            };

            var account = await _createHandler.HandleAsync(command, cancellationToken);
            var response = MapToResponse(account);

            return CreatedAtAction(nameof(GetAccountingAccount), new { id = account.Id }, response);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
        {
            return Conflict(new ErrorResponse 
            { 
                Error = new ErrorDetails 
                { 
                    Code = "DUPLICATE_IDENTIFIER",
                    Message = ex.Message 
                } 
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ErrorResponse 
            { 
                Error = new ErrorDetails 
                { 
                    Code = "VALIDATION_ERROR",
                    Message = ex.Message 
                } 
            });
        }
    }

    /// <summary>
    /// Retrieves all accounting accounts
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all accounting accounts</returns>
    /// <remarks>
    /// Returns the complete list of accounting accounts available for transaction categorization.
    /// 
    /// **Performance**: Responds in &lt;100ms
    /// 
    /// Sample request:
    /// 
    ///     GET /api/accounting-accounts
    /// 
    /// </remarks>
    /// <response code="200">Returns the list of accounting accounts. Empty array if none exist.</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "List accounting accounts",
        Description = "Retrieves all accounting accounts",
        OperationId = "ListAccountingAccounts",
        Tags = new[] { "Accounting Accounts" }
    )]
    [ProducesResponseType(typeof(List<AccountingAccountResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAccountingAccounts(CancellationToken cancellationToken)
    {
        var query = new ListAccountingAccountsQuery();
        var accounts = await _listHandler.HandleAsync(query, cancellationToken);
        var responses = accounts.Select(MapToResponse).ToList();

        return Ok(responses);
    }

    /// <summary>
    /// Retrieves an accounting account by its unique identifier
    /// </summary>
    /// <param name="id">The accounting account unique identifier</param>
    /// <returns>The accounting account details</returns>
    /// <remarks>
    /// **Performance**: Responds in &lt;100ms
    /// </remarks>
    /// <response code="200">Accounting account found and returned.</response>
    /// <response code="404">Accounting account not found or endpoint not implemented.</response>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get accounting account by ID",
        Description = "Retrieves a specific accounting account",
        OperationId = "GetAccountingAccount",
        Tags = new[] { "Accounting Accounts" }
    )]
    [ProducesResponseType(typeof(AccountingAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public IActionResult GetAccountingAccount(Guid id)
    {
        // This is a placeholder - implement if needed
        return NotFound(new { error = "Endpoint not yet implemented" });
    }

    private static AccountingAccountResponse MapToResponse(AccountingAccount account)
    {
        return new AccountingAccountResponse
        {
            Id = account.Id,
            Name = account.Name,
            Identifier = account.Identifier,
            CreatedAt = account.CreatedAt
        };
    }
}
