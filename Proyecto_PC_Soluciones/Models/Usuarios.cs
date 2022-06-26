using System.ComponentModel.DataAnnotations;
namespace Proyecto_PC_Soluciones.Models
{
    public class Usuarios
    {
        [Display(Name = "ID")] public int  id_usuario { get; set; }
        [Required][Display(Name = "Nombre")] public string nom_usuario { get; set; }
        [Required][Display(Name = "DNI")] public string dni_usuario { get; set; }
        [Required][Display(Name = "Email")] public string user_usuario { get; set; }
        [Required][Display(Name = "Clave")] public string pass_usuario { get; set; }
        [Display(Name = "Tipo")] public int id_tipo_usuario { get; set; }
    }
}
