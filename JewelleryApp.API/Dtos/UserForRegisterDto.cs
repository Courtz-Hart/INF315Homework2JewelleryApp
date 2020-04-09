using System.ComponentModel.DataAnnotations;

namespace JewelleryApp.API.Dtos
{
    public class UserForRegisterDto
    {

        [Required]
        public string Username { get; set; }


        [Required] //How to do validations in square brackets
        [StringLength(8, MinimumLength = 4, ErrorMessage = "You musy sepcify password between 4 and 8 characters")]
        public string Password { get; set; }
    }
}