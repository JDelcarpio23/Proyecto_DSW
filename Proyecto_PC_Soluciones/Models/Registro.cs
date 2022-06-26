using System.ComponentModel.DataAnnotations;
namespace Proyecto_PC_Soluciones.Models
{
    public class Registro
    {
        [Display(Name = "ID de Artículo")]public int id_articulo { get; set; }
        [Display(Name = "Descripción")] public string nom_articulo { get; set; }
        [Display(Name = "Precio")] public decimal pre_articulo { get; set; }
        [Display(Name = "Cantidad")] public int cantidad { get; set; }
        [Display(Name = "Monto")] public decimal monto { get { return pre_articulo * cantidad; } }
    }
}
