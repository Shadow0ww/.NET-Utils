namespace Utils.Common
{
    public class BoolUtils
    {
        /// <summary>
        /// Bool类型True
        /// </summary>
        public static string TRUE = "TRUE";

        /// <summary>
        /// Bool类型False
        /// </summary>
        public static string FALSE = "FALSE";

        /// <summary>
        /// 格式化字符串为布尔类型。支持输出为真的字符串为：
        /// <para>TRUE</para>
        /// <para>YES</para>
        /// <para>Y</para>
        /// <para>1</para>
        /// <para>不区分大小写</para>
        /// </summary>
        /// <param name="arg">需要格式化的字符串</param>
        /// <returns>布尔值</returns>
        public static bool FormatValue(string arg)
        {
            if (string.IsNullOrEmpty(arg))
            {
                return false;
            }

            if (arg.ToUpper().Equals("TRUE") ||
                arg.ToUpper().Equals("YES") ||
                arg.ToUpper().Equals("Y") ||
                arg.ToUpper().Equals("1") ||
                arg.Equals(BoolEnum.True.ToString()))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 格式化数字为布尔类型。
        /// 0 - True
        /// 1 - False
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static bool FormatValue(int arg)
        {
            return FormatValue(arg.ToString("0"));
        }
    }

    /// <summary>
    /// 布尔型的枚举对象。
    /// <para>True - 1</para>
    /// <para>False - 0</para>
    /// </summary>
    public enum BoolEnum
    {
        /// <summary>
        /// 1 - 标识成功
        /// </summary>
        True = 1,

        /// <summary>
        /// 0 - 标识失败
        /// </summary>
        False = 0
    }
}
