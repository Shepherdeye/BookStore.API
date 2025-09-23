﻿using System.ComponentModel.DataAnnotations;

namespace BookStore.API.DTOs.Request
{
    public class RegisterDTO
    {

        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public string UserName { get; set; }= string.Empty;

        [Required,DataType(DataType.EmailAddress)]
        public string Email { get; set; }= string.Empty;

        [Required,DataType(DataType.Password)]
        public string Password { get; set; }= string.Empty;

        [Required, DataType(DataType.Password),Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }= string.Empty;
        public string? Addresse { get; set; }


    }
}
