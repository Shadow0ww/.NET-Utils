using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Utils.Demo
{
    //关键字   返回类型 委托类型  签名
    //delegate void     MyDel(    int x  );
    //讲解：
    //【概念】委托表示的是一个或多个方法的集合。你可以把delegate看作一个包含有序方法列表的对象，这些方法具有相同的签名和返回类型
    //        方法列表称为[调用列表]
    //【匹配】委托保存方法可以来自任何类或者结构，只要匹配两点：1.返回类型 2.委托签名(包括ref和out修饰符)
    //【注意】调用列表中的方法，可以是实例方法也可以是静态方法
    //        调用委托时候，会执行调用列表中所有方法

    public class DelegateDemo1
    {
        //委托调用的本地方法
        public void Print1(string value, RichTextBox rt)
        {
            rt.AppendText(string.Format("方法1：{0}", value) + Common.FileUtils.NEW_LINE_SPACE);
        }
        public void Print2(string value, RichTextBox rt)
        {
            rt.AppendText(string.Format("方法2：{0}", value) + Common.FileUtils.NEW_LINE_SPACE);
        }
        public void Print3(string value, RichTextBox rt)
        {
            rt.AppendText(string.Format("方法3：{0}", value) + Common.FileUtils.NEW_LINE_SPACE);
        }

    }
}
