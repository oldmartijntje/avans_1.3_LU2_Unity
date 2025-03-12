using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.scripts.Models
{
    public class Environment2DTemplate
    {
        public string Name { get; set; } = "";

        [Range(10, 100)]
        public int MaxHeight { get; set; }

        [Range(20, 200)]
        public int MaxLength { get; set; }

    }

    public class Environment2D : Environment2DTemplate
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "";

    }

    public class FullEnvironment2DObject
    {
        public int environmentId { get; set; } 
        public Environment2D environmentData { get; set; }
        public List<Object2D> environmentObjects { get; set; }
    }
}
