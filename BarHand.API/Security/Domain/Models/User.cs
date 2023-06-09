﻿using System.Text.Json.Serialization;

namespace BarHand.API.Security.Domain.Models;

public class User
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string LastName { get; set; }
   
    public string Role { get; set; }
    
    //Possible Role Attribute
    [JsonIgnore]
    public string PasswordHash { get; set; }
}