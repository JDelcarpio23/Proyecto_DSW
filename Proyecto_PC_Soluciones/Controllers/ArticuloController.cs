using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; //selectList
using Microsoft.Data.SqlClient;
using System.Data;
using Proyecto_PC_Soluciones.Models;
using Microsoft.AspNetCore.Authorization;
namespace Proyecto_PC_Soluciones.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ArticuloController : Controller
    {
<<<<<<< HEAD
<<<<<<< HEAD
        string cadena = @"server=JDelcarpio-I7; database=ProyectoVisual; " +
=======
        string cadena = @"server=BRYAN; database=ProyectoVisual; " +
>>>>>>> 429ef7c0bfb738c993617c072373dee49aa10b3a
=======
        string cadena = @"server=BRYAN; database=ProyectoVisual; " +
>>>>>>> 429ef7c0bfb738c993617c072373dee49aa10b3a
        "Trusted_Connection=True; MultipleActiveResultSets=True; TrustServerCertificate=False; Encrypt=False";
        IEnumerable<Articulo> articulos()
        {
            List<Articulo> temporal = new List<Articulo>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("exec usp_Articulo", cn);
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Articulo obj = new Articulo()
                    {
                        id_articulo = dr.GetInt32(0),
                        nom_articulo = dr.GetString(1),
                        pre_articulo = dr.GetDecimal(2),
                        stock_articulo = dr.GetInt32(3),
                    };
                    temporal.Add(obj);
                }
            }
            return temporal;
        }
        Articulo Buscar(int codigo)
        {
            return articulos().Where(c => c.id_articulo == codigo).FirstOrDefault();
        }
        public async Task<IActionResult> Index()
        {
            return View(await Task.Run(() => articulos()));
        }
        public async Task<IActionResult> Create()
        {
            return View(await Task.Run(() => new Articulo()));
        }
        [HttpPost]
        public async Task<IActionResult> Create(Articulo reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_agregar_articulo", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@nom_articulo", reg.nom_articulo);
                    cmd.Parameters.AddWithValue("@pre_articulo", reg.pre_articulo);
                    cmd.Parameters.AddWithValue("@stock_articulo", reg.stock_articulo);
                    cn.Open();
                    int c = cmd.ExecuteNonQuery(); //ejecuta y retorna el numero de reg insertados
                    mensaje = $"Se ha insertado {c} Articulo";
                }
                catch (Exception ex) { mensaje = ex.Message;/*en caso de error*/ }
                finally { cn.Close(); }
            }
            //refrescar la vista, el retorno del POST
            ViewBag.mensaje = mensaje;
            return View(await Task.Run(() => reg));
        }
        public async Task<IActionResult> Edit(int codigo)
        {
            // si no esta vacio id
            Articulo reg = Buscar(codigo);

            //enviar el Cliente
            return View(await Task.Run(() => reg));
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Articulo reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_actualiza_articulo", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_articulo", reg.id_articulo);
                    cmd.Parameters.AddWithValue("@nom_articulo", reg.nom_articulo);
                    cmd.Parameters.AddWithValue("@pre_articulo", reg.pre_articulo);
                    cmd.Parameters.AddWithValue("@stock_articulo", reg.stock_articulo);
                    cn.Open();
                    int c = cmd.ExecuteNonQuery(); //ejecuta
                    mensaje = $"Se ha actualizado {c} Articulo";
                }
                catch (Exception ex) { mensaje = ex.Message;/*en caso de error*/ }
                finally { cn.Close(); }
            }
            //refrescar la vista, el retorno del POST
            ViewBag.mensaje = mensaje;
            return View(await Task.Run(() => reg));
        }


    }
}
