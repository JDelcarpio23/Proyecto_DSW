using System.ComponentModel.DataAnnotations;
namespace Proyecto_PC_Soluciones.Models
{
    public class Articulo
    {
        [Display(Name = "ID de Artículo")] public int id_articulo { get; set; }
        [Display(Name = "Descripción")] public string nom_articulo { get; set; }
        [Display(Name = "Precio")] public decimal pre_articulo { get; set; }
        [Display(Name = "Stock")] public int stock_articulo { get; set; }
    }
}
