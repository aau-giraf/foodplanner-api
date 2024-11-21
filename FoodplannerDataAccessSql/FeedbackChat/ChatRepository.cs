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
        const string sql = "SELECT * FROM ChatThread WHERE Id = @ChatThreadId";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<ChatThread>(sql, new { ChatThreadId });
            return result;
        }
    }

    public async Task<IEnumerable<ChatThread>> GetAllChatThreadsAsync()
    {
        const string sql = "SELECT * FROM ChatThread";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QueryAsync<ChatThread>(sql);
            return result.ToList();
        }
    }

    // Methods for Message
    public async Task<Message> GetMessageByIdAsync(int MessageId)
    {
      const string sql = "SELECT * FROM Message WHERE Id = @MessageId";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<Message>(sql, new { MessageId });
            return result;
        }
    }

    public async Task<IEnumerable<Message>> GetMessagesByChatThreadIdAsync(int chatThreadId)
    {
        const string sql = "SELECT * FROM Message WHERE ChatThreadId = @chatThreadId";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QueryAsync<Message>(sql);
            return result.ToList();
        }
    }

    public async Task AddMessageAsync(Message message)
    {
        const string sql = "INSERT INTO Message (Content, SentAt, SentByUserId, ChatThreadId) VALUES (@Content, @SentAt, @SentByUserId, @RecievedByUserId, @ChatThreadId)";
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