using FoodplannerDataAccessSql;
using Dapper;
using FoodplannerModels.FeedbackChat;

// Handles database operations related to chat threads and messages
public class ChatRepository(CorePostgreSQLConnectionFactory connectionFactory) : IChatRepository
{
    // Retrieves a chat thread by its id, throws an exception if no matching chat thread is found
    public async Task<ChatThread> GetThreadByIdAsync(int ChatThreadId)
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

    // Retrieves the ID of the chat thread belonging to a child
    public async Task<int?> GetThreadIdByChildIdAsync(int childId)
    {
        const string sql = "SELECT chat_thread_id FROM chat_thread WHERE child_id = @ChildId";
        using var connection = connectionFactory.Create();
        connection.Open();
        return await connection.QueryFirstOrDefaultAsync<int?>(sql, new { ChildId = childId });
    }
    
    // Creates a chat thread for a child and returns the newly created chat thread ID
    public async Task<int> CreateThreadByChildIdAsync(int ChildId)
    {
        const string sql = "INSERT INTO chat_thread (child_id) VALUES (@ChildId) RETURNING chat_thread_id";
        await using (var connection = connectionFactory.Create())
        {
            connection.Open();
            var chatThreadId = await connection.ExecuteScalarAsync<int>(sql, new { ChildId });
            return chatThreadId;
        }
    }

    // Retrieves a message by its id, Throws an exception if no matching message is found
    public async Task<Message?> GetMessageByIdAsync(int MessageId)
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

    // Retrieves all messages from a chat thread   
    public async Task<IEnumerable<Message>> GetMessagesByThreadIdAsync(int chatThreadId)
    {
        const string sql = "SELECT message.*, users.first_name FROM message JOIN users ON message.user_id = users.id WHERE message.chat_thread_id = @chatThreadId ORDER BY message.date ASC";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QueryAsync<Message>(sql, new{ chatThreadId });
            return result;
        }
    }

    // Insert a new message to the database
    public async Task<int> AddMessageAsync(AddMessageTemp message)
    {
        const string sql = "INSERT INTO message (content, date, chat_thread_id, user_id) VALUES (@Content, @Date, @ChatThreadId, @UserId) RETURNING message_id";
        await using (var connection = connectionFactory.Create())
        {
            connection.Open();
            var messageId = await connection.ExecuteScalarAsync<int>(sql, message);
            return messageId;
        }
    }

    // Update the content of a message and mark it as edited
    public async Task<int> EditMessageAsync(EditMessageDTO message)
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
    public async Task<IEnumerable<Message>> GetAllMessagesAsync()
    {
        var sql = "SELECT * FROM message";
        using (var connection = connectionFactory.Create())
        {
            var messages = await connection.QueryAsync<Message>(sql);
            return messages;

        }
    }

    // Permanently deletes a message by its ID
    public async Task<int> DeleteMessageAsync(int id)
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