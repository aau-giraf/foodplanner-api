//using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using FoodplannerDataAccessSql;
using FoodplannerModels;
using FoodplannerModels.FeedbackChat;

// Handles database operations related to chat threads and messages
public class ChatRepository(PostgreSQLConnectionFactory connectionFactory) : IChatRepository
{
    

    // Methods for ChatThread

    /* Retrives a chat thread by its ID
    Throws an exception if no matching chat thread is found */
    public async Task<ChatThread> GetChatThreadByIdAsync(int ChatThreadId)
    {
        const string sql = "SELECT * FROM chat_thread WHERE chat_thread_id = @ChatThreadId";
        await using var connection = connectionFactory.Create();
        connection.Open();
        var result = await connection.QuerySingleOrDefaultAsync<ChatThread>(sql, new { ChatThreadId });
        connection.Close();
        

        if(result != null)
        {
            return result;
        }
        else
        {
            throw new Exception($"ChatThread with ID {ChatThreadId} not found.");
        }
    }

    // Retrives the ID of the chat thread belonging to a child
    public async Task<int> GetChatThreadIdByChildIdAsync(int ChildId)
    {
        const string sql = "SELECT chat_thread_id FROM chat_thread WHERE child_id = @ChildId";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QueryFirstOrDefaultAsync<int>(sql, new { ChildId });
            return result;
        }
    }
    
    // Creates a chat thread for a child and returns the newly created chat thread ID
    public async Task<int> AddChatThreadIdByChildIdAsync(int ChildId)
    {
        const string sql = "INSERT INTO chat_thread (child_id) VALUES (@ChildId) RETURNING chat_thread_id";
        
    
        await using (var connection = connectionFactory.Create())
        {
            connection.Open();
            var chatThreadId = await connection.ExecuteScalarAsync<int>(sql, new { ChildId });
            return chatThreadId;
        }
    }

    
    // Methods for Message

    /* Retrives a message by its ID
    Throws an exception if no matching message is found */
    public async Task<Message> GetByIdAsync(int MessageId)
    {
      const string sql = "SELECT * FROM message WHERE message_id = @MessageId";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<Message>(sql, new { MessageId });
            connection.Close();
            

            if(result != null)
            {
                return result;
            }
            else
            {
                throw new Exception($"Message with ID {MessageId} not found.");
            }
        }
    }

    // Message in IEnum param has to be DTO then we can JOIN the 2 queries, so we can select the name based on the userId
    // public async Task<IEnumerable<Message>> GetMessagesByChatThreadIdAsync(int chatThreadId)
    // {
    //     const string sql = "SELECT * FROM message WHERE chat_thread_id = @chatThreadId";
    //     using (var connection = connectionFactory.Create())
    //     {
    //         connection.Open();
    //         var result = await connection.QueryAsync<Message>(sql, new{ chatThreadId });
    //         return result.ToList();
    //     }
    // }

    /* Retrieves all messages from a chat thread together with the senders first name
    Messages are ordered from oldest to newest */    
    public async Task<IEnumerable<Message>> GetMessagesByChatThreadIdAsync(int chatThreadId)
    {
        const string sql = @"
                                SELECT message.*, users.first_name
                                FROM message 
                                JOIN users ON message.user_id = users.id
                                WHERE message.chat_thread_id = @chatThreadId
                                ORDER BY message.date ASC";

        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QueryAsync<Message>(sql, new{ chatThreadId });
            return result;
        }
    }
    // Insert a new message to the database
    public async Task<int> InsertAsync(Message message)
    {
        const string sql = "INSERT INTO message (content, date, chat_thread_id, user_id) VALUES (@Content, @Date, @ChatThreadId, @UserId)";
        await using (var connection = connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, message);
            return result;
        }
    }

    // Update the content of a message and mark it as edited
    public async Task<int> UpdateAsync(Message message)
    {
        const string sql = "UPDATE message SET content = @Content, is_edited = true WHERE message_id = @MessageId";
        await using (var connection = connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, message);
            return result;
        }
    }

    // Archive a message instead of deleting it from the database
    public async Task ArchiveMessageAsync(int MessageId)
    {
        const string sql = "UPDATE message SET archived = true WHERE message_id = @MessageId";
        await using (var connection = connectionFactory.Create())
        {
            connection.Open();
            await connection.ExecuteAsync(sql, new {MessageId});
        }
    }

    // Retrieves all messages from the database
    public async Task<IEnumerable<Message>> GetAllAsync()
    {
        var sql = "SELECT * FROM message";
        using (var connection = connectionFactory.Create())
        {
            var messages = await connection.QueryAsync<Message>(sql);
            return messages;

        }
    }

    // Permanently deletes a message by its ID
    public async Task<int> DeleteAsync(int id)
    {
        var sql = "DELETE FROM message WHERE message_id = @MessageId";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, new { MessageId = id });
            return result;
        }
    }
}