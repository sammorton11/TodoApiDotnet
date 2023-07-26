using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

namespace WebApi01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoAppController : ControllerBase
    {
        private IConfiguration _config;
        public TodoAppController(IConfiguration config) 
        {
            _config = config;
        }

        [HttpGet]
        [Route("GetNotes")]
        public JsonResult GetNotes() 
        {
            string query = "SELECT * FROM dbo.notes";
            DataTable table = new DataTable();
            string sqlDatasource = _config.GetConnectionString("todoAppDBConnectionString");
            SqlDataReader myReader;

            using (SqlConnection myCon = new(sqlDatasource)) 
            {
                myCon.Open();

                using SqlCommand myCommand = new(query, myCon);
                myReader = myCommand.ExecuteReader();
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
            string query = "SELECT * FROM dbo.notes where NoteId=@id";
            DataTable table = new DataTable();
            string sqlDatasource = _config.GetConnectionString("todoAppDBConnectionString");
            SqlDataReader myReader;

            using (SqlConnection myCon = new(sqlDatasource)) 
            {
                myCon.Open();
                using (SqlCommand myCommand = new(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@id", id);
                    myReader = myCommand.ExecuteReader();
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
            string query = "INSERT into dbo.notes (Title, Description) values(@title, @description)";
            DataTable table = new();
            string sqlDatasource = _config.GetConnectionString("todoAppDBConnectionString");
            SqlDataReader myReader;

            using (SqlConnection myCon = new(sqlDatasource)) 
            {
                myCon.Open();

                using SqlCommand myCommand = new(query, myCon);
                myCommand.Parameters.AddWithValue("@title", title);
                myCommand.Parameters.AddWithValue("@description", description);
                myReader = myCommand.ExecuteReader();
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
            string query = "DELETE from dbo.notes where NoteId=@id";
            DataTable table = new();
            string sqlDatasource = _config.GetConnectionString("todoAppDBConnectionString");
            SqlDataReader myReader;

            using (SqlConnection myCon = new(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@id", id);
                    myReader = myCommand.ExecuteReader();
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
            string query = "DELETE from dbo.notes";
            string sqlDatasource = _config.GetConnectionString("todoAppDBConnectionString");

            using (SqlConnection myCon = new(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new(query, myCon))
                {
                    myCommand.ExecuteNonQuery(); // Use ExecuteNonQuery for DELETE
                }
            }

            return new JsonResult("Deleted Successfully");
        }

        // [HttpPut]
        // [Route("UpdateNote")]
        // public JsonResult UpdateNote([FromForm] int id, [FromForm] string title, [FromForm] string description)
        // {
        //     string query = "UPDATE dbo.notes SET Title = @title, Description = @description WHERE NoteId = @id";
        //     string sqlData = _config.GetConnectionString("todoAppDBConnectionString");
        //     DataTable table = new();
        //     SqlDataReader myReader;
        //     using (SqlConnection myCon = new(sqlData)) 
        //     {
        //         myCon.Open();
        //         using (SqlCommand command = new(query, myCon))
        //         {
        //             command.Parameters.AddWithValue("@id", id);
        //             command.Parameters.AddWithValue("@title", title);
        //             command.Parameters.AddWithValue("@description", description);
        //             myReader = command.ExecuteReader();
        //             table.Load(myReader);
        //
        //             myReader.Close();
        //             myCon.Close();
        //         }
        //     }
        //
        //     return new JsonResult("Updated " + title);
        // }
        
        [HttpPut]
        [Route("UpdateNote")]
        public JsonResult UpdateNote([FromForm] int id, [FromForm] string title, [FromForm] string description)
        {
            string query = "UPDATE dbo.notes SET Title = @title, Description = @description WHERE NoteId = @id";
            string sqlData = _config.GetConnectionString("todoAppDBConnectionString");

            using (SqlConnection myCon = new SqlConnection(sqlData))
            {
                myCon.Open();
                using (SqlCommand command = new SqlCommand(query, myCon))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@title", title);
                    command.Parameters.AddWithValue("@description", description);
                    int rowsAffected = command.ExecuteNonQuery(); // Use ExecuteNonQuery for UPDATE

                    if (rowsAffected > 0)
                    {
                        return new JsonResult("Updated " + title);
                    }
                    else
                    {
                        return new JsonResult("Note not found or no changes made.");
                    }
                }
            }
        }

    }
}
