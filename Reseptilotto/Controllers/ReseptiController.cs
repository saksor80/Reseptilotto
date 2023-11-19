using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Reseptilotto.Models;
using System.Linq;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Xml;
using System;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace Reseptilotto.Controllers
{
    [Route("api/[controller]")]
    public class ReseptiController : Controller
    {

        [HttpGet]
        public IEnumerable<Resepti> GetAll()
        {
            return _context.InputItems.ToList();
        }

        [HttpGet("{id}", Name = "GetInput")]
        public IActionResult GetById(long id)
        {

            int i = 0;
            int Update = 0;
            string Content, Resepti = "";
            string[,] Reseptit = new string[30, 3];
            JObject json = new JObject { };

                SqlConnectionStringBuilder sqlbuilder = new SqlConnectionStringBuilder
                {
                    DataSource = "mysqlserver2300823.database.windows.net",
                    UserID = "avirrankoski",
                    Password = "Morowindi100",
                    InitialCatalog = "Ruokareseptit"
                };

                using (SqlConnection connection = new SqlConnection(sqlbuilder.ConnectionString))
                {
                    String sql = "SELECT * FROM reseptit";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                        while (reader.Read())
                        {
                            Content = reader["content"].ToString();

                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(Content);
                            Content = JsonConvert.SerializeXmlNode(doc);

                            DateTime Date1 = DateTime.Now;

                            if (reader["last_used_date"].ToString() != "")
                            {
                                DateTime DateDB = DateTime.Parse(reader["last_used_date"].ToString());
                                DateDB = DateDB.Add(TimeSpan.FromDays(2));
                                Debug.WriteLine(DateDB);

                                if (Date1 >= DateDB)
                                {
                                    Reseptit[i, 0] = reader["id"].ToString();
                                    Reseptit[i, 1] = reader["last_used_date"].ToString();
                                    Reseptit[i, 2] = Content;
                                    i++;
                                }
                            }

                            if (reader["last_used_date"].ToString() == "")
                            {
                                Reseptit[i, 0] = reader["id"].ToString();
                                Reseptit[i, 1] = reader["last_used_date"].ToString();
                                Reseptit[i, 2] = Content;
                                i++;
                            }
                        }
                        Debug.WriteLine(i);
                        Random rnd = new Random();
                            int index = rnd.Next(1, (Reseptit.Length/3));
                            Debug.WriteLine(index);

                            for (int j = 0; j < (Reseptit.Length / 3); j++)
                                if (Convert.ToInt32(Reseptit[j, 0]) == index)
                                {
                                    Resepti = Reseptit[j,2];
                                    Update = Convert.ToInt32(Reseptit[j,0]);
                                    Debug.WriteLine(Resepti);
                                    json = JObject.Parse(Resepti);
                                    break;
                                }
                            
                        for (int k = 0; k < (Reseptit.Length/3); k++)
                                Debug.Write(k + " " + Reseptit[k,0] + " " + Reseptit[k,1] + " " + Reseptit[k,2] + "\n\n");
                        }
                    }
                    /*
                    //Database column last_used_date Update

                    DateTime Date2 = DateTime.Now;

                    string query = "UPDATE reseptit SET last_used_date = @Date2 WHERE id = " + Update;

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Date2", Date2);

                        //con.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }
                    */
                }
            
            var item = _context.InputItems.FirstOrDefault(t => t.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return new ObjectResult(json);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Resepti item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            _context.InputItems.Add(item);
            _context.SaveChanges();

            return CreatedAtRoute("GetInput", new { id = item.Id }, item);
        }

        private readonly ReseptiContext _context;

        public ReseptiController(ReseptiContext context)
        {
            _context = context;

        }
    }
}
