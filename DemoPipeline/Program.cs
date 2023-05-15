using System.Text;

namespace DemoPipeline
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int zero = 0;
            Pipeline pipeline = new Pipeline();
            pipeline.AddMiddleware(new Middleware((context) => context.Add("Noeud 1")));
            pipeline.AddMiddleware(new Middleware((context) => context.Add("Noeud 2")));
            pipeline.AddMiddleware(new Middleware((context) => context.Add("Noeud 3")));
            pipeline.AddMiddleware(new Middleware((context) => context.Add("Noeud 4")));
            pipeline.AddMiddleware(new Middleware((context) => context.Add("Noeud 5")));
            pipeline.AddMiddleware(new Middleware((context) => context.Add("Noeud 6")));
            pipeline.AddMiddleware(new Middleware((context) => context.Add((2 / zero).ToString())));

            PipelineContext pipelineContext = pipeline.Start();
            Console.WriteLine(pipelineContext.Result);
        }
    }

    class Pipeline
    {
        private Middleware? _main;

        public Pipeline() 
        { 
        }

        public void AddMiddleware(Middleware middleware)
        {
            if(_main is null)
                _main = middleware;
            else
            {
                Middleware current = _main;
                while(current.Next is not null)
                {
                    current = current.Next;
                }

                current.Next = middleware;
            }
        }

        public PipelineContext Start()
        {
            PipelineContext context = new PipelineContext();

            try
            {
                if (_main is not null)
                    _main.Execute(context);
            }
            catch (Exception ex)
            {
                context.Add(ex);
            }


            return context;
        }
    }

    class Middleware
    {
        private Action<PipelineContext> _execute;

        public Middleware(Action<PipelineContext> execute)
        {
            _execute = execute;
        }

        public Middleware? Next { get; set; }

        public void Execute(PipelineContext pipelineContext)
        {
            _execute(pipelineContext);

            if (Next is not null)
            {                
                Next.Execute(pipelineContext);

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