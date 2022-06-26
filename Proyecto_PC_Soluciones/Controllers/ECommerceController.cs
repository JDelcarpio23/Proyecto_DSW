using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Session;
using System.Data;
using Newtonsoft.Json;
using Proyecto_PC_Soluciones.Models;
using Microsoft.AspNetCore.Authorization;
namespace Proyecto_PC_Soluciones.Controllers
{
    public class ECommerceController : Controller
    {
        const string cadena = @"server=BRYAN; database=ProyectoVisual; Trusted_Connection=True; MultipleActiveResultSets=True; TrustServerCertificate=False; Encrypt=False";
        string sesion = "";

        [Authorize(Roles = "Cliente")]
        IEnumerable<Articulo> articulo()
        {
            List<Articulo> temporal = new List<Articulo>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cm = new SqlCommand("select * from tb_articulo", cn);
                SqlDataReader dr = cm.ExecuteReader();

                while (dr.Read())
                {
                    Articulo articulo = new Articulo()
                    {
                        id_articulo = dr.GetInt32(0),
                        nom_articulo = dr.GetString(1),
                        pre_articulo = dr.GetDecimal(2),
                        stock_articulo = dr.GetInt32(3),
                    };

                    temporal.Add(articulo);

                }
            }
            return temporal;
        }
        Articulo Buscar(int id)
        {
            return articulo().FirstOrDefault(a => a.id_articulo == id);
        }
        public async Task<IActionResult> Portal()
        {
            if (HttpContext.Session.GetString("carrito") == null)
            {
                HttpContext.Session.SetString("carrito", JsonConvert.SerializeObject(new List<Registro>()));
            }
            return View(await Task.Run(() => articulo()));
        }
        public async Task<IActionResult> Agregar(int? id = null)
        {
            if (id == null)
                return RedirectToAction("Portal");
            else
                return View(await Task.Run(() => Buscar((int)id)));
        }
        [HttpPost]public async Task<IActionResult> Agregar(int codigo, int cantidad)
        {
            Articulo reg = Buscar(codigo);

            Registro item = new Registro()
            {
                id_articulo = reg.id_articulo,
                nom_articulo = reg.nom_articulo,
                pre_articulo = reg.pre_articulo,
                cantidad = cantidad,
            };

            List<Registro> auxiliar =
                JsonConvert.DeserializeObject<List<Registro>>(HttpContext.Session.GetString("carrito"));

            auxiliar.Add(item);

            HttpContext.Session.SetString("carrito", JsonConvert.SerializeObject(auxiliar));

            ViewBag.mensaje = "Articulo Agregado";

            return View(await Task.Run(() => reg));
        }
        public async Task<IActionResult> Carrito()
        {
            if (HttpContext.Session.GetString("carrito") == null)
                return RedirectToAction("Portal");

            List<Registro> auxiliar =
                JsonConvert.DeserializeObject<List<Registro>>(HttpContext.Session.GetString("carrito"));

            if (auxiliar.Count == 0)
                return RedirectToAction("Portal");

            //si tiene registros
            return View(await Task.Run(() => auxiliar));
        }
        public async Task<IActionResult> Delete(int id)
        {
            List<Registro> auxiliar =
                JsonConvert.DeserializeObject<List<Registro>>(HttpContext.Session.GetString("carrito"));

            Registro item = auxiliar.FirstOrDefault(s => s.id_articulo == id);
            auxiliar.Remove(item);

            HttpContext.Session.SetString("carrito", JsonConvert.SerializeObject(auxiliar));

            return RedirectToAction("Carrito");
        }
        public async Task<IActionResult> Pedido()
        {
            List<Registro> auxiliar =
                JsonConvert.DeserializeObject<List<Registro>>(HttpContext.Session.GetString("carrito"));

            return View(await Task.Run(() => auxiliar));
        }
        [HttpPost]public async Task<IActionResult> Pedido(string dni, string nombre)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlTransaction tr = cn.BeginTransaction(IsolationLevel.Serializable);
                try
                {
                    List<Registro> auxiliar =
                    JsonConvert.DeserializeObject<List<Registro>>(HttpContext.Session.GetString("carrito"));

                    SqlCommand cmd = new SqlCommand("usp_inserta_venta", cn, tr);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id_venta", SqlDbType.VarChar, 8).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@dni_usuario", dni);
                    cmd.Parameters.AddWithValue("@nom_usuario", nombre);
                    cmd.Parameters.AddWithValue("@monto", auxiliar.Sum(x => x.monto));
                    cmd.ExecuteNonQuery();

                    string idpedido = cmd.Parameters["@id_venta"].Value.ToString();

                    foreach (Registro reg in auxiliar)
                    {
                        cmd = new SqlCommand(
                            "exec usp_inserta_detaVenta @id_venta, @id_articulo, @precio, @cantidad", cn, tr);
                        cmd.Parameters.AddWithValue("@id_venta", idpedido);
                        cmd.Parameters.AddWithValue("@id_articulo", reg.id_articulo);
                        cmd.Parameters.AddWithValue("@precio", reg.pre_articulo);
                        cmd.Parameters.AddWithValue("@cantidad", (Int16)reg.cantidad);
                        cmd.ExecuteNonQuery();
                    }

                    auxiliar.ForEach(it =>
                    {
                        cmd = new SqlCommand(
                            "exec usp_actualiza_stock_articulo @id_articulo, @cantidad", cn, tr);
                        cmd.Parameters.AddWithValue("@id_articulo", it.id_articulo);
                        cmd.Parameters.AddWithValue("@cantidad", (Int16)it.cantidad);
                        cmd.ExecuteNonQuery();
                    });

                    //si todo esta OK
                    tr.Commit();
                    mensaje = $"Se ha registrador el pedido numero {idpedido} satisfactoriamente";
                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                    tr.Rollback();//deshacer el proceso en caso de error
                }
                finally { cn.Close(); }
            }

            return RedirectToAction("Mensaje", new { msj = mensaje });
        }
        public IActionResult Mensaje(string msj)
        {
            ViewBag.mensaje = msj;
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
        
    }
}
