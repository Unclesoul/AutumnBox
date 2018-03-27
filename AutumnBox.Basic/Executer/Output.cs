/* =============================================================================*\
*
* Filename: OutputData.cs
* Description: 
*
* Version: 1.0
* Created: 9/15/2017 16:01:48(UTC+8:00)
* Compiler: Visual Studio 2017
* 
* Author: zsh2401
* Company: I am free man
*
\* =============================================================================*/
namespace AutumnBox.Basic.Executer
{
    using AutumnBox.Basic.Util;
    using AutumnBox.Support.Log;
    using System;
    /// <summary>
    /// 输出封装类
    /// </summary>
    public class Output : IPrintable
    {
        /// <summary>
        /// 所有的输出
        /// </summary>
        public string[] LineAll
        {
            get
            {
                return All.Split(Environment.NewLine.ToCharArray());
            }
        }

        /// <summary>
        /// 所有的标准输出
        /// </summary>
        public string[] LineOut
        {
            get
            {
                return Out.Split(Environment.NewLine.ToCharArray());
            }
        }

        /// <summary>
        /// 所有的标准错误
        /// </summary>
        public string[] LineError
        {
            get
            {
                return Error.Split(Environment.NewLine.ToCharArray());
            }
        }

        /// <summary>
        /// 所有的输出
        /// </summary>
        public string All { get; protected set; }

        /// <summary>
        /// 所有的标准输出
        /// </summary>
        public string Out { get; protected set; }
        /// <summary>
        /// 所有的标准错误
        /// </summary>
        public string Error { get; protected set; }

        /// <summary>
        /// 判断输出中是否包含某段字符串
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns></returns>
        public bool Contains(string str, bool ignoreCase = true)
        {
            if (ignoreCase)
            {
                return All.ToLower().Contains(str.ToLower());
            }
            else
            {
                return All.Contains(str);
            }
        }

        /// <summary>
        /// 获取完整的输出数据
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return All.ToString();
        }

        /// <summary>
        /// 构建一个空的Output对象
        /// </summary>
        public Output()
        {
            this.Out = "";
            this.Error = "";
            this.All = "";
        }

        /// <summary>
        /// 构建
        /// </summary>
        /// <param name="all">所有内容</param>
        /// <param name="stdOutput">标准输出</param>
        /// <param name="stdError">标准错误</param>
        public Output(string all, string stdOutput, string stdError = "")
        {
            this.Out = stdOutput;
            this.Error = stdError;
            this.All = all;
        }

        /// <summary>
        /// 以可定义的tag或发送者,发送log
        /// </summary>
        /// <param name="tagOrSender"></param>
        /// <param name="printOnRelease"></param>
        public void PrintOnLog(object tagOrSender, bool printOnRelease = false)
        {
            if (printOnRelease)
            {
                Logger.Info(tagOrSender, $"{this.GetType().Name}.PrintOnLog():{Environment.NewLine}{ToString()}");
            }
            else
            {
                Logger.Debug(tagOrSender, $"{this.GetType().Name}.PrintOnLog():{Environment.NewLine}{ToString()}");
            }
        }

        /// <summary>
        /// 以可定义的tag或发送者,打印在控制台
        /// </summary>
        /// <param name="tagOrSender"></param>
        public void PrintOnConsole(object tagOrSender)
        {
            Console.WriteLine($"{tagOrSender} {this.GetType().Name}.PrintOnConsole():{Environment.NewLine}{ToString()}");
        }
    }
}
