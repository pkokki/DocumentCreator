using DocumentCreator;
using DocumentCreator.ExcelFormulaParser.Languages;
using DocumentCreator.Model;
using DocumentCreator.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentCreatorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EvaluationsController : ControllerBase
    {
        private readonly IRepository repository;

        public EvaluationsController(IRepository repository)
        {
            this.repository = repository;
        }
        
        [HttpGet]
        public string Get()
        {
            return "Hello, Evaluations!";
        }

        [HttpPost]
        public IActionResult TestEvaluations(EvaluationRequest request)
        {
            IEnumerable<TemplateField> templateFields = null;
            if (!string.IsNullOrEmpty(request.TemplateName))
            {
                var template = repository.GetLatestTemplate(request.TemplateName);
                if (template == null)
                    return NotFound();
                var templateProcessor = new TemplateProcessor();
                templateFields = templateProcessor.FindTemplateFields(template.Buffer);
            }

            var processor = new ExpressionEvaluator();
            var response = processor.Evaluate(request, templateFields);
            return Ok(response);
        }
    }
}
