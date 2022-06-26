using System.ComponentModel.DataAnnotations;
namespace Proyecto_PC_Soluciones.Models
{
    public class Pedido
    {
        [Display(Name = "ID Venta")] public string id_venta { get; set; }
        [Display(Name = "Fecha Venta")] public DateTime fecha_venta { get; set; }
        [Display(Name = "DNI Usuario")] public string dni_usuario { get; set; }
        [Display(Name = "Nombre Usuario")] public string nom_usuario { get; set; }

        [Display(Name = "Monto")] public decimal monto { get; set; }


    }
}
