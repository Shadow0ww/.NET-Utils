using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Demo
{
    /// <summary>
    /// ==========================工厂模式（Factory Pattern）====================================
    /// 介绍
    ///     ○意图：定义一个创建对象的接口，让其子类自己决定实例化哪一个工厂类，工厂模式使其创建过程延迟到子类进行。
    ///     ○主要解决：主要解决接口选择的问题。
    ///     ○何时使用：我们明确地计划不同条件下创建不同实例时。
    ///     ○如何解决：让其子类实现工厂接口，返回的也是一个抽象的产品。
    ///     ○关键代码：创建过程在其子类执行。
    ///     ○应用实例： 
    ///         1、您需要一辆汽车，可以直接从工厂里面提货，而不用去管这辆汽车是怎么做出来的，以及这个汽车里面的具体实现。 
    ///         2、Hibernate 换数据库只需换方言和驱动就可以。
    ///     ○优点： 
    ///         1、一个调用者想创建一个对象，只要知道其名称就可以了。 
    ///         2、扩展性高，如果想增加一个产品，只要扩展一个工厂类就可以。 
    ///         3、屏蔽产品的具体实现，调用者只关心产品的接口。
    ///     ○缺点：
    ///         每次增加一个产品时，都需要增加一个具体类和对象实现工厂，使得系统中类的个数成倍增加，在一定程度上增加了系统的复杂度，同时也增加了系统具体类的依赖。这并不是什么好事。
    ///     ○使用场景： 
    ///         1、日志记录器：记录可能记录到本地硬盘、系统事件、远程服务器等，用户可以选择记录日志到什么地方。 
    ///         2、数据库访问，当用户不知道最后系统采用哪一类数据库，以及数据库可能有变化时。 
    ///         3、设计一个连接服务器的框架，需要三个协议，"POP3"、"IMAP"、"HTTP"，可以把这三个作为产品类，共同实现一个接口。
    ///     ○注意事项：
    ///         作为一种创建类模式，在任何需要生成复杂对象的地方，都可以使用工厂方法模式。
    ///         有一点需要注意的地方就是复杂对象适合使用工厂模式，而简单对象，特别是只需要通过 new 就可以完成创建的对象，无需使用工厂模式。
    ///         如果使用工厂模式，就需要引入一个工厂类，会增加系统的复杂度。
    /// </summary>
    class Factory_Pattern
    {

        class Program
        {
            static void Main(string[] args)
            {
                //调用工厂类，生产不同的形状
                shape shape1 = ShapeFactory.getShape(ShapeType.Circle);
                shape1.draw();
                shape shape2 = ShapeFactory.getShape(ShapeType.Rectangle);
                shape2.draw();
                shape shape3 = ShapeFactory.getShape(ShapeType.Square);
                shape3.draw();
                shape shape4 = ShapeFactory.getShape(ShapeType.Triangle);
                shape4.draw();
                Console.ReadKey();
            }
        }
        /// <summary>
        /// 形状接口
        /// </summary>
        public interface shape
        {
            void draw();
        }

        /// <summary>
        /// 形状类
        /// </summary>
        public class Rectangle : shape
        {
            public void draw()
            {
                Console.WriteLine("矩形实现形状的接口方法draw()。");
            }
        }
        public class Square : shape
        {
            public void draw()
            {
                Console.WriteLine("正方形实现形状的接口方法draw()。");
            }
        }
        public class Circle : shape
        {
            public void draw()
            {
                Console.WriteLine("圆形实现形状的接口方法draw()。");

            }
        }
        public class Triangle : shape
        {
            public void draw()
            {
                Console.WriteLine("三角形实现形状的接口方法draw()。");

            }
        }

        /// <summary>
        /// 枚举
        /// </summary>
        public enum ShapeType
        {
            Rectangle,
            Square,
            Circle,
            Triangle

        }

        /// <summary>
        /// 工厂
        /// </summary>
        public class ShapeFactory
        {
            public static shape getShape(ShapeType shapetype)
            {
                if (shapetype == ShapeType.Circle)
                {
                    return new Circle();
                }
                else if (shapetype == ShapeType.Rectangle)
                {
                    return new Rectangle();
                }
                else if (shapetype == ShapeType.Triangle)
                {
                    return new Triangle();
                }
                else
                {
                    return new Square();
                }

            }
        }
    }
}
