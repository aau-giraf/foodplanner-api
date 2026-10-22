using FoodplannerModels.Account;
using FoodplannerModels.Auth;
using FoodplannerModels.FeedbackChat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// Adds the FeedbackChatController to the FoodPlannerApi.controller namespace
namespace FoodplannerApi.Controller;

public class ChatController(IChatService chatService, IAuthService authService) : BaseController
{
    // URL: api/Chat/GetIdFromToken
    // Gets the users id from their JWT token
    [HttpGet]
    public async Task<string> GetIdFromToken([FromHeader(Name = "Authorization")] string token)
    {
        return authService.RetrieveIdFromJwtToken(token);
    }

    // URL: api/Chat/AddMessage
    // Adds message from AddMessageDTO and retrieves UserId from JWT token in Authorization header. Returns 201 Created if successful, or 400 Bad Request if unsuccessful.
    [Authorize(Roles = "Parent, Teacher")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddMessage([FromBody] AddMessageDTO messageDto, [FromHeader(Name = "Authorization")] string token)
    {
        try
        {
            // Retrieve UserId from the JWT token
            var idString = authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int userId))
            {
                return BadRequest(new { Message = "Id er ikke et tal" });
            }
         
            // Call the service layer to add the message
            var result = await chatService.AddMessageAsync(messageDto, userId);
            if (result)
            {
                return Created(string.Empty, result);
            }
            return BadRequest(new { Message = "Kunne ikke oprette besked" });
        }
        catch (Exception ex)
        {
           
            Console.WriteLine(ex.Message);
            return StatusCode(500, new { Message = "En serverfejl opstod" });
        }
    }

    // URL: api/Chat/GetMessages/{chatThreadId}
    // Retrieves messages for a given chatThreadId. Returns 200 OK with the list of messages if successful, or 400 Bad Request if unsuccessful.
    [Authorize(Roles = "Parent, Teacher")]
    [HttpGet("{chatThreadId}")]
    [ProducesResponseType(typeof(IEnumerable<UserNameFeedbackChatDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetMessages(int chatThreadId)
    {
        try
        {
            var messages = await chatService.GetMessagesAsync(chatThreadId);
            return Ok(messages);
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }
    
    // URL: api/Chat/GetThreadIdsForChild/{childId}
    // Retrieves chatThreadId and userId for a given childId and JWT token in Authorization header. Returns 200 OK with the data if successful, or 400 Bad Request if unsuccessful.
    [Authorize(Roles = "Teacher")]
    [HttpGet("{childId}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetThreadIdsForChild(int childId, [FromHeader(Name = "Authorization")] string token)
    {
        try
        {
            // Retrieve UserId from the JWT token            
            var idString = authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int id)) {
                return BadRequest(new ErrorResponse {Message = ["Id er ikke et tal"]});
            }

            // Call the service layer to get the chatThreadId by childId
            var chatThreadId = await chatService.GetThreadIdByChildIdAsync(childId);
            return Ok(chatThreadId);
        }
        catch (Exception )
        {
            return BadRequest();
        }
    }
    
    // URL: api/Chat/GetThreadIdsFromParentToken
    // Retrieves chatThreadId and userId from JWT token in Authorization header. Returns 200 OK with the data if successful, or 400 Bad Request if unsuccessful.
    [Authorize(Roles = "Parent")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetThreadIdsFromParentToken([FromHeader(Name = "Authorization")] string token) {
        try {
            // Retrieve UserId from the JWT token
            var idString = authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int id)) {
                return BadRequest(new ErrorResponse {Message = ["Id er ikke et tal"]});
            }

            // Call the service layer to get the chatThreadIds by userId
            IEnumerable<int> chatThreadIds = await chatService.GetThreadIdsByParentIdAsync(id);
            return Ok(chatThreadIds);
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }
    
    // URL: api/Chat/ArchiveMessage/{messageId}
    // Archives a message by its ID. Returns 200 OK if successful, or 400 Bad Request if unsuccessful.
    [Authorize(Roles = "Parent, Teacher")]
    [HttpDelete("{messageId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ArchiveMessage(int messageId)
    {
        try
        {
            var result = await chatService.ArchiveMessageAsync(messageId);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }
    
    // URL: api/Chat/EditMessage
    // Updates a message from UpdateMessageDTO. Returns 200 OK if successful, or 400 Bad Request if unsuccessful.
    [Authorize(Roles = "Parent, Teacher")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EditMessage([FromBody] EditMessageDTO message)
    {
        try
        {
            var result = await chatService.EditMessageAsync(message);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }
}