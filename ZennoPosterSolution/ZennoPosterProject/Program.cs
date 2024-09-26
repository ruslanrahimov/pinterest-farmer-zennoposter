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
            instance.ClearCache();

            string threadid = project.Variables["threadid"].Value;

            lock (SyncObject)
            {
                //Получаем путь к профиль файлу

                string workDirPath = project.Variables["workDirPath"].Value;

                List<string> zpprofilesTemp = new List<string>();
                string zpprofile = String.Empty;
                string[] zpprofilesArray = Directory.GetFiles(workDirPath, "*.zpprofile", SearchOption.TopDirectoryOnly).OrderBy(d => new FileInfo(d).LastWriteTime).ToArray();

                if (zpprofilesArray.Length == 0)
                {
                    throw new Exception($@"Папка c аккаунтами пуста {workDirPath}");
                }

                zpprofilesTemp.AddRange(zpprofilesArray);
                zpprofile = zpprofilesTemp[0];
                FileInfo fileInfo = new FileInfo(zpprofile);

                //Сохраняем переменные которые есть в Профиле для дальнейшего пересохранения
                string zpproileToExractDirName = fileInfo.Name.Replace(".zpprofile", "");

                string extractPath = $@"{project.Directory}\extract\{zpproileToExractDirName}"; // Путь для извлечения

                if (Directory.Exists(extractPath)) // Проверяем, существует ли директория
                {
                    Directory.Delete(extractPath, true); // Если существует - удаляем, иначе не даст разархивировать
                }

                ZipFile.ExtractToDirectory(zpprofile, extractPath); // Извлекаем содержимое архива

                string variablesXML = File.ReadAllText(extractPath + @"\variables.zpdata"); // Читаем переменные из variables.zpdata

                project.Xml.FromString(variablesXML);

                // Проходимся по всем сохраненным в профиле переменным и сохранем их в список
                for (int i = 0; i < project.Xml.SaveVariablesData.Variables.VariablesData.Count; i++)
                {
                    string variableName = project.Xml.SaveVariablesData.Variables.VariablesData[i].VariableName.Value;
                    project.Lists["DefaultVariablesToSave"].Add(variableName);

                }

                Directory.Delete(extractPath, true); //Удаляем папку с извлеченными данными

                //Загружаем профиль
                project.SendInfoToLog($"{threadid}: Беру в работу профиль - {fileInfo.Name}", true);
                project.Profile.Load(zpprofile, true);

                //Перемещаем файл в папку temp
                string zpprofilesTempFolderPath = $@"{project.Directory}\src\zpprofiles-temp";

                if (!Directory.Exists(zpprofilesTempFolderPath)) //Проверяем имеется ли папка zpprofiles_temp
                {
                    Directory.CreateDirectory(zpprofilesTempFolderPath); // Создаем если нет
                }

                if (File.Exists($@"{zpprofilesTempFolderPath}\{fileInfo.Name}")) //Проверяем нет ли файла в папке перед перемещением
                {
                    File.Delete($@"{zpprofilesTempFolderPath}\{fileInfo.Name}");
                }

                File.Move(zpprofile, $@"{zpprofilesTempFolderPath}\{fileInfo.Name}");

                project.Variables["zpprofileName"].Value = fileInfo.Name;

            }

            return 0;
        }
    }
}