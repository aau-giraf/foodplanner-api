using FoodplannerApi.Helpers;
using FoodplannerModels.Account;
using FoodplannerModels.Auth;
using FoodplannerModels.FeedbackChat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;




namespace FoodplannerApi.Controller;


public class FeedbackChatController : BaseController
{
    private readonly IChatService _chatService;
    private readonly IAuthService _authService;
    
    public FeedbackChatController(IChatService chatService, AuthService authService)
    {
        _chatService = chatService;
        _authService = authService;
    }
    
    
    [HttpPost]
    //[Authorize(Roles = "Parent, Teacher")]
    public async Task<IActionResult> AddMessage([FromBody] AddMessageDTO messageDto, [FromHeader(Name = "Authorization")] string token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(messageDto.Content) || messageDto.Content.Length > 1000)
            {
                return BadRequest(new { Message = "Message content must be between 1 and 1000 characters." });
            }

            var idString = _authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int userId))
            {
                return BadRequest(new { Message = "Id er ikke et tal" });
            }

            var result = await _chatService.AddMessageAsync(messageDto, userId);
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



    
    [HttpGet("{chatThreadId}")]
    //[Authorize(Roles = "Parent, Teacher")]
    public async Task<IActionResult> GetMessages(int chatThreadId)
    {
        try
        {
            var messages = await _chatService.GetMessagesAsync(chatThreadId);
            return Ok(messages);
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }
    
    [HttpGet("{childId}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetChatThreadIdAndUserIdFromChildIdAndToken(int childId, [FromHeader(Name = "Authorization")] string token)
    {
        try
        {
            var idString = _authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int id)) {
                return BadRequest(new ErrorResponse {Message = ["Id er ikke et tal"]});
            }
            var chatThreadId = await _chatService.GetChatThreadIdByChildIdAsync(childId);
            var response = new {
                UserId = id,
                ChatThreadId = chatThreadId
            };
            return Ok(response);
        }
        catch (Exception )
        {
            return BadRequest();
        }
    }
    
    [HttpGet]
    [Authorize(Roles = "Parent")]
    public async Task<IActionResult> GetChatThreadIdAndUserIdFromToken([FromHeader(Name = "Authorization")] string token) {
        try {    
            var idString = _authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int id)) {
                return BadRequest(new ErrorResponse {Message = ["Id er ikke et tal"]});
            }
            var chatThreadId = await _chatService.GetChatThreadIdByUserIdAsync(id);
            
            var response = new {
                UserId = id,
                ChatThreadId = chatThreadId
            };
            return Ok(response);
        }
        catch (Exception )
        {
            return BadRequest();
        }
    }
    
    
    [HttpDelete("{messageId}")]
    [Authorize(Roles = "Parent, Teacher")]
    public async Task<IActionResult> ArchiveMessage(int messageId)
    {
        try
        {
            var result = await _chatService.ArchiveMessageAsync(messageId);
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
    
    [HttpPut]
    [Authorize(Roles = "Parent, Teacher")]
    public async Task<IActionResult> UpdateMessage([FromBody] UpdateMessageDTO message)
    {
        try
        {
            var result = await _chatService.UpdateMessageAsync(message);
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