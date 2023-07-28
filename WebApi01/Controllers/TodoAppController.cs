using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

namespace WebApi01.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TodoAppController : ControllerBase
{
    private readonly string _sqlDataSource;
    private SqlDataReader? _reader;
    private readonly DataTable? _table;
    private readonly SqlConnection? _sqlConnection;
    public TodoAppController(IConfiguration config) 
    {
        _sqlDataSource = config.GetConnectionString("todoAppDBConnectionString");
        _sqlConnection = GetConnection(_sqlDataSource);
        _table = new DataTable();
    }

    private static SqlDataReader GetReader(SqlCommand command, IDbConnection? connection)
    {
        connection?.Open();
        return command.ExecuteReader(CommandBehavior.CloseConnection);
    }

    private static SqlConnection GetConnection(string source)
    {
        SqlConnection connection = new(source);
        return connection;
    }


    private static SqlCommand GetCommand(string query, SqlConnection? connection)
    {
        return new SqlCommand(query, connection);
    }


    [HttpGet]
    [Route("GetNotes")]
    public JsonResult GetNotes() 
    {
        const string query = "SELECT * FROM dbo.notes";

        using (_sqlConnection) 
        {
            try
            {
                using var myCommand = GetCommand(query, _sqlConnection);
                if (_sqlConnection != null) _reader = GetReader(myCommand, _sqlConnection);
                if (_reader != null) _table?.Load(_reader);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                _sqlConnection?.Close();
            }
        }

        using (_table)
        {
            return new JsonResult(_table);
        }
    }

        
        

    [HttpGet]
    [Route("GetNote")]
    public JsonResult GetNote(int id) 
    {
        const string query = "SELECT * FROM dbo.notes where NoteId=@id";
            
        using (_sqlConnection) 
        {
            try
            {
                using var myCommand = GetCommand(query, _sqlConnection);
                myCommand.Parameters.AddWithValue("@id", id);
                    
                if (_sqlConnection != null) _reader = GetReader(myCommand, _sqlConnection);
                if (_reader != null) _table?.Load(_reader);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                _sqlConnection?.Close();
            }
        }

        using (_table)
        {
            return new JsonResult(_table);
        }
    }
        
        

    [HttpPost]
    [Route("AddNotes")]
    public JsonResult AddNotes([FromForm] string title, [FromForm] string description) 
    {
        const string query = "INSERT into dbo.notes (Title, Description) values(@title, @description)";

        using (_sqlConnection) 
        {
            try
            {
                using var myCommand = GetCommand(query, _sqlConnection);
                myCommand.Parameters.AddWithValue("@title", title);
                myCommand.Parameters.AddWithValue("@description", description);
                    
                if (_sqlConnection != null) _reader = GetReader(myCommand, _sqlConnection);
                if (_reader != null) _table?.Load(_reader);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                _sqlConnection?.Close();
            }
        }

        using (_table)
        {
            return new JsonResult(_table);
        }
    }
        
        

    [HttpDelete]
    [Route("DeleteNotes")]
    public JsonResult DeleteNotes(int id)
    {
        const string query = "DELETE from dbo.notes where NoteId=@id";

        using (_sqlConnection) 
        {
            try
            {
                using var myCommand = GetCommand(query, _sqlConnection);
                myCommand.Parameters.AddWithValue("@id", id);
                    
                if (_sqlConnection != null) _reader = GetReader(myCommand, _sqlConnection);
                if (_reader != null) _table?.Load(_reader);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                _sqlConnection?.Close();
            }
        }

        using (_table)
        {
            return new JsonResult(_table);
        }
    }
        
        
        
    [HttpDelete]
    [Route("DeleteAllNotes")]
    public JsonResult DeleteAllNotes()
    {
        const string query = "DELETE from dbo.notes";

        using var myCon = _sqlConnection;
        try
        {
            myCon?.Open();
            using var myCommand = GetCommand(query, myCon);
            myCommand.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
            
        return new JsonResult("All notes have been deleted successfully.");
    }
        
        

    [HttpPut]
    [Route("UpdateNote")]
    public JsonResult UpdateNote([FromForm] int id, [FromForm] string title, [FromForm] string description)
    {
        const string query = "UPDATE dbo.notes SET Title = @title, Description = @description WHERE NoteId = @id";

        using var myCon = GetConnection(_sqlDataSource);
        using var command = GetCommand(query, myCon);
        try
        {
            myCon.Open();
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@title", title);
            command.Parameters.AddWithValue("@description", description);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
            
        var rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0 ? new JsonResult("Updated " + title) 
            : new JsonResult("Note not found or no changes made.");
    }
}