using Models;
using Soap_Basic.Interfaces;
using Soap_Basic.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Soap_Basic
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Encap" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Encap.svc or Encap.svc.cs at the Solution Explorer and start debugging.
    public class Encap : IUserEncap
    {
        public UserResponse CreateUser(User user)
        {
            UserResponse resp = new UserResponse();
            try
            {
                int id = user.CreateUser();
                if( id > 0)
                {
                    resp.WasSuccessful = true;
                    resp.ID = id;
                }
                else
                {
                    resp.WasSuccessful = false;
                    resp.Error = "Your friendly error message.";
                }
            }
            catch(Exception ex)
            {
                resp.WasSuccessful = false;
                resp.Error = "Your friendly error message.";
            }
            return resp;
        }

        public UserResponse UpdateUser(User user)
        {
            UserResponse resp = new UserResponse();
            try
            {
                if (user.UpdateUser())
                {
                    resp.WasSuccessful = true;
                }
                else
                {
                    resp.WasSuccessful = false;
                    resp.Error = "Your friendly error message.";
                }
            }
            catch (Exception ex)
            {
                resp.WasSuccessful = false;
                resp.Error = "Your friendly error message.";
            }
            return resp;
        }

        public UserResponse GetUser(int id)
        {
            UserResponse resp = new UserResponse();
            try
            {
                User user = User.GetUser(id);
                if(user != null)
                {
                    resp.UserList = new List<User>();
                    resp.UserList.Add(user);
                    resp.WasSuccessful = true;
                }
                else
                {
                    resp.WasSuccessful = false;
                    resp.Error = "Your friendly error message.";
                }
            }
            catch (Exception ex)
            {
                resp.WasSuccessful = false;
                resp.Error = "Your friendly error message.";
            }
            return resp;
        }

        public UserResponse ListUsers()
        {
            UserResponse resp = new UserResponse();
            try
            {
                List<User> list = User.ListUsers();
                if(list != null)
                {
                    resp.WasSuccessful = true;
                    resp.UserList = list;
                }
                else
                {
                    resp.WasSuccessful = false;
                    resp.Error = "Your friendly error message.";
                }
            }
            catch (Exception ex)
            {
                resp.WasSuccessful = false;
                resp.Error = "Your friendly error message.";
            }
            return resp;
        }
    }
}
