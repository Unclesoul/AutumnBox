﻿/*************************************************
** auth： zsh2401@163.com
** date:  2018/3/18 22:28:15 (UTC +8:00)
** desc： ...
*************************************************/
using AutumnBox.OpenFramework.Open;

namespace AutumnBox.OpenFramework.Internal
{
    /// <summary>
    /// 框架加载器
    /// </summary>
    public static class FrameworkLoader
    {
        /// <summary>
        /// 设置Log api
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="api"></param>
        public static void SetLogApi(Context ctx, ILogApi api)
        {
            ctx.PermissionCheck();
            OpenApi.Log = api;
        }
        /// <summary>
        /// 设置GuiApi
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="api"></param>
        public static void SetGuiApi(Context ctx, IGuiApi api)
        {
            ctx.PermissionCheck();
            OpenApi.Gui = api;
        }
    }
}
