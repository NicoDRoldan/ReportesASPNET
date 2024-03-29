﻿using System.Data;

namespace ReportesASPNET.Models
{
    public class UsuariosModels
    {
        public int Id_Usuario { get; set; }
        public string Usuario { get; set; }
        public int Id_Categoria { get; set; }
        public string Password { get; set; }
        public bool Estado { get; set; }
        public DateTime? Fecha { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public CategoriaModels Categoria { get; set; }
    }
}