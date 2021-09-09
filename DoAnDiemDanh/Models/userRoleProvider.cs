using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace DoAnDiemDanh.Models
{
    public class userRoleProvider : RoleProvider
    {
        public override string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }
      
        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            BaseModel db = new BaseModel();

            var tkgv = db.Entity.TAIKHOANGIANGVIENs.Where(s => s.TaiKhoan == username).SingleOrDefault();
            var tksv = db.Entity.TAIKHOANSINHVIENs.Where(s => s.TaiKhoan == username).SingleOrDefault();
            string[] role = null;
            if(tksv != null)
            {
                role = (from tk in db.Entity.TAIKHOANSINHVIENs
                            where tk.TaiKhoan == username
                            join quyen in db.Entity.QUYENs on tk.MaQuyen equals quyen.MaQuyen
                            select quyen.TenQuyen).ToArray();
            }

            if (tkgv != null)
            {
                role = (from tk in db.Entity.TAIKHOANGIANGVIENs
                            where tk.TaiKhoan == username
                            join quyen in db.Entity.QUYENs on tk.MaQuyen equals quyen.MaQuyen
                            select quyen.TenQuyen).ToArray();
            }
            if (role == null) role = new string[] { "SinhVien" };
            return role;
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}