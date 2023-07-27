using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

namespace WebApi01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoAppController : ControllerBase
    {
        private readonly string _sqlDataSource;
        public TodoAppController(IConfiguration config) 
        {
            _sqlDataSource = config.GetConnectionString("todoAppDBConnectionString");
        }
        


        [HttpGet]
        [Route("GetNotes")]
        public JsonResult GetNotes() 
        {
            const string query = "SELECT * FROM dbo.notes";
            var table = new DataTable();

            using (SqlConnection myCon = new(_sqlDataSource)) 
            {
                myCon.Open();
                using SqlCommand myCommand = new(query, myCon);
                var myReader = myCommand.ExecuteReader();
                table.Load(myReader);
        
                myReader.Close();
                myCon.Close();
            }
        
            return new JsonResult(table);
        }

        
        

        [HttpGet]
        [Route("GetNote")]
        public JsonResult GetNote(int id) 
        {
            const string query = "SELECT * FROM dbo.notes where NoteId=@id";
            var table = new DataTable();

            using (SqlConnection myCon = new(_sqlDataSource)) 
            {
                myCon.Open();
                using (SqlCommand myCommand = new(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@id", id);
                    var myReader = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
                    table.Load(myReader);
                    
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }
        
        

        [HttpPost]
        [Route("AddNotes")]
        public JsonResult AddNotes([FromForm] string title, [FromForm] string description) 
        {
            const string query = "INSERT into dbo.notes (Title, Description) values(@title, @description)";
            DataTable table = new();

            using (SqlConnection myCon = new(_sqlDataSource)) 
            {
                myCon.Open();

                using SqlCommand myCommand = new(query, myCon);
                myCommand.Parameters.AddWithValue("@title", title);
                myCommand.Parameters.AddWithValue("@description", description);
                var myReader = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
                table.Load(myReader);

                myReader.Close();
                myCon.Close();
            }

            return new JsonResult("Added Successfully");
        }
        
        

        [HttpDelete]
        [Route("DeleteNotes")]
        public JsonResult DeleteNotes(int id)
        {
            const string query = "DELETE from dbo.notes where NoteId=@id";
            DataTable table = new();

            using (SqlConnection myCon = new(_sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@id", id);
                    var myReader = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Deleted Successfully");
        }
        
        
        
        [HttpDelete]
        [Route("DeleteAllNotes")]
        public JsonResult DeleteAllNotes()
        {
            const string query = "DELETE from dbo.notes";

            using (SqlConnection myCon = new(_sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new(query, myCon))
                {
                    myCommand.ExecuteNonQuery(); // Use ExecuteNonQuery for DELETE
                }
            }

            return new JsonResult("Deleted Successfully");
        }
        
        

        [HttpPut]
        [Route("UpdateNote")]
        public JsonResult UpdateNote([FromForm] int id, [FromForm] string title, [FromForm] string description)
        {
            const string query = "UPDATE dbo.notes SET Title = @title, Description = @description WHERE NoteId = @id";
            var sqlDataSource = this._sqlDataSource;

            using var myCon = new SqlConnection(sqlDataSource);
            myCon.Open();
            
            using var command = new SqlCommand(query, myCon);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@title", title);
            command.Parameters.AddWithValue("@description", description);
            
            var rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0 ? new JsonResult("Updated " + title) 
                : new JsonResult("Note not found or no changes made.");
        }
    }
}
