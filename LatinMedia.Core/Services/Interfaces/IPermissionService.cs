using System;
using System.Collections.Generic;
using System.Text;
using LatinMedia.DataLayer.Entities.Permissions;
using LatinMedia.DataLayer.Entities.User;

namespace LatinMedia.Core.Services.Interfaces
{
    public interface IPermissionService
    {

        #region Role

        List<Role> GetRoles();
        void AddRolesToUser(List<int> roleIds, int userId);
        void EditRolesUser(int userId, List<int> rolesId);

        List<string> GetUserRoles(int userId);
        void RemoveRolesUser(int userId);

        int AddRole(Role role);
        Role GetRoleById(int roleId);
        void UpdateRole(Role role);
        void DeleteRole(Role role);

        #endregion

        #region Permission

        List<Permission> GetAllPermissions();
        void AddPermissionsToRole(int roleId, List<int> permissions);
        List<int> PermissionsRole(int roleId);
        void UpdatePermissionsRole(int roleId, List<int> permissions);
        bool CheckPermission(int permissionId, string email);
        bool CheckUserIsRole(string email);

        #endregion

    }
}
