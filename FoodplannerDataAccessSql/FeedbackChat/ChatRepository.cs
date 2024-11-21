//using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using FoodplannerDataAccessSql;

public class ChatRepository(PostgreSQLConnectionFactory connectionFactory) : IChatRepository
{
    

    // Methods for ChatThread
    public async Task<ChatThread> GetChatThreadByIdAsync(int ChatThreadId)
    {
        const string sql = "SELECT * FROM chat_thread WHERE chat_thread_id = @ChatThreadId";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<ChatThread>(sql, new { ChatThreadId });
            return result;
        }
    }

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
    public async Task<Message> GetMessageByIdAsync(int MessageId)
    {
      const string sql = "SELECT * FROM message WHERE message_id = @MessageId";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<Message>(sql, new { MessageId });
            return result;
        }
    }

    public async Task<IEnumerable<Message>> GetMessagesByChatThreadIdAsync(int chatThreadId)
    {
        const string sql = "SELECT * FROM message WHERE chat_thread_id = @chatThreadId";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QueryAsync<Message>(sql, new{ chatThreadId });
            return result.ToList();
        }
    }

    public async Task AddMessageAsync(Message message)
    {
        const string sql = "INSERT INTO message (content, date, chat_thread_id, user_id) VALUES (@Content, @Date, @ChatThreadId, @UserId)";
        await using (var connection = connectionFactory.Create())
        {
            connection.Open();
            await connection.ExecuteAsync(sql, message);
        }
    }

    public async Task UpdateMessageAsync(Message message)
    {
       const string sql = "UPDATE Message SET Content = @Content WHERE MessageId = @MessageId";
       await using (var connection = connectionFactory.Create())
        {
            connection.Open();
            await connection.ExecuteAsync(sql, message);
        }
    }

    public async Task ArchiveMessageAsync(int MessageId)
    {
        const string sql = "UPDATE Message SET Archived = 1 WHERE MessageId = @MessageId";
        await using (var connection = connectionFactory.Create())
        {
            connection.Open();
            await connection.ExecuteAsync(sql, MessageId);
        }
    }
}