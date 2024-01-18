﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using ReportesASPNET.Models;

namespace ReportesASPNET.Controllers
{
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        
        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            List<UsuariosModels> Usuarios = new List<UsuariosModels>();
            List<CategoriaModels> Categorias = new List<CategoriaModels>();

            try
            {
                string query = "SELECT id_Usuario, Usuario, Password, c.Nombre [Categoria], Estado, Fecha, u.Nombre, Apellido FROM Usuarios u INNER JOIN Categorias c ON c.id_Categoria = u.id_Categoria ORDER BY id_Usuario;";

                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("Defaultconnection")))
                {
                    sqlConnection.Open();

                    using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                    {
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UsuariosModels usuario = new UsuariosModels();
                                CategoriaModels categoria = new CategoriaModels(_configuration);

                                usuario.Id_Usuario = reader.GetInt32(0);
                                usuario.Usuario = reader.GetString(1);
                                usuario.Password = reader.GetString(2);
                                usuario.Estado = reader.GetBoolean(4);
                                usuario.Fecha = reader.IsDBNull(5) ? null : reader.GetDateTime(5);
                                usuario.Nombre = reader.GetString(6);
                                usuario.Apellido = reader.GetString(7);

                                categoria.Nombre = reader.GetString(3);
                                usuario.Categoria = categoria;

                                Usuarios.Add(usuario);
                            }
                        }
                    }
                    sqlConnection.Close();
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return View(Usuarios);
        }
    }
}
