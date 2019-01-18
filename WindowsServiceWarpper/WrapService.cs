using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceProcess;
using System.IO;

namespace cn.zhg.windowsservicewarpper
{
    public class WarpService : ServiceBase
    {
        /**配置文件名*/
        private const string PROPFILE = "warpservice.ini";
        private string Exe;
        private string StartExe;
        private string StopExe;
        private string StartArgs;
        private string StopArgs;
        private string WorkingDirectory;
        private string StartWorkingDirectory;
        private string StopWorkingDirectory;
        private bool AutoKill = false;
        private Process startProcess;
        private Process stopProcess;
        public WarpService()
        {
            this.ServiceName = "封装服务";
            log("初始化服务\n当前目录:" + Directory.GetCurrentDirectory() + "\n当前线程:" + Process.GetCurrentProcess().ProcessName + "\n当前exe名称:" + Process.GetCurrentProcess().MainModule.FileName + "\n启动目录:" + AppDomain.CurrentDomain.BaseDirectory);
            initProp();
        }
        /**
         * 初始化配置 
         **/
        private void initProp()
        {
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "/" + PROPFILE;
            if (!File.Exists(fileName))
            {
                File.WriteAllText(fileName, Properties.Resources.warpservice);
            }
            else
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                string[] lines = File.ReadAllLines(fileName);
                if (lines != null && lines.Length > 0)
                {
                    foreach (string _line in lines)
                    {
                        string line = _line.Trim();
                        if (line.Length == 0 || line.StartsWith("#"))
                        {
                            continue;
                        }
                        int i = line.IndexOf('=');
                        if (i == -1)
                        {
                            dict.Add(line, null);
                        }
                        else
                        {
                            string key = line.Substring(0, i).Trim();
                            string val = line.Substring(i + 1).Trim();
                            dict.Add(key, val);
                        }
                    }
                }
                if (dict.ContainsKey("ServiceName") &&isNoEmpty(dict["ServiceName"]))
                {
                    this.ServiceName = dict["ServiceName"];
                }
                dict.TryGetValue("Exe", out this.Exe);
                dict.TryGetValue("StartExe", out this.StartExe);
                dict.TryGetValue("StopExe", out this.StopExe);
                dict.TryGetValue("StartArgs", out this.StartArgs);
                dict.TryGetValue("StopArgs", out this.StopArgs);
                dict.TryGetValue("WorkingDirectory", out this.WorkingDirectory);
                dict.TryGetValue("StartWorkingDirectory", out this.StartWorkingDirectory);
                dict.TryGetValue("StopWorkingDirectory", out this.StopWorkingDirectory); 

                if (dict.ContainsKey("AutoKill") && isNoEmpty(dict["AutoKill"]))
                {
                    this.AutoKill = dict["AutoKill"].ToUpper().Equals("TRUE");
                }
            }
        }

        protected override void OnStart(string[] args)
        {
            ProcessStartInfo info = null;
            if (isNoEmpty(StartExe))
            {
                info = new ProcessStartInfo(StartExe);
            }
            else if (isNoEmpty(Exe))
            {
                info = new ProcessStartInfo(Exe);
            }
            if (info != null)
            {
                if (isNoEmpty(StartArgs))
                {
                    info.Arguments = StartArgs;
                }
                if (isNoEmpty(StartWorkingDirectory))
                {
                    info.WorkingDirectory = StartWorkingDirectory;
                }
                else if (isNoEmpty(WorkingDirectory))
                {
                    info.WorkingDirectory = WorkingDirectory;
                }else
                {
                    info.WorkingDirectory = new FileInfo(info.FileName).DirectoryName;
                }
                try
                {
                    info.CreateNoWindow = true;
                    info.UseShellExecute = true; 
                    log("启动命令:" + info.FileName + "\n参数:" + info.Arguments + "\n工作目录:" + info.WorkingDirectory);
                    startProcess = new Process();
                    startProcess.StartInfo = info;
                    bool ret = startProcess.Start();
                    log("启动结果:" + ret + "\nId:" + startProcess.Id);
                }
                catch (Exception igr)
                {
                    log("启动异常:" + igr.Message);
                }
            }
        }

        protected override void OnStop()
        {
            if (AutoKill)
            {
                if (startProcess != null)
                {
                    try
                    {
                        startProcess.Kill();
                    }
                    catch (Exception)
                    { }
                }
                return;
            }
            ProcessStartInfo info = null;
            if (isNoEmpty(StopExe))
            {
                info = new ProcessStartInfo(StopExe);
            }
            else if (isNoEmpty(Exe))
            {
                info = new ProcessStartInfo(Exe);
            }
            if (info != null)
            {
                if (isNoEmpty(StopArgs))
                {
                    info.Arguments = StopArgs;
                }
                if (isNoEmpty(StopWorkingDirectory))
                {
                    info.WorkingDirectory = StopWorkingDirectory;
                }
                else if (isNoEmpty(WorkingDirectory))
                {
                    info.WorkingDirectory = WorkingDirectory;
                }
                else
                {
                    info.WorkingDirectory = new FileInfo(info.FileName).DirectoryName;
                }
                try
                {
                    info.CreateNoWindow = true;
                    info.UseShellExecute = true; 
                    log("停止命令:" + info.FileName + "\n参数:" + info.Arguments + "\n工作目录:" + info.WorkingDirectory);
                    stopProcess = new Process();
                    stopProcess.StartInfo = info;
                    bool ret = stopProcess.Start();
                    log("停止结果:" + ret + "\nId:" + startProcess.Id);
                }
                catch (Exception igr)
                {
                    log("停止异常:" + igr.Message);
                }
            }
        }
        private bool isNoEmpty(string str)
        {
            return str != null && str.Length > 0;
        }
        /**
         * 写入日志文件
         * */
        private void log(string msg)
        {
            this.EventLog.WriteEntry(msg, EventLogEntryType.Information);
        }
    }
}
