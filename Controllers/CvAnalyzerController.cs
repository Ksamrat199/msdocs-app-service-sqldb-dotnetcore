using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreSqlDb.Dtos;
using DotNetCoreSqlDb.Models.CvAnalysis;
using DotNetCoreSqlDb.Services.CvAnalysis.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreSqlDb.Controllers
{
    [ApiController]
    [Route("api/cv")] 
    public class CvAnalyzerController : ControllerBase
    {
        private readonly ICvAnalyzer _analyzer;
        private readonly IGroupingService _groupingService;

        public CvAnalyzerController(ICvAnalyzer analyzer, IGroupingService groupingService)
        {
            _analyzer = analyzer;
            _groupingService = groupingService;
        }

        [HttpPost("analyze")]
        [RequestSizeLimit(100_000_000)]
        public async Task<ActionResult<AnalyzeResponse>> Analyze([FromForm] IFormFile[] files)
        {
            if (files == null || files.Length == 0)
            {
                return BadRequest("No files uploaded");
            }

            var inputs = new List<CvInput>(files.Length);
            foreach (var file in files)
            {
                if (file.Length <= 0)
                {
                    continue;
                }

                await using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                inputs.Add(new CvInput
                {
                    Name = Path.GetFileNameWithoutExtension(file.FileName),
                    FileName = file.FileName,
                    Bytes = ms.ToArray()
                });
            }

            var analysis = await _analyzer.AnalyzeAsync(inputs);
            var groups = await _groupingService.GroupBySkillsAsync(analysis.Profiles);

            return Ok(new AnalyzeResponse
            {
                Result = analysis,
                Groups = groups.ToList()
            });
        }

        [HttpPost("analyze-json")]
        public async Task<ActionResult<AnalyzeResponse>> AnalyzeJson([FromBody] List<CvFileJsonDto> files)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest("Empty request body");
            }

            var inputs = new List<CvInput>(files.Count);
            foreach (var f in files)
            {
                try
                {
                    var bytes = Convert.FromBase64String(f.Base64);
                    inputs.Add(new CvInput
                    {
                        Name = string.IsNullOrWhiteSpace(f.Name) ? Path.GetFileNameWithoutExtension(f.FileName) : f.Name,
                        FileName = f.FileName,
                        Bytes = bytes
                    });
                }
                catch (FormatException)
                {
                    return BadRequest($"Invalid Base64 for file: {f.FileName}");
                }
            }

            var analysis = await _analyzer.AnalyzeAsync(inputs);
            var groups = await _groupingService.GroupBySkillsAsync(analysis.Profiles);

            return Ok(new AnalyzeResponse
            {
                Result = analysis,
                Groups = groups.ToList()
            });
        }
    }
}
