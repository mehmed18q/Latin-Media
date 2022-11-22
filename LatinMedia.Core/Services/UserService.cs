using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LatinMedia.Core.Convertors;
using LatinMedia.Core.Genertors;
using LatinMedia.Core.Security;
using LatinMedia.Core.Services.Interfaces;
using LatinMedia.Core.ViewModels;
using LatinMedia.DataLayer.Context;
using LatinMedia.DataLayer.Entities.User;
using LatinMedia.DataLayer.Entities.Wallet;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Remotion.Linq.Clauses.ResultOperators;

namespace LatinMedia.Core.Services
{
    public class UserService : IUserService
    {
        private LatinMediaContext _context;
        private IHostingEnvironment _environment;
        private IPermissionService _permissionService;

        public UserService(LatinMediaContext context, IHostingEnvironment environment, IPermissionService permissionService)
        {
            _context = context;
            _environment = environment;
            _permissionService = permissionService;
        }

        public bool IsExsitEmail(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }

        public bool IsExsitMobile(string mobile)
        {
            return _context.Users.Any(u => u.Mobile == mobile);
        }

        public int AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return user.UserId;
        }

        public User LoginUser(LoginViewModel login)
        {
            string password = PasswordHelper.EncodePasswordMd5(login.Password);
            string email = FixedText.FixedEmail(login.Email);
            return _context.Users.SingleOrDefault(u => u.Email == email && u.Password == password);
        }

        public User GetUserByEmail(string email)
        {
            return _context.Users.SingleOrDefault(u => u.Email == email);
        }

        public User GetUserByActiveCode(string activeCode)
        {
            return _context.Users.SingleOrDefault(u => u.ActiveCode == activeCode);
        }

        public User GetUserById(int userId)
        {
            return _context.Users.Find(userId);
        }

        public int GetUserIdByEmail(string email)
        {
            return _context.Users.Single(u => u.Email == email).UserId;
        }

        public void UpdateUser(User user)
        {
            _context.Update(user);
            _context.SaveChanges();

        }

        public bool ActiveAccount(string activeCode)
        {
            var user = _context.Users.SingleOrDefault(u => u.ActiveCode == activeCode);
            if (user == null || user.IsActive)
                return false;

            user.IsActive = true;
            user.ActiveCode = GeneratorName.GenrateUniqeCode();

            _context.Users.Update(user);
            _context.SaveChanges();
            return true;

        }

        public InformationUserViewModel GetUserInformation(string email)
        {
            var user = GetUserByEmail(email);
            InformationUserViewModel information = new InformationUserViewModel();
            information.Email = user.Email;
            information.FirstName = user.FirstName;
            information.LastName = user.LastName;
            information.Mobile = user.Mobile;
            information.RegisterDate = user.CreateDate;
            information.Wallet = BalanceWalletUser(email);
            information.UserAvatar = user.UserAvatar;
            return information;
        }

        public InformationUserViewModel GetUserInformation(int userId)
        {
            var user = GetUserById(userId);
            InformationUserViewModel information = new InformationUserViewModel();
            information.Email = user.Email;
            information.FirstName = user.FirstName;
            information.LastName = user.LastName;
            information.Mobile = user.Mobile;
            information.RegisterDate = user.CreateDate;
            information.Wallet = BalanceWalletUser(user.Email);
            information.UserAvatar = user.UserAvatar;
            return information;
        }

        public EditProfileViewModel GetDataForEditProfileUser(string email)
        {
            return _context.Users.Where(u => u.Email == email).Select(u => new EditProfileViewModel()
            {
                FirstName = u.FirstName,
                LastName = u.LastName,
                Mobile = u.Mobile,
                AvatarName = u.UserAvatar,
                Email = u.Email
            }).Single();
        }

        public void EditProfile(string email, EditProfileViewModel profile)
        {
            if (profile.UserAvatar != null)
            {
                string imagePath = "";
                if (profile.AvatarName != "default.png")
                {
                    //------Delete User Image --------//
                    imagePath = Path.Combine(_environment.WebRootPath, "UserAvatar", profile.AvatarName);
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }

                }
                //-------Upload New User Image --------//
                profile.AvatarName = GeneratorName.GenrateUniqeCode() + Path.GetExtension(profile.UserAvatar.FileName);
                imagePath = Path.Combine(_environment.WebRootPath, "UserAvatar", profile.AvatarName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    profile.UserAvatar.CopyTo(stream);
                }
            }

            var user = GetUserByEmail(email);
            user.FirstName = profile.FirstName;
            user.LastName = profile.LastName;
            user.Mobile = profile.Mobile;
            user.UserAvatar = profile.AvatarName;

            UpdateUser(user);
        }

        public bool CompareOldPassword(string email, string oldPassword)
        {
            string hashOldPassword = PasswordHelper.EncodePasswordMd5(oldPassword);
            return _context.Users.Any(u => u.Email == email && u.Password == hashOldPassword);
        }

        public void ChangeUserPassword(string email, string newPassword)
        {
            var user = GetUserByEmail(email);
            user.Password = PasswordHelper.EncodePasswordMd5(newPassword);
            UpdateUser(user);
        }

        public bool ChangeUserEmail(int userId, string token, string newEmail)
        {
            var user = _context.Users.SingleOrDefault(u => u.UserId == userId && u.ActiveCode == token);
            if (user != null)
            {
                user.Email = EncryptData.Decrypt(newEmail);

                user.ActiveCode = GeneratorName.GenrateUniqeCode();
                UpdateUser(user);
                return true;
            }

            {
                return false;
            }
        }

        public int BalanceWalletUser(string email)
        {
            int userId = GetUserIdByEmail(email);
            //-----------واریز--------------------------------------------//
            var deposit = _context.Wallets.Where(w => w.UserId == userId
                                                      && w.TypeId == 1
                                                      && w.IsPay == true)
                                          .Select(w => w.Amount);

            //---------برداشت ---------------------------------------------//
            var removal = _context.Wallets.Where(w => w.UserId == userId
                                                      && w.TypeId == 2
                                                      && w.IsPay == true)
                                          .Select(w => w.Amount);

            return (deposit.Sum() - removal.Sum());


        }

        public List<WalletInfoViewModel> GetWalletUser(string email)
        {
            int userId = GetUserIdByEmail(email);

            return _context.Wallets.Where(w => w.UserId == userId && w.IsPay)
                .Select(w => new WalletInfoViewModel()
                {
                    DateTime = w.CreateDate,
                    Amount = w.Amount,
                    Description = w.Description,
                    Type = w.TypeId
                }).ToList();
        }

        public int ChargeWallet(string email, int amount, string description, bool isPay = false)
        {
            Wallet wallet = new Wallet()
            {
                Amount = amount,
                CreateDate = DateTime.Today,
                Description = description,
                IsPay = isPay,
                TypeId = 1,
                UserId = GetUserIdByEmail(email)
            };

            return AddWallet(wallet);
        }

        public int AddWallet(Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            _context.SaveChanges();
            return wallet.WalletId;
        }

        public Wallet GetWalletByWalletId(int walletId)
        {
            return _context.Wallets.Find(walletId);
        }

        public void UpdateWallet(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            _context.SaveChanges();
        }

        public SideBarAdminPanelViewModel GetSideBarAdminPanelData(string email)
        {
            return _context.Users.Where(u => u.Email == email).Select(u => new SideBarAdminPanelViewModel()
            {
                FirstName = u.FirstName,
                LastName = u.LastName,
                AvatarName = u.UserAvatar
            }).Single();

        }

        public UsersForAdminViewModel GetUsers(int pageId = 1, int take = 1, string filterByEmail = "", string filterByMobile = "")
        {
            IQueryable<User> result = _context.Users;  //lazyLoad;

            if (!string.IsNullOrEmpty(filterByEmail))
            {
                result = result.Where(u => u.Email.Contains(filterByEmail));
            }

            if (!string.IsNullOrEmpty(filterByMobile))
            {
                result = result.Where(u => u.Mobile == filterByMobile);
            }

            int takeData = take;
            int skip = (pageId - 1) * takeData;

            UsersForAdminViewModel list = new UsersForAdminViewModel();
            list.CurrentPage = pageId;
            list.PageCount = (int)Math.Ceiling(result.Count() / (double)takeData);
            list.Users = result.OrderByDescending(u => u.CreateDate).Skip(skip).Take(takeData).ToList();
            list.UserCounts = _context.Users.Count();

            return list;
        }

        public UsersForAdminViewModel GetDeleteUsers(int pageId = 1, int take = 10, string filterByEmail = "",
            string filterByMobile = "")
        {
            IQueryable<User> result = _context.Users.IgnoreQueryFilters().Where(u => u.IsDelete);  //lazyLoad;

            if (!string.IsNullOrEmpty(filterByEmail))
            {
                result = result.Where(u => u.Email.Contains(filterByEmail));
            }

            if (!string.IsNullOrEmpty(filterByMobile))
            {
                result = result.Where(u => u.Mobile == filterByMobile);
            }

            int takeData = take;
            int skip = (pageId - 1) * takeData;

            UsersForAdminViewModel list = new UsersForAdminViewModel();
            list.CurrentPage = pageId;
            list.PageCount = (int)Math.Ceiling(result.Count() / (double)takeData);
            list.Users = result.OrderByDescending(u => u.CreateDate).Skip(skip).Take(takeData).ToList();
            list.UserCounts = _context.Users.Count();

            return list;
        }

        public int AddUserFromAdmin(CreateUserViewModel model)
        {
            User user = new User();
            user.Email = model.Email;
            user.ActiveCode = GeneratorName.GenrateUniqeCode();
            user.CreateDate = DateTime.Now;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.IsActive = model.IsActive;
            user.Mobile = model.Mobile;
            user.Password = PasswordHelper.EncodePasswordMd5(model.Password);

            #region Save Avatar

            if (model.UserAvatar != null)
            {
                string imagePath = "";

                //-------Upload New User Image --------//
                user.UserAvatar = GeneratorName.GenrateUniqeCode() + Path.GetExtension(model.UserAvatar.FileName);
                imagePath = Path.Combine(_environment.WebRootPath, "UserAvatar", user.UserAvatar);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    model.UserAvatar.CopyTo(stream);
                }
            }

            #endregion

            return AddUser(user);
        }

        public EditUserViewModel GetUserForShowInEditMode(int userId)
        {
            return _context.Users.Where(u => u.UserId == userId).Select(u => new EditUserViewModel()
            {
                Email = u.Email,
                AvatarName = u.UserAvatar,
                FirstName = u.FirstName,
                IsActive = u.IsActive,
                LastName = u.LastName,
                Mobile = u.Mobile,
                UserId = u.UserId,
                UserRoles = u.UserRoles.Select(r => r.RoleId).ToList(),

            }).Single();
        }

        public void EditUserFromAdmin(EditUserViewModel editUser)
        {
            var user = GetUserById(editUser.UserId);
            user.FirstName = editUser.FirstName;
            user.LastName = editUser.LastName;
            user.Mobile = editUser.Mobile;
            user.IsActive = editUser.IsActive;

            if (!string.IsNullOrEmpty(editUser.Password))
            {
                user.Password = PasswordHelper.EncodePasswordMd5(editUser.Password);
            }

            if (editUser.UserAvatar != null)
            {
                string imagePath = "";
                if (editUser.AvatarName != "default.png")
                {
                    //------Delete User Image --------//
                    imagePath = Path.Combine(_environment.WebRootPath, "UserAvatar", editUser.AvatarName);
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }

                }
                //-------Upload New User Image --------//
                editUser.AvatarName = GeneratorName.GenrateUniqeCode() + Path.GetExtension(editUser.UserAvatar.FileName);
                imagePath = Path.Combine(_environment.WebRootPath, "UserAvatar", editUser.AvatarName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    editUser.UserAvatar.CopyTo(stream);
                }
            }

            user.UserAvatar = editUser.AvatarName;
            _context.Users.Update(user);
            _context.SaveChanges();


        }

        public void DeleteUser(int userId)
        {
            var user = GetUserById(userId);
            user.IsDelete = true;
            UpdateUser(user);
            _permissionService.RemoveRolesUser(user.UserId);
        }

        public int UserFinalCount()
        {
            return _context.Users.Count(u => u.IsActive);
        }

        public List<User> GetLatestRegisteredUsers()
        {
            return _context.Users.OrderByDescending(u=> u.CreateDate).Take(5).ToList();
        }
    }
}
