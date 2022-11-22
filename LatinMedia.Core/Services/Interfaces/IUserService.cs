using System;
using System.Collections.Generic;
using System.Text;
using LatinMedia.Core.ViewModels;
using LatinMedia.DataLayer.Entities.User;
using LatinMedia.DataLayer.Entities.Wallet;

namespace LatinMedia.Core.Services.Interfaces
{
    public interface IUserService
    {
        #region Account
        bool IsExsitEmail(string email);
        bool IsExsitMobile(string mobile);
        int AddUser(User user);
        User LoginUser(LoginViewModel login);
        User GetUserByEmail(string email);
        User GetUserByActiveCode(string activeCode);
        User GetUserById(int userId);
        int GetUserIdByEmail(string email);
        void UpdateUser(User user);
        bool ActiveAccount(string activeCode);


        #endregion

        #region UserPanel

        InformationUserViewModel GetUserInformation(string email);
        InformationUserViewModel GetUserInformation(int userId);
        EditProfileViewModel GetDataForEditProfileUser(string email);
        void EditProfile(string email, EditProfileViewModel profile);
        bool CompareOldPassword(string email, string oldPassword);
        void ChangeUserPassword(string email, string newPassword);

        bool ChangeUserEmail(int userId, string token, string newEmail);
       

        #endregion

        #region Wallet

        int BalanceWalletUser(string email);
        List<WalletInfoViewModel> GetWalletUser(string email);
        int ChargeWallet(string email, int amount, string description, bool isPay = false);
        int AddWallet(Wallet wallet);
        Wallet GetWalletByWalletId(int walletId);
        void UpdateWallet(Wallet wallet);

        #endregion

        #region Admin Panel

        SideBarAdminPanelViewModel GetSideBarAdminPanelData(string email); //GetDeleteUsers
        UsersForAdminViewModel GetUsers(int pageId = 1,int take=10 , string filterByEmail = "", string filterByMobile = ""); 
        UsersForAdminViewModel GetDeleteUsers(int pageId = 1, int take = 10, string filterByEmail = "", string filterByMobile = "");
        int AddUserFromAdmin(CreateUserViewModel model);
        EditUserViewModel GetUserForShowInEditMode(int userId);
        void EditUserFromAdmin(EditUserViewModel editUser);
        void DeleteUser(int userId);

        int UserFinalCount();
        List<User> GetLatestRegisteredUsers();

        #endregion
    }
}
