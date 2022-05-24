using ExampleWebApi.Services;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ExampleWebApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ExampleController : ApiController
    {
        private readonly ExampleService _exampleService;

        public ExampleController(ExampleService exampleService)
        {
            _exampleService = exampleService;
        }

        [HttpGet]
        [Route("example/list")]
        public IHttpActionResult ListItems()
        {
            return Ok(_exampleService.ListItems());
        }
    }
}
