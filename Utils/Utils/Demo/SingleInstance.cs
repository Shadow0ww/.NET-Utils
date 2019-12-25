using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Demo
{
    //============================单例模式=================================
    /*
    介绍#
    单例模式是软件工程中最着名的模式之一。从本质上讲，单例是一个只允许创建自身的单个实例的类，并且通常可以简单地访问该实例。
    最常见的是，单例不允许在创建实例时指定任何参数——否则对实例的第二个请求但具有不同的参数可能会有问题！
    （如果对于具有相同参数的所有请求都应访问相同的实例，则工厂模式更合适。）本文仅处理不需要参数的情况。
    通常，单例的要求是它们是懒惰地创建的——即直到第一次需要时才创建实例。
    
    在C＃中实现单例模式有各种不同的方法。我将以优雅的相反顺序呈现它们，从最常见的、不是线程安全的版本开始，一直到完全延迟加载的、线程安全的、简单且高性能的版本。

    然而，所有这些实现都有四个共同特征：

        ·单个构造函数，它是私有且无参数的。这可以防止其他类实例化它（这将违反模式）。
            请注意，它还可以防止子类化——如果一个单例对象可以被子类化一次，那么它就可以被子类化两次，如果每个子类都可以创建一个实例，则违反了该模式。
            如果您需要基类型的单个实例，则可以使用工厂模式，但是确切的类型要到运行时才能知道。
        ·类是密封的。严格来说，由于上述原因，这是不必要的，但是可以帮助JIT进行更多的优化。
        ·一个静态变量，用于保存对单个已创建实例的引用（如果有的话）。
        ·公共静态意味着获取对单个已创建实例的引用，必要时创建一个实例。

    请注意，所有这些实现还使用公共静态属性Instance 作为访问实例的方法。在所有情况下，可以轻松地将属性转换为方法，而不会影响线程安全性或性能。
    */

    /// <summary>
    /// 第一个版本 ——不是线程安全的
    /// 
    /// 如前所述，上述内容不是线程安全的。两个不同的线程都可以评估测试if (instance==null)并发现它为true，然后两个都创建实例，这违反了单例模式。
    /// 请注意，实际上，在计算表达式之前可能已经创建了实例，但是内存模型不保证其他线程可以看到实例的新值，除非已经传递了合适的内存屏障（互斥锁）。
    /// </summary>
    public sealed class Singleton
    {
        //糟糕的代码！不要使用！
        private static Singleton instance = null;

        private Singleton()
        {
        }

        public static Singleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Singleton();
                }
                return instance;
            }
        }
    }



    /// <summary>
    /// 第二个版本 —— 简单的线程安全
    /// 
    /// 此实现是线程安全的。线程取消对共享对象的锁定，然后在创建实例之前检查是否已创建实例。这
    ///     会解决内存屏障问题（因为锁定确保在获取锁之后逻辑上发生所有读取，并且解锁确保在锁定释放之前逻辑上发生所有写入）并确保只有一个线程将创建实例
    ///     （仅限于一次只能有一个线程可以在代码的那一部分中——当第二个线程进入它时，第一个线程将创建实例，因此表达式将计算为false）。
    ///     不幸的是，每次请求实例时都会获得锁定，因此性能会受到影响。
    /// 请注意，不是typeof(Singleton)像这个实现的某些版本那样锁定，而是锁定一个私有的静态变量的值。
    ///     锁定其他类可以访问和锁定的对象（例如类型）会导致性能问题甚至死锁。这是我通常的风格偏好——只要有可能，只锁定专门为锁定目的而创建的对象，或者为了特定目的（例如，等待/触发队列）而锁定的文档。
    ///     通常这些对象应该是它们所使用的类的私有对象。这有助于使编写线程安全的应用程序变得更加容易。
    /// </summary>
    public sealed class Singleton2
    {
        private static Singleton2 instance = null;
        private static readonly object padlock = new object();

        Singleton2()
        {
        }
        public static Singleton2 Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Singleton2();
                    }
                    return instance;
                }
            }
        }
    }



    /// <summary>
    /// 第三个版本 - 使用双重检查锁定尝试线程安全
    /// 
    /// 该实现尝试是线程安全的，而不必每次都取出锁。不幸的是，该模式有四个缺点：
    ///     ·它在Java中不起作用。这似乎是一个奇怪的事情，但是如果您需要Java中的单例模式，这是值得知道的，C＃程序员也可能是Java程序员。
    ///         Java内存模型无法确保构造函数在将新对象的引用分配给Instance之前完成。Java内存模型经历了1.5版本的重新改进，但是在没有volatile变量（如在C＃中）的情况下，双重检查锁定仍然会被破坏。
    ///     ·在没有任何内存障碍的情况下，ECMA CLI规范也打破了这一限制。
    ///         有可能在.NET 2.0内存模型（比ECMA规范更强）下它是安全的，但我宁愿不依赖那些更强大的语义，特别是如果对安全性有任何疑问的话。
    ///         使instance变量volatile变得有效，就像明确的内存屏障调用一样，尽管在后一种情况下，甚至专家也无法准确地就需要哪些屏障达成一致。
    ///         我尽量避免专家对对错意见也不一致的情况！
    ///     ·这很容易出错。该模式需要完全如上所述——任何重大变化都可能影响性能或正确性。
    ///     ·它的性能仍然不如后续的实现。
    /// </summary>
    public sealed class Singleton3
    {
        // 糟糕的代码！不要使用！
        private static Singleton3 instance = null;
        private static readonly object padlock = new object();
        Singleton3()
        {
        }

        public static Singleton3 Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new Singleton3();
                        }
                    }
                }
                return instance;
            }
        }
    }



    /// <summary>
    /// 第四个版本 - 不太懒，不使用锁且线程安全
    /// 
    /// 正如您所看到的，这实际上非常简单——但是为什么它是线程安全的，它有多懒惰？C＃中的静态构造函数仅在创建类的实例或引用静态成员时执行，并且每个AppDomain只执行一次。考虑到无论发生什么情况，都需要执行对新构造的类型的检查，这比在前面的示例中添加额外检查要快。然而，还有一些小缺陷：
    ///     ·它并不像其他实现那样懒惰。特别是，如果您有Instance之外的静态成员，那么对这些成员的第一次引用将涉及到创建实例。这将在下一个实现中得到纠正。
    ///     ·如果一个静态构造函数调用另一个静态构造函数，而另一个静态构造函数再次调用第一个构造函数，则会出现复杂情况。
    ///         查看.NET规范（目前是分区II的第9.5.3节），了解有关类型初始化器的确切性质的更多详细信息——它们不太可能会影响您，但是有必要了解静态构造函数在循环中相互引用的后果。
    ///     ·类型初始化器的懒惰性只有在.NET没有使用名为BeforeFieldInit的特殊标志标记类型时才能得到保证。
    ///         不幸的是，C＃编译器（至少在.NET 1.1运行时中提供）将所有没有静态构造函数的类型（即看起来像构造函数但被标记为静态的块）标记为BeforeFieldInit。
    ///         我现在有一篇文章，详细介绍了这个问题。另请注意，它会影响性能，如在页面底部所述的那样。
    /// 您可以使用此实现（并且只有这一个）的一个快捷方式是将 Instance作为一个公共静态只读变量，并完全删除该属性。
    ///     这使得基本的框架代码非常小！然而，许多人更愿意拥有一个属性，以防将来需要采取进一步行动，而JIT内联可能会使性能相同。
    ///     （注意，如果您需要懒惰的，静态构造函数本身仍然是必需的。）
    /// </summary>
    public sealed class Singleton4
    {
        private static readonly Singleton4 instance = new Singleton4();

        // 显式静态构造函数告诉C＃编译器
        // 不要将类型标记为BeforeFieldInit
        static Singleton4()
        {
        }

        private Singleton4()
        {
        }

        public static Singleton4 Instance
        {
            get
            {
                return instance;
            }
        }
    }

    /// <summary>
    /// 第五版 - 完全懒惰的实例化
    /// 
    /// 在这里，实例化是由对嵌套类的静态成员的第一次引用触发的，该引用只发生在Instance中。
    ///     这意味着实现是完全懒惰的，但是具有前面实现的所有性能优势。
    ///     请注意，尽管嵌套类可以访问封闭类的私有成员，但反之则不然，因此需要instance在此处为内部成员。
    ///     不过，这不会引起任何其他问题，因为类本身是私有的。但是，为了使实例化变得懒惰，代码要稍微复杂一些。
    /// </summary>
    public sealed class Singleton5
    {
        private Singleton5()
        {
        }

        public static Singleton5 Instance { get { return Nested.instance; } }

        private class Nested
        {
            // 显式静态构造告诉C＃编译器
            // 未标记类型BeforeFieldInit
            static Nested()
            {
            }

            internal static readonly Singleton5 instance = new Singleton5();
        }
    }


    /// <summary>
    /// 第六版 - 使用.NET 4的 Lazy 类型
    /// 如果您使用的是.NET 4（或更高版本），则可以使用 System.Lazy 类型使惰性变得非常简单。您需要做的就是将委托传递给调用Singleton构造函数的构造函数——使用lambda表达式最容易做到这一点。
    /// 
    /// 它很简单，而且性能很好。它还允许您检查是否已使用IsValueCreated 属性创建实例（如果需要的话）。
    /// 上面的代码隐式地将LazyThreadSafetyMode.ExecutionAndPublication用作Lazy<Singleton> 的线程安全模式。根据您的要求，您可能希望尝试其他模式。
    /// </summary>
    public sealed class Singleton6
    {
        private static readonly Lazy<Singleton6> lazy =
            new Lazy<Singleton6>(() => new Singleton6());

        public static Singleton6 Instance { get { return lazy.Value; } }

        private Singleton6()
        {
        }
    }

}
