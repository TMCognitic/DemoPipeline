using System.Text;

namespace DemoPipeline
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int zero = 0;
            Pipeline pipeline = new Pipeline();
            pipeline.AddMiddleware(new ActionMiddleware((context) => context.Add("Noeud 1")));
            pipeline.AddMiddleware(new ActionMiddleware((context) => context.Add("Noeud 2")));
            pipeline.AddMiddleware(new ActionMiddleware((context) => context.Add("Noeud 3")));
            pipeline.AddMiddleware(new ActionMiddleware((context) => context.Add("Noeud 4")));
            pipeline.AddMiddleware(new ActionMiddleware((context) => context.Add("Noeud 5")));
            pipeline.AddMiddleware(new ActionMiddleware((context) => context.Add("Noeud 6")));
            //pipeline.AddMiddleware(new ActionMiddleware((context) => context.Add((2 / zero).ToString())));

            PipelineContext pipelineContext = pipeline.Start();
            Console.WriteLine(pipelineContext.Result);
        }
    }

    class Pipeline
    {
        private Middleware _main;

        public Pipeline() 
        {
            _main = new ExceptionMiddleware();
        }

        public void AddMiddleware(Middleware middleware)
        {
            Middleware current = _main;
            while(current.Next is not null)
            {
                current = current.Next;
            }
            current.Next = middleware;
        }

        public PipelineContext Start()
        {
            PipelineContext context = new PipelineContext();
            _main.Execute(context);
            return context;
        }
    }

    abstract class Middleware
    {
        protected Middleware()
        {
        }

        public Middleware? Next { get; set; }

        public abstract void Execute(PipelineContext pipelineContext);
    }

    class ActionMiddleware : Middleware
    {
        private Action<PipelineContext> _execute;

        public ActionMiddleware(Action<PipelineContext> execute)
        {
            _execute = execute;
        }

        public override void Execute(PipelineContext pipelineContext)
        {
            _execute(pipelineContext);

            if (Next is not null)
            {
                Next.Execute(pipelineContext);

            }
        }
    }

    class ExceptionMiddleware : Middleware
    {
        public ExceptionMiddleware()
        {            
        }

        public override void Execute(PipelineContext context)
        {
            try
            {
                if (Next is not null)
                {
                    Next.Execute(context);
                }
            }
            catch (Exception ex)
            {
                context.Add(ex);
            }            
        }
    }

    class PipelineContext
    {
        private StringBuilder StringBuilder { get; init; }
        public string Result { get { return StringBuilder.ToString(); } }

        public PipelineContext()
        {
            StringBuilder = new StringBuilder();
        }

        public void Add(string texte)
        {
            StringBuilder.AppendLine(texte);
        }

        public void Add(Exception ex)
        {
            StringBuilder.Clear();
            StringBuilder.AppendLine(ex.Message);
        }
    }
}