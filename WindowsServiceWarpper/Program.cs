using System.ServiceProcess;

namespace cn.zhg.windowsservicewarpper
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main( )
        {  
            ServiceBase.Run(new ServiceBase[] {
              new WarpService()
           });
        }
    }
}
