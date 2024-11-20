using Microsoft.EntityFrameworkCore;
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

    public async Task AddChatThreadAsync(ChatThread chatThread)
    {
        const string sql = "INSERT INTO ChatThread (ChatThreadId, ChatThreadName) VALUES (@ChatThreadId, @ChatThreadName)";
        await using (var connection = connectionFactory.Create())
        {
            connection.Open();
            await connection.ExecuteAsync(sql, chatThread);
        }
      
    }

    public async Task UpdateChatThreadAsync(ChatThread chatThread)
    {
      
    }

    public async Task DeleteChatThreadAsync(int id)
    {
       
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
        const string sql = "INSERT INTO Message (MessageId, Content, SentAt, SentByUserId, RecievedByUserId, ChatThreadId) VALUES (@MessageId, @Content, @SentAt, @SentByUserId, @RecievedByUserId, @ChatThreadId)";
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

    public async Task DeleteMessageAsync(int MessageId)
    {
        const string sql = "DELETE FROM Message WHERE MessageId = @MessageId";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            await connection.ExecuteAsync(sql, new { MessageId });
        }
    }
}