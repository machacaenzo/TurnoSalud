using Entities;
using Model.Dao;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controllers
{
    public class UsuarioController
    {
        UsuarioDao usuarioDao = new UsuarioDao();
        public List<Usuario> ListarUsuarios(string cTexto, bool activos = true)
        {
            return usuarioDao.listarUsuarios(cTexto, activos);
        }
        
        public string AgregarUsuario(Usuario usuario)
        {
            return usuarioDao.agregarUsuario(usuario);
        }
        public string ActualizarUsuario(Usuario usuario)
        {
            return usuarioDao.actualizarUsuario(usuario);
        }
        public string EliminarUsuario(int idUsuario)
        {
            return usuarioDao.darBajaUsuario(idUsuario);
        }
        public string ReactivarUsuario(int idUsuario)
        {
            return usuarioDao.darAltaUsuario(idUsuario);
        }
        public bool Login(string user, string pas)
        {
            return usuarioDao.login(user, pas);
        }
    }
}
