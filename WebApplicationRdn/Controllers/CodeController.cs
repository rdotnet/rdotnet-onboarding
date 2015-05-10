using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using RDotNet;
using RDotNet.Graphics;
using Svg;
using WebApplicationRdn.Models;
using System.Web.Mvc;

namespace WebApplicationRdn.Controllers
{

    public class RCode
    {
        public int Id { get; set; }
        public string Language { get; set; }
        public string Block { get; set; }
        public RResult Result { get; set; }
    }

    public class RResult
    {
        public List<string> Console { get; set; }
        public List<string> Plots { get; set; }
        public List<string> Errors { get; set; }
        public List<RInternalResult> InternalResults { get; set; }

        public static RResult Empty = new RResult();
    }

    public class CodeController : ApiController
    {
        private static REngine _engine;
        private static readonly SvgGraphicsDevice GraphicsDevice = new SvgGraphicsDevice(new SvgContextMapper(700, 700, SvgUnitType.Pixel, null));
        private static readonly CharacterDevice CharacterDevice = new CharacterDevice();
        private static readonly SymbolicExpressionToResultMapper Mapper = new SymbolicExpressionToResultMapper();

        public CodeController()
        {
            if (_engine != null) return;

            _engine = REngine.GetInstance(null, true, null, CharacterDevice);
            _engine.Initialize();
            _engine.Install(GraphicsDevice);
        }

        public RCode Execute(int id, [FromBody] RCode code)
        {
            GraphicsDevice.ClearImages();

            if (string.IsNullOrEmpty(code.Block))
            {
                code.Result = RResult.Empty;
                return code;
            }

            var statements = code.Block.Split(new[] { ';', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries); //TODO: Use tokenizer.
            try
            {
                CharacterDevice.ResetConsole();
                var evaluated = statements.Select(_engine.Evaluate).ToList();
                var plots = GraphicsDevice.GetImages().Select(RenderSvg).ToList();
                var console = CharacterDevice.GetOutput().ToList();
                var internalResults = evaluated.Select(Mapper.Map).ToList();

                var result = new RResult
                {
                        Plots = plots,
                        Console = console,
                        InternalResults = internalResults
                };


                code.Result = result;
                return code;
            }
            catch (Exception ex)
            {
                //TODO: Service reliability logging
                return CreateErrorResult(code, ex);
            }
        }

        private static string RenderSvg(SvgDocument image)
        {
            using (var stream = new MemoryStream())
            {
                image.Write(stream);
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    var contents = reader.ReadToEnd();
                    return contents;
                }
            }
        }

        private static RCode CreateErrorResult(RCode code, Exception ex)
        {
            code.Result = new RResult { Errors = new List<string> { ex.Message } };
            return code;
        }
    }

    public class HomeController : Controller
    {
        public ViewResult Index()
        {
            return View();
        }
    }
}
