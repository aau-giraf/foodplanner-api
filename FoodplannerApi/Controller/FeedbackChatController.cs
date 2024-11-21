using FoodplannerModels.FeedbackChat;
using Microsoft.AspNetCore.Mvc;


namespace FoodplannerApi.Controller;

public class FeedbackChatController : BaseController
{
    private readonly IChatService _chatService;
    
    public FeedbackChatController(IChatService chatService)
    {
        _chatService = chatService;
    }
    
    [HttpPost]
    public async Task<IActionResult> AddMessage([FromBody] AddMessageDTO message )
    {
        try {
        var result = await _chatService.AddMessage(message);
        if (result)
            {
                return Created(string.Empty, result);
            }
            return BadRequest();
        }
        catch (Exception e){
            return BadRequest();
        }
    }
    
    [HttpGet("{childId}")]
    public async Task<IActionResult> GetMessages(int childId)
    {
        try
        {
            var messages = await _chatService.GetMessagesAsync(childId);
            return Ok(messages);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    
}