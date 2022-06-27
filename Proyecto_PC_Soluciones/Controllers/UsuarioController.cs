using Microsoft.AspNetCore.Mvc;
using Microsoft.Data;
using Microsoft.Data.SqlClient;
using Proyecto_PC_Soluciones.Models;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Proyecto_PC_Soluciones.Controllers
{
    public class UsuarioController : Controller
    {
        const string cadena = @"server=JDelcarpio-I7; database=ProyectoVisual; Trusted_Connection=True; MultipleActiveResultSets=True; TrustServerCertificate=False; Encrypt=False";
        string sesion = "";

        IEnumerable<Tipo> tipo()
        {
            List<Tipo> temporal = new List<Tipo>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("exec usp_listarTipo", cn);
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Tipo obj = new Tipo()
                    {
                        id_tipo_usuario = dr.GetInt32(0),
                        tipo_usuario = dr.GetString(1),
                    };
                    temporal.Add(obj);
                }
            }
            return temporal;
        }
        IEnumerable<Usuarios> usuario()
        {
            List<Usuarios> temporal = new List<Usuarios>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("exec usp_listarUsuario", cn);
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Usuarios obj = new Usuarios()
                    {
                        id_usuario = dr.GetInt32(0),
                        nom_usuario = dr.GetString(1),
                        dni_usuario = dr.GetString(2),
                        user_usuario=dr.GetString(3),
                        pass_usuario = dr.GetString(4),
                        id_tipo_usuario = dr.GetInt32(5),
                    };
                    temporal.Add(obj);
                }
            }
            return temporal;
        }
        Usuarios Buscar(int id)
        {
            return usuario().Where(u => u.id_usuario == id).FirstOrDefault();
        }
        public async Task<IActionResult> Index()
        {
            return View(await Task.Run(() => usuario()));
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.tipos = new SelectList(await Task.Run(() => tipo()), "id_tipo_usuario", "tipo_usuario");
            return View(await Task.Run(() => new Usuarios()));
        }
        [HttpPost]public async Task<IActionResult> Create(Usuarios reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_agregar_usuario", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@nom_usuario", reg.nom_usuario);
                    cmd.Parameters.AddWithValue("@dni_usuario", reg.dni_usuario);
                    cmd.Parameters.AddWithValue("@user_usuario", reg.user_usuario);
                    cmd.Parameters.AddWithValue("@pass_usuario", reg.pass_usuario);
                    cmd.Parameters.AddWithValue("@id_tipo_usuario", reg.id_tipo_usuario);
                    cn.Open();

                    int c = cmd.ExecuteNonQuery();
                    mensaje = $"Se ha insertado {c} usuario";
                }

                catch (Exception ex) { mensaje = ex.Message; }
                finally { cn.Close(); }
            }

            ViewBag.mensaje = mensaje;
            ViewBag.tipos = new SelectList(await Task.Run(() => tipo()), "id_tipo_usuario", "tipo_usuario", reg.id_tipo_usuario);
            return View(await Task.Run(() => reg));

        }
        public async Task<IActionResult> Edit(int id)
        {
            Usuarios reg = Buscar(id);
            ViewBag.tipos = new SelectList(await Task.Run(() => tipo()), "id_tipo_usuario", "tipo_usuario", reg.id_tipo_usuario);

            return View(await Task.Run(() => reg));
        }
        [HttpPost]public async Task<IActionResult> Edit(Usuarios reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_actualiza_usuario", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario", reg.id_usuario);
                    cmd.Parameters.AddWithValue("@nom_usuario", reg.nom_usuario);
                    cmd.Parameters.AddWithValue("@dni_usuario", reg.dni_usuario);
                    cmd.Parameters.AddWithValue("@user_usuario", reg.user_usuario);
                    cmd.Parameters.AddWithValue("@pass_usuario", reg.pass_usuario);
                    cmd.Parameters.AddWithValue("@id_tipo_usuario", reg.id_tipo_usuario);
                    cn.Open();

                    int c = cmd.ExecuteNonQuery();
                    mensaje = $"Se ha actualizado {c} usuario";
                }

                catch (Exception ex) { mensaje = ex.Message; }
                finally { cn.Close(); }
            }

            ViewBag.mensaje = mensaje;
            ViewBag.tipos = new SelectList(await Task.Run(() => tipo()), "id_tipo_usuario", "tipo_usuario", reg.id_tipo_usuario);
            return View(await Task.Run(() => reg));

        }
        public async Task<IActionResult> Delete(int id)
        {
            Usuarios reg = Buscar(id);
            return View(await Task.Run(() => reg));
        }
        [HttpPost]public async Task<IActionResult> Delete(Usuarios reg, int id)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("delete from tb_usuario Where id_usuario = " + id, cn);
                    cn.Open();

                    int c = cmd.ExecuteNonQuery();
                    mensaje = $"Se ha eliminado {c} usuario";
                }
                catch (Exception ex) { mensaje = ex.Message; }
                finally { cn.Close(); }
            }
            ViewBag.mensaje = mensaje;
            return View(await Task.Run(() => reg));
        }

    }
}
