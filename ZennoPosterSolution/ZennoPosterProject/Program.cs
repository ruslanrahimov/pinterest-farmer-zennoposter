using Global.ZennoExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Linq;
using System.Net.Sockets;
using System.Resources;
using System.Text;
using ZennoLab.CommandCenter;
using ZennoLab.Emulation;
using ZennoLab.InterfacesLibrary.ProjectModel;
using ZennoLab.InterfacesLibrary.ProjectModel.Enums;
using System.Threading;

namespace ZennoPosterProject
{
    /// <summary>
    /// Класс для запуска выполнения скрипта
    /// </summary>
    public class Program : IZennoExternalCode
    {
        /// <summary>
        /// Метод для запуска выполнения скрипта
        /// </summary>
        /// <param name="instance">Объект инстанса выделеный для данного скрипта</param>
        /// <param name="project">Объект проекта выделеный для данного скрипта</param>
        /// <returns>Код выполнения скрипта</returns>		
        public int Execute(Instance instance, IZennoPosterProjectModel project)
        {
            string accountType = project.Variables["accountType"].Value;
            string zpprofileName = project.Variables["zpprofileName"].Value;
            string[] variablesToSave = project.Lists["DefaultVariablesToSave"].ToArray();
            string tempFilePath = $@"{project.Directory}\src\zpprofiles-temp\{zpprofileName}";

            if (File.Exists(tempFilePath))
            {
                project.Profile.Save($@"{project.Directory}\profiles\ban-{accountType}\{zpprofileName}", false, true, true, true, true, true, true, true, true, variablesToSave);
                File.Delete(tempFilePath);
            }

            return 0;
        }
    }
}