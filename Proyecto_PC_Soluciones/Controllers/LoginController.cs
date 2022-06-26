using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Session;
using System.Data;
using Proyecto_PC_Soluciones.Models;

namespace Proyecto_PC_Soluciones.Controllers
{
    public class LoginController : Controller
    {
        const string cadena = @"server=DESKTOP-V44JFQH\SQLEXPRESS; database=ProyectoVisual; Trusted_Connection=True; MultipleActiveResultSets=True; TrustServerCertificate=False; Encrypt=False";
        string sesion = "";

        string Ingreso(string usuario, string clave)
        {
            string ingreso = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_login_usuario", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@login", usuario);
                    cmd.Parameters.AddWithValue("@clave", clave);
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        ingreso = usuario;
                    }
                }
                catch (Exception) { ingreso = ""; }
                finally { cn.Close(); }

                return ingreso;
            }
        }
        public async Task<IActionResult> Inicio()
        {
            HttpContext.Session.SetString(sesion, "");
            return View(await Task.Run(() => new Usuarios()));
        }
        [HttpPost]public async Task<IActionResult> Inicio(Usuarios reg)
        {
            /*if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Ingrese los datos");
                return View(reg);
            }*/

            string xusuario = Ingreso(reg.user_usuario, reg.pass_usuario);
            if (string.IsNullOrEmpty(xusuario))
            {
                ModelState.AddModelError("", "Usuario o Clave Incorrecta");
                return View(await Task.Run(() => reg));
            }

            HttpContext.Session.SetString(sesion, xusuario);
            return RedirectToAction("Portal", "ECommerce");
        }
        public IActionResult Plataforma()
        {
            ViewBag.usuario = HttpContext.Session.GetString(sesion);
            return View();
        }
        public async Task<IActionResult> Create()
        {
            return View(await Task.Run(() => new Usuarios()));
        }
        [HttpPost]public async Task<IActionResult> Create(Usuarios reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_registrar_usuario", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@nom_usuario", reg.nom_usuario);
                    cmd.Parameters.AddWithValue("@dni_usuario", reg.dni_usuario);
                    cmd.Parameters.AddWithValue("@user_usuario", reg.user_usuario);
                    cmd.Parameters.AddWithValue("@pass_usuario", reg.pass_usuario);
                    //cmd.Parameters.AddWithValue("@id_tipo_usuario", reg.id_tipo_usuario);
                    cn.Open();

                    int c = cmd.ExecuteNonQuery();
                    mensaje = $"Se ha Registrado {c} Usuario";
                }

                catch (Exception ex) { mensaje = ex.Message; /* en caso de error */ }
                finally { cn.Close(); }
            }
            ViewBag.mensaje = mensaje;
            return View(await Task.Run(() => reg));

        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
