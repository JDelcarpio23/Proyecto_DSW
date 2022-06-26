using Microsoft.AspNetCore.Mvc;
using Microsoft.Data;
using Microsoft.Data.SqlClient;
using Proyecto_PC_Soluciones.Models;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Proyecto_PC_Soluciones.Controllers
{
    public class ConsultaController : Controller
    {
<<<<<<< HEAD
        string cadena = @"server=JDelcarpio-I7;database=ProyectoVisual;
=======
        string cadena = @"server=BRYAN;database=ProyectoVisual;
>>>>>>> 429ef7c0bfb738c993617c072373dee49aa10b3a
        Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=False;Encrypt=False;";
        IEnumerable<Pedido> pedido(string f1, string f2)
        {
            List<Pedido> temporal = new List<Pedido>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("usp_pedido_iniciofin", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@f1", DateTime.Parse(f1));
                cmd.Parameters.AddWithValue("@f2", DateTime.Parse(f2));
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    temporal.Add(new Pedido()
                    {
                        id_venta = dr.GetString(0),
                        fecha_venta = dr.GetDateTime(1),
                        dni_usuario = dr.GetString(2),
                        nom_usuario = dr.GetString(3),
                        monto = dr.GetDecimal(4)
                    });
                }

            }
            return temporal;
        }
        IEnumerable<Pedido> pedidoLista()
        {


            List<Pedido> temporal = new List<Pedido>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("usp_listarpedido", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    temporal.Add(new Pedido()
                    {
                        id_venta = dr.GetString(0),
                        fecha_venta = dr.GetDateTime(1),
                        dni_usuario = dr.GetString(2),
                        nom_usuario = dr.GetString(3),
                        monto = dr.GetDecimal(4)
                    });
                }

            }
            return temporal;
        }
        Pedido Buscar(string id)
        {

            return pedidoLista().Where(c => c.id_venta == id).FirstOrDefault();
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Pedido(string f1 = "1/1/1800 12:00:00 AM", string f2 = "1/1/9000 12:00:00 AM")
        {

            ViewBag.f1 = f1;
            ViewBag.f2 = f2;

            return View(await Task.Run(() => pedido(f1, f2)));
        }


        [HttpGet]
        public async Task<IActionResult> BuscaPedido(string id)
        {
            Pedido reg = Buscar(id);
            return View(await Task.Run(() => reg));

        }

        
    }
}
