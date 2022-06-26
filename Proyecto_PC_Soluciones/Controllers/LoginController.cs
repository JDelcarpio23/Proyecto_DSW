using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Session;
using System.Data;
using Proyecto_PC_Soluciones.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Proyecto_PC_Soluciones.Controllers
{
    public class LoginController : Controller
    {
        const string cadena = @"server=JDelcarpio-I7; database=ProyectoVisual; Trusted_Connection=True; MultipleActiveResultSets=True; TrustServerCertificate=False; Encrypt=False";
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

        IEnumerable<Usuarios> ListaUsuario()
        {
            List<Usuarios> temporal = new List<Usuarios>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("exec usp_valida_usuario", cn);
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Usuarios obj = new Usuarios()
                    {
                        id_usuario = dr.GetInt32(0),
                        nom_usuario = dr.GetString(1),
                        dni_usuario = dr.GetString(2),
                        user_usuario = dr.GetString(3),
                        pass_usuario = dr.GetString(4),
                        id_tipo_usuario = dr.GetInt32(5),
                    };
                    temporal.Add(obj);
                }
            }
            return temporal;
        }

        Usuarios ValidarUsuario(string usuario, string clave)
        {
            return ListaUsuario().Where(u => u.user_usuario == usuario && u.pass_usuario == clave).FirstOrDefault();
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
            var _usuario = ValidarUsuario(reg.user_usuario, reg.pass_usuario);

            if (_usuario != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, _usuario.nom_usuario),
                    new Claim("user_usuario", _usuario.user_usuario)
                };

                if (_usuario.id_tipo_usuario == 1)
                {
                    claims.Add(new Claim(ClaimTypes.Role, "Cliente"));
                }

                if (_usuario.id_tipo_usuario == 2) { 
                    claims.Add(new Claim(ClaimTypes.Role, "Administrador"));
                }
                
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                HttpContext.Session.SetString(sesion, xusuario);
            }

            if (string.IsNullOrEmpty(xusuario))
            {
                ModelState.AddModelError("", "Usuario o Clave Incorrecta");
                return View(await Task.Run(() => reg));
            }

            if (_usuario.id_tipo_usuario == 1)
            {
                return RedirectToAction("Portal", "ECommerce");
            }

            if (_usuario.id_tipo_usuario == 2)
            {
                return RedirectToAction("Index", "Articulo");
            }

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

        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Inicio", "Login");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
