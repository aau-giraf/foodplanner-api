using FoodplannerApi.Helpers;
using FoodplannerModels.Account;
using FoodplannerModels.FeedbackChat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace FoodplannerApi.Controller;

public class FeedbackChatController : BaseController
{
    private readonly IChatService _chatService;
    private readonly AuthService _authService;
    
    public FeedbackChatController(IChatService chatService, AuthService authService)
    {
        _chatService = chatService;
        _authService = authService;
    }
    
    [HttpPost]
    [Authorize(Roles = "Parent, Teacher")]
    public async Task<IActionResult> AddMessage([FromBody] AddMessageDTO message )
    {
        try {
        var result = await _chatService.AddMessageAsync(message);
        if (result)
            {
                return Created(string.Empty, result);
            }
            return BadRequest();
        }
        catch (Exception){
            return BadRequest();
        }
    }
    
    [HttpGet("{chatThreadId}")]
    [Authorize(Roles = "Parent, Teacher")]
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
    public async Task<IActionResult> GetChatThreadIdByChildId(int childId)
    {
        try
        {
            var chatThreadId = await _chatService.GetChatThreadIdByChildIdAsync(childId);
            return Ok(chatThreadId);
        }
        catch (Exception )
        {
            return BadRequest();
        }
    }
    
    [HttpGet]
    [Authorize(Roles = "Parent")]
    public async Task<IActionResult> GetChatThreadIdByUserId([FromHeader(Name = "Authorization")] string token) {
        try {    
            var idString = _authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int id)) {
                return BadRequest(new ErrorResponse {Message = ["Id er ikke et tal"]});
            }
            var chatThreadId = await _chatService.GetChatThreadIdByUserIdAsync(id);
            return Ok(chatThreadId);
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