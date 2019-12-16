using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using System.Text;

namespace Utils.Common
{
    public class StringUtils
    {
        /// <summary>
        /// 比较两个字符串 是否相等 （不区分大小写）
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static bool Compare(string str1, string str2)
        {
            try
            {
                if (string.Compare(str1, str2, true) == 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 将对象转换为字符串类型对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Parse(Object obj)
        {
            return obj.ToString();
        }

        /// <summary>
        /// 将Float类型对象转换成字符串，是否变成整型
        /// </summary>
        /// <param name="fNumber">float对象</param>
        /// <param name="isToInteger">是否变整型</param>
        /// <returns></returns>
        public static string Parse(float fNumber, bool isToInteger)
        {
            if (isToInteger)
            {
                var nNumber = (int)fNumber;
                return nNumber.ToString("");
            }

            return fNumber.ToString("");
        }

        /// <summary>
        /// 将Float类型对象转换成字符串
        /// </summary>
        /// <param name="fNumber">float对象</param>
        /// <returns></returns>
        public static string Parse(float fNumber)
        {
            return Parse(fNumber, false);
        }

        /// <summary>
        /// 根据字符编码，将字节数组转换成字符串
        /// </summary>
        /// <param name="encode"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string GetString(Encoding encode, byte[] bytes)
        {
            return encode.GetString(bytes);
        }

        /// <summary>
        /// 将字节数组转换成字符串
        /// <para>默认采用UTF8编码方式</para>
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string GetString(byte[] bytes)
        {
            return GetString(Encoding.UTF8, bytes);
        }

        /// <summary>
        /// 将流对象内容转换成字符串
        /// </summary>
        /// <param name="encode"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string GetString(Encoding encode, MemoryStream stream)
        {
            return encode.GetString(stream.ToArray());
        }

        /// <summary>
        /// 将流对象内容转换成字符串
        /// <para>默认采用UTF8编码方式</para>
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string GetString(MemoryStream stream)
        {
            return GetString(Encoding.UTF8, stream);
        }
        /// <summary>
        /// 将Base64string转成string
        /// </summary>
        /// <param name="msgBase64"></param>
        /// <returns></returns>
        public static string ParseBase64(string msgBase64)
        {
            return string.IsNullOrEmpty(msgBase64) ? string.Empty : GetString(Convert.FromBase64String(msgBase64));
        }

        /// <summary>
        /// 将string转成Base64string
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string ToBase64(string msg)
        {
            return string.IsNullOrEmpty(msg) ? string.Empty : Convert.ToBase64String(ToBytes(msg));
        }

        /// <summary>
        /// 将字符串转换成字符数组。
        /// <para>默认采用UTF8编码</para>
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static byte[] ToBytes(string arg)
        {
            return ToBytes(Encoding.UTF8, arg);
        }

        /// <summary>
        /// 根据编码方式，将字符串转换成字节数组
        /// </summary>
        /// <param name="encode"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static byte[] ToBytes(Encoding encode, string arg)
        {
            return encode.GetBytes(arg);
        }

        /// <summary>
        /// 将字符串转换成Stream流对象
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static MemoryStream ToMemoryStream(string arg)
        {
            return ToMemoryStream(ToBytes(arg));
        }

        /// <summary>
        /// 将字节数组转换成Stream流对象
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static MemoryStream ToMemoryStream(byte[] bytes)
        {
            var stream = new MemoryStream();
            stream.Write(bytes, 0, bytes.Length);

            return stream;
        }

        /// <summary>
        /// 压缩字符串。使用SharpZipLib将字符串压缩成字节数组。
        /// 压缩比一般可以达到30%以上
        /// <para>输入：字符串</para>
        /// <para>输出：压缩后的字节数组</para>
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static byte[] ZipString(string arg)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(arg);
                using (var stream = new MemoryStream())
                {
                    using (var ZipStream = new ZipOutputStream(stream))
                    {
                        ZipStream.PutNextEntry(new ZipEntry("0"));
                        //ZipStream.SetLevel(5);

                        ZipStream.Write(buffer, 0, buffer.Length);
                        ZipStream.Finish();

                        return stream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("压缩字符串异常", ex);
            }
        }

        /// <summary>
        /// 解压字符串。使用SharpZipLib将字节数组解压缩成字符串。
        /// <para>输入：压缩后的字节流数组</para>
        /// <para>输出：解压缩的字符串</para>
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string UnZipString(byte[] buffer)
        {
            try
            {
                using (var stream = new MemoryStream(buffer))
                {
                    using (var inputStream = new ZipInputStream(stream))
                    {
                        inputStream.GetNextEntry();
                        using (var outStream = new MemoryStream())
                        {
                            int size;
                            var bytes = new byte[1024];
                            do
                            {
                                size = inputStream.Read(bytes, 0, bytes.Length);
                                outStream.Write(bytes, 0, size);
                            }
                            while (size > 0);

                            return GetString(outStream);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("解压缩字节数组异常", ex);
            }
        }

        /// <summary>
        /// 将int值转化成16进制
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToStringHex(int Value)
        {
            return Value.ToString("X");
        }







    }
}
