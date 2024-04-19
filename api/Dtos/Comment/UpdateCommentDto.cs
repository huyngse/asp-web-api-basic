using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Comment
{
    public class UpdateCommentDto
    {
        [Required]
        [MinLength(5)]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;
        [Required]
        [MinLength(5)]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;
    }
}
