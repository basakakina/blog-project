using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WEB.Areas.Admin.Models
{
    public class PostVM
    {
        public Guid Id { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(200)]
        public string Slug { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]        
        public Guid? CategoryId { get; set; }   
        public List<SelectListItem> Categories { get; set; } = new();

        public bool IsPublished { get; set; }

        public List<Guid> SelectedTagIds { get; set; } = new();
        public List<SelectListItem> Tags { get; set; } = new();

       
    }
}
