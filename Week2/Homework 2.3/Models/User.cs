﻿using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;

namespace M101DotNet.WebApp.Models
{
    public class User
    {
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }
    }
}