﻿using FoodplannerModels.FeedbackChat;
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
        var result = await _chatService.AddMessageAsync(message);
        if (result)
            {
                return Created(string.Empty, result);
            }
            return BadRequest();
        }
        catch (Exception e){
            return BadRequest(e.StackTrace);
        }
    }
    
    [HttpGet("{childId}")]
    public async Task<IActionResult> GetMessages(int childId)
    {
        
              
            var messages = await _chatService.GetMessagesAsync(childId);
            return Ok(messages);
    }
    
    [HttpDelete("{messageId}")]
    public async Task<IActionResult> ArchiveMessage(int messageId)
    {
        var result = await _chatService.ArchiveMessageAsync(messageId);
        if (result)
        {
            return Ok();
        }
        return BadRequest();
    }
    
}