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
    
    [HttpGet]
    public async Task<IActionResult> GetAllChatThreads()
    {
        var chatThreads = await _chatService.GetAllChatThreadsAsync();
        return Ok(chatThreads);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddMessage([FromBody] AddMessageDTO message )
    {
        try {
        var result = await _chatService.AddMessageToThread(message);
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
}