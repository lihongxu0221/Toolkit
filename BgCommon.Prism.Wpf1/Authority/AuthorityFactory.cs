// using BgCommon.Configuration;
// using BgCommon.Prism.Wpf.Authority.Entities;
// using BgCommon.Prism.Wpf.Authority.Models;
// using BgCommon.Prism.Wpf.Authority.Modules.User.Views;
// using BgCommon.Prism.Wpf.Authority.Services;
// using Prism.Dialogs;

// namespace BgCommon.Prism.Wpf.Authority;

// /// <summary>
// /// 权限校验工具类.
// /// </summary>
// public static class AuthorityFactory
// {
//     public static readonly string LoginParamFile = "LoginParam.data";

//     /// <summary>
//     /// 显示登陆界面.
//     /// </summary>
//     /// <param name="container">ioc容器.</param>
//     /// <param name="isAllowAutoLoad">是否允许自动登陆.</param>
//     /// <returns>Task.</returns>
//     public static async Task<AuthorityResult> ShowLoginViewAsync(this IContainerExtension container, bool isAllowAutoLoad = false)
//     {
//         ArgumentNullException.ThrowIfNull(container, nameof(container));
//         AuthorityResult? loginResult = null;
//         try
//         {
//             IDialogService dialogService = container.Resolve<IDialogService>();
//             IAuthorityService authorityService = container.Resolve<IAuthorityService>();
//             ConfigurationMgr<LoginParam>? loginParam = new ConfigurationMgr<LoginParam>(LoginParamFile, ConfigurationMgr<LoginParam>.SerializeMethod.Json);
//             try
//             {
//                 loginParam.LoadFromFile();
//                 if (loginParam.Entity == null)
//                 {
//                     loginParam.Entity = new LoginParam();
//                     await loginParam.SaveToFileAsync();
//                 }
//             }
//             catch (Exception)
//             {
//                 loginParam.Entity = new LoginParam();
//                 await loginParam.SaveToFileAsync();
//             }

//             loginParam.Entity.IsAllowAutoLogin = isAllowAutoLoad;
//             if (!isAllowAutoLoad)
//             {
//                 loginParam.Entity.IsAutoLogin = false;
//             }

//             // 未勾选自动登录时，显示登陆界面
//             if (isAllowAutoLoad && authorityService.IsDebugMode)
//             {
//                 UserInfo? user = Users.FirstOrDefault(u => u.Authority == UserAuthority.Admin);
//                 if (user != null)
//                 {
//                     loginResult = await LoginAsync(user.UserName, user.Password);
//                     return loginResult!;
//                 }
//             }

//             IDialogResult? ret = await dialogService.ShowDialogAsync<UserLoginView>(
//                 Constraints.LoginHostWindow,
//                 keys =>
//                 {
//                     keys.Add(Constraints.LoginParam, loginParam!);
//                 });

//             if (ret != null)
//             {
//                 loginResult = ret.Parameters.GetValue<AuthorityResult>(Constraints.LoginResult);
//                 if (ret.Exception != null)
//                 {
//                     LogRun.Error(ret.Exception);
//                 }

//                 try
//                 {
//                     LoginParam? loginParam1 = ret.Parameters.GetValue<LoginParam>(Constraints.LoginParam);
//                     if (loginParam1 != null)
//                     {
//                         this.loginParam!.Entity = loginParam1;
//                         await this.loginParam.SaveToFileAsync();
//                     }
//                 }
//                 catch (Exception ex)
//                 {
//                     LogRun.Error(ex, "保存登陆参数出错！");
//                 }
//             }

//             return loginResult!;
//         }
//         catch (Exception ex)
//         {
//             LogRun.Error(ex);
//             loginResult = new AuthorityResult();
//             loginResult.Code = -1;
//             loginResult.Message = ex.Message;
//             return loginResult;
//         }
//     }

//     /// <summary>
//     /// 切换用户.
//     /// </summary>
//     /// <param name="container">ioc容器.</param>
//     /// <returns>返回登陆结果.</returns>
//     public static Task<AuthorityResult> ShowSwitchUserViewAsync(this IContainerExtension container)
//     {
//         AuthorityResult? loginResult = null;
//         try
//         {
//             LoginParam loginParam = new LoginParam();
//             loginParam.UserName = this.loginParam?.Entity?.UserName ?? string.Empty;
//             loginParam.Password = this.loginParam?.Entity?.Password ?? string.Empty;
//             loginParam.IsAllowAutoLogin = false;
//             loginParam.IsAllowRemember = false;
//             loginParam.IsAutoLogin = false;
//             loginParam.IsRemember = false;

//             IDialogResult? ret = await dialogService.ShowDialogAsync<UserLoginView>(
//                  Constraints.LoginHostWindow,
//                  keys => keys.Add(Constraints.LoginParam, loginParam));
//             if (ret != null)
//             {
//                 loginResult = ret.Parameters.GetValue<AuthorityResult>(Constraints.LoginResult);
//                 if (ret.Exception != null)
//                 {
//                     LogRun.Error(ret.Exception);
//                 }
//             }

//             return loginResult!;
//         }
//         catch (Exception ex)
//         {
//             LogRun.Error(ex);
//             loginResult = new AuthorityResult();
//             loginResult.Code = -1;
//             loginResult.Success = false;
//             loginResult.Message = ex.Message;
//             return loginResult;
//         }
//     }
// }
