using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        //// GET api/values
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/values/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/values
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
        //[HttpPost]
        //public string CreateUser(User user)
        //{
        //    //return user.CreateUser().ToString();
        //}

        //[HttpPost]
        //public string UpdateUser(User user)
        //{
        //    return user.UpdateUser().ToString();
        //}

        //[HttpGet("{id}")]
        //public User GetUser(int id)
        //{
        //    return Models.User.GetUser(id);
        //}

        //[HttpGet]
        //public List<User> ListUsers()
        //{
        //    return Models.User.ListUsers();
        //}
    }
}
