using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
}
