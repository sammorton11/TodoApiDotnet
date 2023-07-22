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
        [HttpPost]
        [Route("AddNotes")]
        public JsonResult AddNotes([FromForm] string newNotes) 
        {
            string query = "INSERT into dbo.notes values(@newNotes)";
            DataTable table = new();
            string sqlDatasource = _config.GetConnectionString("todoAppDBConnectionString");
            SqlDataReader myReader;

            using (SqlConnection myCon = new(sqlDatasource)) 
            {
                myCon.Open();

                using SqlCommand myCommand = new(query, myCon);
                myCommand.Parameters.AddWithValue("@newNotes", newNotes);
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
            string query = "DELETE from dbo.notes where id=@id";
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
    }
}
