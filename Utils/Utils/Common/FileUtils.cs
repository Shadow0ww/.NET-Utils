using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;

namespace Utils.Common
{
    public class FileUtils
    {
        /// <summary>
        /// 文件路径的分割符号。
        /// <para>
        /// 左斜杠 \
        /// </para>
        /// </summary>
        public static string PATH_SEPERATOR = "\\";

        /// <summary>
        /// 回车换行符号
        /// <para>\r\n</para>
        /// </summary>
        public static string NEW_LINE_SPACE = "\r\n";

        /// <summary>
        /// 空格
        /// <para>点 .</para>
        /// </summary>
        public static string FILE_SPACE = " ";

        /// <summary>
        /// 文件的后缀标识 
        /// <para>点 .</para>
        /// </summary>
        public static string FILE_DOT = ".";

        /// <summary>
        /// 读取文件的所有内容数据,获取文本内容
        /// </summary>
        /// <param name="strSourcePath"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string GetFileContent(string strSourcePath, Encoding encode)
        {
            using (var sr = new StreamReader(strSourcePath, encode))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// 读取文件的所有内容数据，默认以UTF-8格式
        /// </summary>
        /// <param name="strSourcePath"></param>
        /// <returns></returns>
        public static string GetFileContent(string strSourcePath)
        {

            return GetFileContent(strSourcePath, Encoding.UTF8);
        }

        /// <summary>
        /// 设置文件权限
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="username"></param>
        public static void SetFileAccount(string filePath, string username)
        {
            try
            {
                var fileInfo = new FileInfo(filePath);

                FileSecurity fileSecurity = fileInfo.GetAccessControl();

                fileSecurity.AddAccessRule(new FileSystemAccessRule(username, FileSystemRights.FullControl, AccessControlType.Allow));     //以完全控制为例

                fileInfo.SetAccessControl(fileSecurity);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("设置文件权限异常：【路径】：{0} - 【用户名】：{1} - 【异常】：{2}", filePath, username, ex));
            }
        }

        /// <summary>
        ///     将目录属性设置为普通
        /// </summary>
        /// <param name="fileTempPath"></param>
        public static void SetFileNomalAttri(string fileTempPath)
        {
            try
            {
                File.SetAttributes(fileTempPath, FileAttributes.Normal);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 设置文件正常
        /// </summary>
        /// <param name="path"></param>
        public static void SetFileNomal(string path)
        {
            try
            {
                SetFileAccount(path, @"Everyone");
                SetFileNomalAttri(path);
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// 判断是否是文件路径。
        /// <para>如果文件不存在，则判定为非文件</para>
        /// </summary>
        /// <param name="strFilePath"></param>
        /// <returns></returns>
        public static bool IsFile(string strFilePath)
        {
            var fileInfo = new FileInfo(strFilePath);
            if ((fileInfo.Attributes & FileAttributes.Directory) != 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="strFilePath"></param>
        /// <returns></returns>
        public static bool IsExist(string strFilePath)
        {
            if (string.IsNullOrEmpty(strFilePath))
            {
                return false;
            }

            if (IsFile(strFilePath))
            {
                return File.Exists(strFilePath);
            }

            return Directory.Exists(strFilePath);
        }

        /// <summary>
        /// 获取文件的大小
        /// </summary>
        /// <param name="strFilePath"></param>
        /// <returns></returns>
        public static long GetFileSize(string strFilePath)
        {
            if (IsExist(strFilePath))
            {
                var fi = new FileInfo(strFilePath);
                return fi.Length;
            }

            throw new FileNotFoundException("文件不存在", strFilePath);
        }

        /// <summary>
        /// 解析PDF文件内容，读成Base64的字符串返回
        /// </summary>
        /// <param name="strFilePath"></param>
        /// <returns></returns>
        public static String ToBase64String(string strFilePath)
        {
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(strFilePath, FileMode.Open, FileAccess.Read);
                var buf = new byte[fileStream.Length];
                fileStream.Read(buf, 0, buf.Length);

                if (buf.Length != 0)
                {
                    return Convert.ToBase64String(buf);
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                throw new Exception("文件转换Base64字符编码异常" + strFilePath, ex);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        /// <summary>
        /// 将Base64内容解密后写入到指定目录结构的文件中。
        /// <para>如果文件夹不存在，则创建</para>
        /// <para>如果文件已经存在，则覆盖</para>
        /// </summary>
        /// <param name="strBase64Text"></param>
        /// <param name="strFilePath"></param>
        /// <returns></returns>
        public static bool SaveBase64(string strBase64Text, string strFilePath)
        {
            FileStream sFile = null;

            try
            {
                string strFolder = GetFileDirPath(strFilePath);
                CreateFolder(strFolder);

                byte[] bytes = Convert.FromBase64String(strBase64Text);
                sFile = File.Create(strFilePath);
                sFile.Write(bytes, 0, bytes.Length);
                sFile.Flush();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Base64文件保存异常", ex);
            }
            finally
            {
                if (null != sFile)
                {
                    sFile.Close();
                }
            }
        }

        /// <summary>
        /// 获取文件路径的文件夹地址。
        /// <para>IN:  D:\User\JZY\Books\EBook.pdf</para>
        /// <para>OUT: D:\User\JZY\Books</para>
        /// </summary>
        /// <returns></returns>
        public static string GetFileDirPath(string strFilePath)
        {
            if (string.IsNullOrEmpty(strFilePath))
            {
                return string.Empty;
            }

            return Path.GetDirectoryName(strFilePath);
        }

        /// <summary>
        /// 获取文件的名称，带有后缀
        /// <para>IN : D:\User\JZY\File.xml</para>
        /// <para>OUT: File.xml</para>
        /// </summary>
        /// <param name="strFilePath"></param>
        /// <returns></returns>
        public static string GetFileName(string strFilePath)
        {
            if (string.IsNullOrEmpty(strFilePath))
            {
                return string.Empty;
            }

            return Path.GetFileName(strFilePath);
        }

        /// <summary>
        /// 获取文件夹路径的跟盘符位置。
        /// <para>IN : D:\USER\JZY\DIR\FILE.PDF</para>
        /// <para>OUT: D:\</para>
        /// </summary>
        /// <returns></returns>
        public static string GetFilePathRootDir(string strFilePath)
        {
            try
            {
                if (string.IsNullOrEmpty(strFilePath))
                {
                    return string.Empty;
                }

                strFilePath = FormatFilePath(strFilePath);
                int nIndex = strFilePath.IndexOf(PATH_SEPERATOR);

                if (nIndex > 0)
                {
                    return strFilePath.Substring(0, nIndex + 1);
                }

                return strFilePath;
            }
            catch (Exception ex)
            {
                throw new Exception("获取路径的根盘符地址异常", ex);
            }
        }

        /// <summary>
        /// 获取操作系统的根目录地址
        /// <para>
        /// 获取桌面所在的根目录地址。
        /// 默认: C:\
        /// </para>
        /// </summary>
        /// <returns></returns>
        public static string GetOsRootDir()
        {
            string strDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            return GetFilePathRootDir(strDesktopPath);
        }

        /// <summary>
        /// 获取文件的名称,不带有后缀
        /// <para>IN : D:\User\JZY\File.xml</para>
        /// <para>OUT: File</para>
        /// </summary>
        /// <param name="strFilePath"></param>
        /// <returns></returns>
        public static string GetFileNameWithoutExt(string strFilePath)
        {
            if (string.IsNullOrEmpty(strFilePath))
            {
                return string.Empty;
            }

            return Path.GetFileNameWithoutExtension(strFilePath);
        }

        /// <summary>
        /// 创建文件夹。
        /// </summary>
        /// <param name="strFolderPath">文件夹路径</param>
        /// <returns></returns>
        public static void CreateFolder(string strFolderPath)
        {
            try
            {
                if (!IsExist(strFolderPath))
                {
                    Directory.CreateDirectory(strFolderPath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("创建文件夹异常", ex);
            }
        }

        /// <summary>
        /// 删除指定的文件/或者文件夹。
        /// <para>文件：直接删除</para>
        /// <para>文件夹：递归删除整个文件夹</para>
        /// </summary>
        /// <param name="strPath"></param>
        public static bool Delete(string strPath)
        {
            try
            {
                if (string.IsNullOrEmpty(strPath))
                {
                    return false;
                }

                if (!IsExist(strPath))
                {
                    return true;
                }

                if (IsFile(strPath))
                {
                    File.Delete(strPath);
                }
                else
                {
                    Directory.Delete(strPath, true);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("删除文件异常。文档路径:{0}", strPath), ex);
            }
        }

        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="strSrcFile">源文件名称</param>
        /// <param name="strNewFile">目标文件名称</param>
        /// <returns></returns>
        public static bool CopyFile(string strSrcFile, string strNewFile)
        {
            try
            {
                if (!IsExist(strSrcFile))
                {
                    return false;
                }

                //如果文件夹不存在，则先执行创建操作
                CreateFolder(GetFileDirPath(strNewFile));

                File.Copy(strSrcFile, strNewFile, true);
                return true;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.Append("拷贝文件失败。\r\n");
                sb.Append("SrcFile = " + strSrcFile + NEW_LINE_SPACE);
                sb.Append("NewFile = " + strNewFile + NEW_LINE_SPACE);

                throw new Exception(sb.ToString(), ex);
            }

        }

        /// <summary>
        /// 拷贝文件夹内容到新的目录下
        /// </summary>
        /// <param name="strSrcFolder"></param>
        /// <param name="strDestFolder"></param>
        public static void CopyFolder(string strSrcFolder, string strDestFolder)
        {
            try
            {
                if (!FileUtils.IsExist(strSrcFolder))
                {
                    throw new Exception("源文件夹不存在，拷贝终止。SrcFolder=" + strSrcFolder);
                }

                //如果文件夹不存在，则先完成文件夹创建
                CreateFolder(strDestFolder);

                var sDir = new DirectoryInfo(strSrcFolder);
                FileInfo[] fileArray = sDir.GetFiles();
                foreach (FileInfo file in fileArray)
                {
                    file.CopyTo(strDestFolder + PATH_SEPERATOR + file.Name, true);
                }

                // 循环子文件夹
                DirectoryInfo[] subDirArray = sDir.GetDirectories();
                foreach (DirectoryInfo subDir in subDirArray)
                {
                    CopyFolder(subDir.FullName, strDestFolder + PATH_SEPERATOR + subDir.Name);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("拷贝文件夹内容异常", ex);
            }
        }

        /// <summary>
        /// 判断路径是否为绝对路径
        /// </summary>
        /// <param name="strFilePath"></param>
        /// <returns></returns>
        public static bool IsAbsolutePath(string strFilePath)
        {
            if (strFilePath.StartsWith(PATH_SEPERATOR) || strFilePath.StartsWith("/"))
            {
                return false;
            }

            return Path.IsPathRooted(strFilePath);
        }

        /// <summary>
        /// 格式化以\\开头的网络盘符绝对路径。
        /// <para>
        ///     IN:   \\10.70.150.150\\User/JZY\\\DIR\\JAVA.PDF
        /// </para>
        /// <para>
        ///     OUT:  \\10.70.150.150\User\JZY\DIR\JAVA.PDF
        /// </para>
        /// </summary>
        /// <param name="strWebDirPath"></param>
        /// <returns></returns>
        public static string FormatWebPath(string strWebDirPath)
        {
            string strSubString = strWebDirPath.Substring(2);
            strSubString = Regex.Replace(strSubString, "[/\\\\]+", PATH_SEPERATOR);

            if (strSubString.StartsWith(PATH_SEPERATOR))
            {
                strSubString = strSubString.Substring(1);
            }

            return PATH_SEPERATOR + PATH_SEPERATOR + strSubString;
        }

        /// <summary>
        /// 获取EXE主程序运行的根目录地址。
        /// <para>D:\Program files\Root\ActiveDir\</para>
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 使用此方法替代Environment.CurrentDirectory方法。
        /// </remarks>
        public static string GetExeRootPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory + PATH_SEPERATOR;
        }

        /// <summary>
        /// 格式化输入文件路径，得到文件的本地绝对路径。使用示例如下：
        /// 
        /// <para>1: IN :  MyDir/Books/EBook.pdf</para>
        /// <para>   OUT:  D:\User\MyDir\Books\EBook.pdf</para>
        /// 
        /// <para>2: IN :  \\MyDir////Books\EBook.pdf  </para>
        /// <para>   OUT:  D:\User\MyDir\Books\EBook.pdf</para>
        /// 
        /// <para>3: IN :  D:\User///MyDir//Books\\EBook.pdf</para>
        /// <para>   OUT:  D:\User\MyDir\Books\EBook.pdf</para>
        /// 
        /// <para>4: IN :  D:\User///MyDir//Books</para>
        /// <para>   OUT:  D:\User\MyDir\Books</para>
        /// <para>
        ///     注意：在使用ActiveX控件的时候，不使用此方法格式化路径。
        ///     格式化ActiveX控件路径，请使用方法FormatActiveXPath
        /// </para>
        /// </summary>
        /// <param name="strFilePath"></param>
        /// <returns></returns>
        /// <seealso cref="FormatActiveXPath"/>
        public static string FormatFilePath(string strFilePath)
        {
            try
            {
                if (string.IsNullOrEmpty(strFilePath))
                {
                    return string.Empty;
                }

                //如果是以\\开头的网络盘符，则格式化字符串后退出返回
                if (strFilePath.StartsWith(PATH_SEPERATOR + PATH_SEPERATOR))
                {
                    return FormatWebPath(strFilePath);
                }

                //对于非绝对路径，则添加当前的盘符字符串
                if (!IsAbsolutePath(strFilePath))
                {
                    strFilePath = GetExeRootPath() + strFilePath;
                }

                //使用正则表达式格式化路径字符串
                return Regex.Replace(strFilePath, "[/\\\\]+", PATH_SEPERATOR);
            }
            catch (Exception ex)
            {
                throw new Exception("格式化文件路径异常。FilePath=" + strFilePath, ex);
            }
        }

        private static List<FileBean> returnFilesPath;
        /// <summary>
        /// 遍历文件夹
        /// 
        /// </summary>
        /// <param name="theFolder"></param>
        private static List<FileBean> GetFilesPath(DirectoryInfo theFolder)
        {
            try
            {

                foreach (FileInfo nextFile in theFolder.GetFiles())
                {
                    var filePath = new FileBean();
                    filePath.FileName = nextFile.Name;
                    filePath.FileDirPath = nextFile.DirectoryName + "/" + nextFile.Name;
                    returnFilesPath.Add(filePath);
                }
                if (theFolder.GetDirectories().Length != 0)
                {
                    foreach (DirectoryInfo nextFolder in theFolder.GetDirectories())
                    {
                        GetFilesPath(nextFolder);
                    }
                }
                return returnFilesPath;
            }
            catch (Exception ex)
            {

                throw new Exception("遍历文件夹异常", ex);
            }
        }

        /// <summary>
        /// 获取文件夹中的所有文件的文件路径
        /// </summary>
        /// <param name="filesPath"> 文件夹目录</param>
        /// <returns></returns>
        public static List<FileBean> GetFilesList(string filesPath)
        {
            try
            {

                var theFolder = new DirectoryInfo(filesPath);
                returnFilesPath = new List<FileBean>();
                return GetFilesPath(theFolder);
            }
            catch (Exception ex)
            {
                throw new Exception("获取文件夹中的所有文件的文件路径异常！", ex);
            }
        }

        /// <summary>  
        /// 压缩单个文件。使用SharpZipLib将文件进行压缩。
        /// <para>strSrcFile：待压缩文件路径</para>  
        /// <para>strDestFile:压缩后文件路径</para>
        /// </summary>  
        /// <param name="strSrcFile">待压缩文件路径</param>  
        /// <param name="strDestFile">压缩后文件路径</param>  
        /// <remarks>
        /// 2012-08-27 同时增加对字符串的压缩和解压缩方法
        /// </remarks>
        public static void ZipFile(string strSrcFile, string strDestFile)
        {
            try
            {
                using (FileStream fs = File.OpenRead(strSrcFile))
                {
                    var buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);

                    using (FileStream ZipFile = File.Create(strDestFile))
                    {
                        using (var ZipStream = new ZipOutputStream(ZipFile))
                        {
                            var entry = new ZipEntry(GetFileName(strSrcFile));
                            ZipStream.PutNextEntry(entry);
                            ZipStream.SetLevel(5);

                            ZipStream.Write(buffer, 0, buffer.Length);
                            ZipStream.Finish();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("压缩文件异常", ex);
            }
        }

        /// <summary>
        /// 将ZIP文件解压缩到指定位置文件夹中。
        /// <para>strZipFile:待解压缩源文件</para>
        /// <para>strDestDir:解压缩后的文件夹路径</para>
        /// </summary>
        /// <param name="strZipFile">待解压缩的ZIP文件</param>
        /// <param name="strDestDir">解压缩后的文件夹路径</param>
        /// <remarks>
        /// 2012-08-27 同时增加对字符串的压缩和解压缩方法
        /// 2012.12.17 经测试 不支持包含子文件夹的压缩文件包
        /// </remarks>
        public static void UnZipFile(string strZipFile, string strDestDir)
        {
            try
            {
                FileUtils.CreateFolder(strDestDir);
                using (var inputStream = new FileStream(strZipFile, FileMode.Open, FileAccess.Read))
                {
                    using (var zipInStream = new ZipInputStream(inputStream))
                    {
                        ZipEntry entry = zipInStream.GetNextEntry();
                        //sunxx添加循环 遍历解压压缩文件夹下的文件
                        while (entry != null)
                        {
                            string strFilePath = FormatFilePath(strDestDir + "/") + entry.Name;
                            using (var outStream = new FileStream(strFilePath, FileMode.Create, FileAccess.Write))
                            {
                                int size;
                                var buffer = new byte[1024];
                                do
                                {
                                    size = zipInStream.Read(buffer, 0, buffer.Length);
                                    outStream.Write(buffer, 0, size);
                                }
                                while (size > 0);
                            }
                            entry = zipInStream.GetNextEntry();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("解压缩文件异常", ex);
            }
        }

        /// <summary>
        /// 将文本内容写入到指定目录结构的文件中。(默认使用UTF8编码，覆盖文件)
        /// <para>如果文件夹不存在，则创建</para>
        /// <para>如果文件已经存在，则覆盖</para>
        /// </summary>
        /// <param name="strText"></param>
        /// <param name="strFilePath"></param>
        /// <returns></returns>
        public static bool Save(string strText, string strFilePath)
        {
            return Save(strText, strFilePath, false);
        }

        /// <summary>
        /// 将文本内容写入到指定目录结构的文件中。(默认使用UTF8编码)
        /// <para>如果文件夹不存在，则创建</para>
        /// </summary>
        /// <param name="strText"></param>
        /// <param name="strFilePath"></param>
        /// <param name="isAppend">是否覆盖</param>
        /// <returns></returns>
        public static bool Save(string strText, string strFilePath, bool isAppend)
        {
            return Save(strText, strFilePath, Encoding.UTF8, isAppend);
        }

        /// <summary>
        /// 将文本内容写入到指定目录结构的文件中。
        /// </summary>
        /// <param name="strText"></param>
        /// <param name="strFilePath"></param>
        /// <param name="encode"></param>
        /// <param name="isAppend"></param>
        /// <returns></returns>
        public static bool Save(string strText, string strFilePath, Encoding encode, bool isAppend)
        {
            try
            {
                string strFolder = GetFileDirPath(strFilePath);
                CreateFolder(strFolder);

                using (var sw = new StreamWriter(strFilePath, isAppend, encode))
                {
                    sw.Write(strText);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("保存文件异常，FilePath=" + strFilePath, ex);
            }
        }


    }

    /// <summary>
    /// 文件Bean
    /// </summary>
    public class FileBean
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { set; get; }
        /// <summary>
        /// 文件绝对路径
        /// </summary>
        public string FileDirPath { set; get; }
        /// <summary>
        /// 文件相对路径
        /// </summary>
        public string FileParmPath { set; get; }

    }

}
